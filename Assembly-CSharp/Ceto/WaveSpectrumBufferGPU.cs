using Ceto.Common.Unity.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	public class WaveSpectrumBufferGPU : WaveSpectrumBuffer
	{
		private struct Buffer
		{
			public RenderTexture[] data;

			public bool disabled;
		}

		private WaveSpectrumBufferGPU.Buffer[] m_buffers;

		private bool m_samplingEnabled;

		private int m_index = -1;

		private FourierGPU m_fourier;

		private IList<RenderTexture[]> m_enabledData;

		private string m_bufferName;

		private IList<RenderTexture> m_tmpList;

		private RenderBuffer[] m_tmpBuffer2;

		private RenderBuffer[] m_tmpBuffer3;

		private RenderBuffer[] m_tmpBuffer4;

		private Vector4 m_offset;

		public override bool Done
		{
			get
			{
				return true;
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
				return true;
			}
		}

		public WaveSpectrumBufferGPU(int size, Shader fourierSdr, int numBuffers)
		{
			if (numBuffers < 1 || numBuffers > 4)
			{
				throw new InvalidOperationException("Number of buffers is " + numBuffers + " but must be between (inclusive) 1 and 4");
			}
			this.m_buffers = new WaveSpectrumBufferGPU.Buffer[numBuffers];
			this.m_fourier = new FourierGPU(size, fourierSdr);
			this.m_tmpList = new List<RenderTexture>();
			this.m_tmpBuffer2 = new RenderBuffer[2];
			this.m_tmpBuffer3 = new RenderBuffer[3];
			this.m_tmpBuffer4 = new RenderBuffer[4];
			this.m_offset = new Vector4(1f + 0.5f / (float)this.Size, 1f + 0.5f / (float)this.Size, 0f, 0f);
			for (int i = 0; i < numBuffers; i++)
			{
				this.m_buffers[i] = this.CreateBuffer(size);
			}
			this.m_enabledData = new List<RenderTexture[]>();
			this.UpdateEnabledData();
			this.m_bufferName = "Ceto Wave Spectrum GPU Buffer";
		}

		private WaveSpectrumBufferGPU.Buffer CreateBuffer(int size)
		{
			return new WaveSpectrumBufferGPU.Buffer
			{
				data = new RenderTexture[2]
			};
		}

		public override Texture GetTexture(int idx)
		{
			if (this.m_index == -1)
			{
				return Texture2D.blackTexture;
			}
			if (idx < 0 || idx >= this.m_buffers.Length)
			{
				return Texture2D.blackTexture;
			}
			if (this.m_buffers[idx].disabled)
			{
				return Texture2D.blackTexture;
			}
			return this.m_buffers[idx].data[this.m_index];
		}

		public override void Release()
		{
			this.m_tmpList.Clear();
			this.m_fourier.Release();
			int num = this.m_buffers.Length;
			for (int i = 0; i < num; i++)
			{
				RTUtility.ReleaseAndDestroy(this.m_buffers[i].data);
				this.m_buffers[i].data[0] = null;
				this.m_buffers[i].data[1] = null;
			}
		}

		protected override void Initilize(WaveSpectrumCondition condition, float time)
		{
			if (base.InitMaterial == null)
			{
				throw new InvalidOperationException("GPU buffer has not had its Init material set");
			}
			if (base.InitPass == -1)
			{
				throw new InvalidOperationException("GPU buffer has not had its Init material pass set");
			}
			base.InitMaterial.SetTexture("Ceto_Spectrum01", (!(condition.Spectrum01 != null)) ? Texture2D.blackTexture : condition.Spectrum01);
			base.InitMaterial.SetTexture("Ceto_Spectrum23", (!(condition.Spectrum23 != null)) ? Texture2D.blackTexture : condition.Spectrum23);
			base.InitMaterial.SetTexture("Ceto_WTable", condition.WTable);
			base.InitMaterial.SetVector("Ceto_InverseGridSizes", condition.InverseGridSizes());
			base.InitMaterial.SetVector("Ceto_GridSizes", condition.GridSizes);
			base.InitMaterial.SetVector("Ceto_Offset", this.m_offset);
			base.InitMaterial.SetFloat("Ceto_Time", time);
			this.m_tmpList.Clear();
			int num = this.m_buffers.Length;
			for (int i = 0; i < num; i++)
			{
				if (!this.m_buffers[i].disabled)
				{
					this.m_tmpList.Add(this.m_buffers[i].data[1]);
				}
			}
			num = this.m_tmpList.Count;
			if (num == 0)
			{
				return;
			}
			if (num == 1)
			{
				Graphics.Blit(null, this.m_tmpList[0], base.InitMaterial, base.InitPass);
			}
			else if (num == 2)
			{
				this.m_tmpBuffer2[0] = this.m_tmpList[0].colorBuffer;
				this.m_tmpBuffer2[1] = this.m_tmpList[1].colorBuffer;
				RTUtility.MultiTargetBlit(this.m_tmpBuffer2, this.m_tmpList[0].depthBuffer, base.InitMaterial, base.InitPass);
			}
			else if (num == 3)
			{
				this.m_tmpBuffer3[0] = this.m_tmpList[0].colorBuffer;
				this.m_tmpBuffer3[1] = this.m_tmpList[1].colorBuffer;
				this.m_tmpBuffer3[2] = this.m_tmpList[2].colorBuffer;
				RTUtility.MultiTargetBlit(this.m_tmpBuffer3, this.m_tmpList[0].depthBuffer, base.InitMaterial, base.InitPass);
			}
			else if (num == 4)
			{
				this.m_tmpBuffer4[0] = this.m_tmpList[0].colorBuffer;
				this.m_tmpBuffer4[1] = this.m_tmpList[1].colorBuffer;
				this.m_tmpBuffer4[2] = this.m_tmpList[2].colorBuffer;
				this.m_tmpBuffer4[3] = this.m_tmpList[3].colorBuffer;
				RTUtility.MultiTargetBlit(this.m_tmpBuffer4, this.m_tmpList[0].depthBuffer, base.InitMaterial, base.InitPass);
			}
		}

		public void UpdateEnabledData()
		{
			this.m_enabledData.Clear();
			int num = this.m_buffers.Length;
			for (int i = 0; i < num; i++)
			{
				if (!this.m_buffers[i].disabled)
				{
					this.m_enabledData.Add(this.m_buffers[i].data);
				}
			}
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
			this.UpdateEnabledData();
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
			this.UpdateEnabledData();
		}

		public override int EnabledBuffers()
		{
			return this.m_enabledData.Count;
		}

		public override bool IsEnabledBuffer(int idx)
		{
			return idx >= 0 && idx < this.m_buffers.Length && !this.m_buffers[idx].disabled;
		}

		private void CreateTextures()
		{
			int count = this.m_enabledData.Count;
			for (int i = 0; i < count; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					if (this.m_enabledData[i][j] != null)
					{
						RenderTexture.ReleaseTemporary(this.m_enabledData[i][j]);
					}
					RenderTexture temporary = RenderTexture.GetTemporary(this.Size, this.Size, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
					temporary.filterMode = FilterMode.Point;
					temporary.wrapMode = TextureWrapMode.Clamp;
					temporary.name = this.m_bufferName;
					temporary.anisoLevel = 0;
					temporary.Create();
					this.m_enabledData[i][j] = temporary;
				}
			}
		}

		public override void Run(WaveSpectrumCondition condition, float time)
		{
			base.TimeValue = time;
			base.HasRun = true;
			base.BeenSampled = false;
			if (this.m_samplingEnabled)
			{
				throw new InvalidOperationException("Can not run if sampling enabled");
			}
			this.UpdateEnabledData();
			this.CreateTextures();
			int count = this.m_enabledData.Count;
			if (count == 0)
			{
				return;
			}
			this.Initilize(condition, time);
			if (count == 1)
			{
				this.m_index = this.m_fourier.PeformFFT(this.m_enabledData[0]);
			}
			else if (count == 2)
			{
				this.m_index = this.m_fourier.PeformFFT(this.m_enabledData[0], this.m_enabledData[1]);
			}
			else if (count == 3)
			{
				this.m_index = this.m_fourier.PeformFFT(this.m_enabledData[0], this.m_enabledData[1], this.m_enabledData[2]);
			}
			else if (count == 4)
			{
				this.m_index = this.m_fourier.PeformFFT(this.m_enabledData[0], this.m_enabledData[1], this.m_enabledData[2], this.m_enabledData[3]);
			}
		}

		public override void EnableSampling()
		{
			if (this.m_index == -1)
			{
				return;
			}
			this.m_samplingEnabled = true;
			int num = this.m_buffers.Length;
			for (int i = 0; i < num; i++)
			{
				if (!(this.m_buffers[i].data[this.m_index] == null))
				{
					this.m_buffers[i].data[this.m_index].filterMode = FilterMode.Bilinear;
					this.m_buffers[i].data[this.m_index].wrapMode = TextureWrapMode.Repeat;
				}
			}
		}

		public override void DisableSampling()
		{
			if (this.m_index == -1)
			{
				return;
			}
			this.m_samplingEnabled = false;
			int num = this.m_buffers.Length;
			for (int i = 0; i < num; i++)
			{
				if (!(this.m_buffers[i].data[this.m_index] == null))
				{
					this.m_buffers[i].data[this.m_index].filterMode = FilterMode.Point;
					this.m_buffers[i].data[this.m_index].wrapMode = TextureWrapMode.Clamp;
				}
			}
		}
	}
}
