using Ceto.Common.Threading.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	public abstract class WaveSpectrumBufferCPU : WaveSpectrumBuffer
	{
		public class Buffer
		{
			public IList<Vector4[]> data;

			public Color[] results;

			public Texture2D map;

			public bool disabled;

			public bool doublePacked;
		}

		public const int READ = 1;

		public const int WRITE = 0;

		protected WaveSpectrumBufferCPU.Buffer[] m_buffers;

		private FourierCPU m_fourier;

		private Scheduler m_scheduler;

		private List<FourierTask> m_fourierTasks;

		protected InitSpectrumDisplacementsTask m_initTask;

		public override bool Done
		{
			get
			{
				return this.IsDone();
			}
		}

		public override int Size
		{
			get
			{
				return this.m_fourier.size;
			}
		}

		public override bool IsGPU
		{
			get
			{
				return false;
			}
		}

		public Color[] WTable
		{
			get;
			private set;
		}

		public WaveSpectrumBufferCPU(int size, int numBuffers, Scheduler scheduler)
		{
			this.m_buffers = new WaveSpectrumBufferCPU.Buffer[numBuffers];
			this.m_fourier = new FourierCPU(size);
			this.m_fourierTasks = new List<FourierTask>(3);
			this.m_fourierTasks.Add(null);
			this.m_fourierTasks.Add(null);
			this.m_fourierTasks.Add(null);
			this.m_scheduler = scheduler;
			for (int i = 0; i < numBuffers; i++)
			{
				this.m_buffers[i] = this.CreateBuffer(size);
			}
		}

		private WaveSpectrumBufferCPU.Buffer CreateBuffer(int size)
		{
			WaveSpectrumBufferCPU.Buffer buffer = new WaveSpectrumBufferCPU.Buffer();
			buffer.doublePacked = true;
			buffer.data = new List<Vector4[]>();
			buffer.data.Add(new Vector4[size * size]);
			buffer.data.Add(new Vector4[size * size]);
			buffer.results = new Color[size * size];
			buffer.map = new Texture2D(size, size, TextureFormat.RGBAFloat, false, true);
			buffer.map.wrapMode = TextureWrapMode.Repeat;
			buffer.map.filterMode = FilterMode.Bilinear;
			buffer.map.hideFlags = HideFlags.HideAndDontSave;
			buffer.map.name = "Ceto Wave Spectrum CPU Buffer";
			buffer.map.SetPixels(buffer.results);
			buffer.map.Apply();
			return buffer;
		}

		public override void Release()
		{
			int num = this.m_buffers.Length;
			for (int i = 0; i < num; i++)
			{
				UnityEngine.Object.Destroy(this.m_buffers[i].map);
				this.m_buffers[i].map = null;
			}
		}

		public override Texture GetTexture(int idx)
		{
			if (idx < 0 || idx >= this.m_buffers.Length)
			{
				return Texture2D.blackTexture;
			}
			if (this.m_buffers[idx].disabled)
			{
				return Texture2D.blackTexture;
			}
			return this.m_buffers[idx].map;
		}

		public Vector4[] GetWriteBuffer(int idx)
		{
			if (idx < 0 || idx >= this.m_buffers.Length)
			{
				return null;
			}
			if (this.m_buffers[idx].disabled)
			{
				return null;
			}
			return this.m_buffers[idx].data[0];
		}

		public Vector4[] GetReadBuffer(int idx)
		{
			if (idx < 0 || idx >= this.m_buffers.Length)
			{
				return null;
			}
			if (this.m_buffers[idx].disabled)
			{
				return null;
			}
			return this.m_buffers[idx].data[1];
		}

		public WaveSpectrumBufferCPU.Buffer GetBuffer(int idx)
		{
			if (idx < 0 || idx >= this.m_buffers.Length)
			{
				return null;
			}
			if (this.m_buffers[idx].disabled)
			{
				return null;
			}
			return this.m_buffers[idx];
		}

		public override void EnableBuffer(int idx)
		{
			int num = this.m_buffers.Length;
			if (idx < -1 || idx >= num)
			{
				return;
			}
			if (idx == -1)
			{
				for (int i = 0; i < num; i++)
				{
					this.m_buffers[i].disabled = false;
				}
			}
			else
			{
				this.m_buffers[idx].disabled = false;
			}
		}

		public override void DisableBuffer(int idx)
		{
			int num = this.m_buffers.Length;
			if (idx < -1 || idx >= num)
			{
				return;
			}
			if (idx == -1)
			{
				for (int i = 0; i < num; i++)
				{
					this.m_buffers[i].disabled = true;
				}
			}
			else
			{
				this.m_buffers[idx].disabled = true;
			}
		}

		public bool IsDone()
		{
			if (this.m_initTask == null)
			{
				return true;
			}
			if (!this.m_initTask.Done)
			{
				return false;
			}
			int count = this.m_fourierTasks.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.m_fourierTasks[i] != null)
				{
					if (!this.m_fourierTasks[i].Done)
					{
						return false;
					}
				}
			}
			return true;
		}

		public override int EnabledBuffers()
		{
			int num = 0;
			int num2 = this.m_buffers.Length;
			for (int i = 0; i < num2; i++)
			{
				if (!this.m_buffers[i].disabled)
				{
					num++;
				}
			}
			return num;
		}

		public override bool IsEnabledBuffer(int idx)
		{
			return idx >= 0 && idx < this.m_buffers.Length && !this.m_buffers[idx].disabled;
		}

		public override void Run(WaveSpectrumCondition condition, float time)
		{
			if (!this.IsDone())
			{
				throw new InvalidOperationException("Can not run when there are tasks that have not finished");
			}
			base.TimeValue = time;
			base.HasRun = true;
			base.BeenSampled = false;
			if (this.EnabledBuffers() == 0)
			{
				return;
			}
			this.Initilize(condition, time);
			if (Ocean.DISABLE_FOURIER_MULTITHREADING)
			{
				this.m_initTask.Start();
				this.m_initTask.Run();
				this.m_initTask.End();
			}
			int num = this.m_buffers.Length;
			for (int i = 0; i < num; i++)
			{
				if (!this.m_buffers[i].disabled)
				{
					if (this.m_fourierTasks[i] == null)
					{
						this.m_fourierTasks[i] = new FourierTask(this, this.m_fourier, i, this.m_initTask.NumGrids);
					}
					else
					{
						this.m_fourierTasks[i].Reset(i, this.m_initTask.NumGrids);
					}
					FourierTask fourierTask = this.m_fourierTasks[i];
					if (fourierTask.Done)
					{
						throw new InvalidOperationException("Fourier task should not be done before running");
					}
					if (Ocean.DISABLE_FOURIER_MULTITHREADING)
					{
						fourierTask.Start();
						fourierTask.Run();
						fourierTask.End();
					}
					else
					{
						fourierTask.RunOnStopWaiting = true;
						fourierTask.WaitOn(this.m_initTask);
						this.m_scheduler.AddWaiting(fourierTask);
					}
				}
			}
			if (!Ocean.DISABLE_FOURIER_MULTITHREADING)
			{
				this.m_initTask.NoFinish = true;
				this.m_scheduler.Run(this.m_initTask);
			}
		}
	}
}
