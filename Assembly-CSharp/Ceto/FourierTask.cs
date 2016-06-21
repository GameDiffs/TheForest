using Ceto.Common.Containers.Interpolation;
using Ceto.Common.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	public class FourierTask : ThreadedTask
	{
		private FourierCPU m_fourier;

		private DisplacementBufferCPU m_buffer;

		private int m_numGrids;

		private int m_index;

		private IList<Vector4[]> m_data;

		private Color[] m_results;

		private Texture2D m_map;

		private bool m_doublePacked;

		public FourierTask(WaveSpectrumBufferCPU buffer, FourierCPU fourier, int index, int numGrids) : base(true)
		{
			if (this.m_index == -1)
			{
				throw new InvalidOperationException("Index can be -1. Fourier for multiple buffers is not being used");
			}
			if (!(buffer is DisplacementBufferCPU))
			{
				throw new InvalidOperationException("Fourier task currently only designed for displacement buffers");
			}
			this.m_buffer = (buffer as DisplacementBufferCPU);
			this.m_fourier = fourier;
			this.m_index = index;
			this.m_numGrids = numGrids;
			WaveSpectrumBufferCPU.Buffer buffer2 = this.m_buffer.GetBuffer(this.m_index);
			this.m_data = buffer2.data;
			this.m_results = buffer2.results;
			this.m_map = buffer2.map;
			this.m_doublePacked = buffer2.doublePacked;
		}

		public void Reset(int index, int numGrids)
		{
			base.Reset();
			if (this.m_index == -1)
			{
				throw new InvalidOperationException("Index can be -1. Fourier for multiple buffers is not being used");
			}
			this.m_index = index;
			this.m_numGrids = numGrids;
			WaveSpectrumBufferCPU.Buffer buffer = this.m_buffer.GetBuffer(this.m_index);
			this.m_data = buffer.data;
			this.m_results = buffer.results;
			this.m_map = buffer.map;
			this.m_doublePacked = buffer.doublePacked;
		}

		public override void Start()
		{
			base.Start();
		}

		public override IEnumerator Run()
		{
			this.PerformSingleFourier();
			this.FinishedRunning();
			return null;
		}

		public override void End()
		{
			base.End();
			this.m_map.SetPixels(this.m_results);
			this.m_map.Apply();
		}

		private void PerformSingleFourier()
		{
			int num;
			if (this.m_doublePacked)
			{
				num = this.m_fourier.PeformFFT_DoublePacked(0, this.m_data, this);
			}
			else
			{
				num = this.m_fourier.PeformFFT_SinglePacked(0, this.m_data, this);
			}
			if (this.Cancelled)
			{
				return;
			}
			if (num != 1)
			{
				throw new InvalidOperationException("Fourier transform did not result in the read buffer at index " + 1);
			}
			this.ProcessData(this.m_index, this.m_results, this.m_data[num], this.m_numGrids);
		}

		private void ProcessData(int index, Color[] result, Vector4[] data, int numGrids)
		{
			int cHANNELS = QueryDisplacements.CHANNELS;
			int size = this.m_buffer.Size;
			InterpolatedArray2f[] writeDisplacements = this.m_buffer.GetWriteDisplacements();
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					int num = j + i * size;
					int num2 = num * cHANNELS;
					if (numGrids == 1)
					{
						result[num].r = data[num].x;
						result[num].g = data[num].y;
						result[num].b = 0f;
						result[num].a = 0f;
						if (index == 0)
						{
							writeDisplacements[0].Data[num2 + 1] = result[num].r;
						}
						else if (index == 1)
						{
							writeDisplacements[0].Data[num2] += result[num].r;
							writeDisplacements[0].Data[num2 + 2] += result[num].g;
						}
					}
					else if (numGrids == 2)
					{
						result[num].r = data[num].x;
						result[num].g = data[num].y;
						result[num].b = data[num].z;
						result[num].a = data[num].w;
						if (index == 0)
						{
							writeDisplacements[0].Data[num2 + 1] = result[num].r;
							writeDisplacements[1].Data[num2 + 1] = result[num].g;
						}
						else if (index == 1)
						{
							writeDisplacements[0].Data[num2] += result[num].r;
							writeDisplacements[0].Data[num2 + 2] += result[num].g;
							writeDisplacements[1].Data[num2] += result[num].b;
							writeDisplacements[1].Data[num2 + 2] += result[num].a;
						}
					}
					else if (numGrids == 3)
					{
						result[num].r = data[num].x;
						result[num].g = data[num].y;
						result[num].b = data[num].z;
						result[num].a = data[num].w;
						if (index == 0)
						{
							writeDisplacements[0].Data[num2 + 1] = result[num].r;
							writeDisplacements[1].Data[num2 + 1] = result[num].g;
							writeDisplacements[2].Data[num2 + 1] = result[num].b;
							writeDisplacements[3].Data[num2 + 1] = result[num].a;
						}
						else if (index == 1)
						{
							writeDisplacements[0].Data[num2] += result[num].r;
							writeDisplacements[0].Data[num2 + 2] += result[num].g;
							writeDisplacements[1].Data[num2] += result[num].b;
							writeDisplacements[1].Data[num2 + 2] += result[num].a;
						}
						else if (index == 2)
						{
							writeDisplacements[2].Data[num2] += result[num].r;
							writeDisplacements[2].Data[num2 + 2] += result[num].g;
						}
					}
					else if (numGrids == 4)
					{
						result[num].r = data[num].x;
						result[num].g = data[num].y;
						result[num].b = data[num].z;
						result[num].a = data[num].w;
						if (index == 0)
						{
							writeDisplacements[0].Data[num2 + 1] = result[num].r;
							writeDisplacements[1].Data[num2 + 1] = result[num].g;
							writeDisplacements[2].Data[num2 + 1] = result[num].b;
							writeDisplacements[3].Data[num2 + 1] = result[num].a;
						}
						else if (index == 1)
						{
							writeDisplacements[0].Data[num2] += result[num].r;
							writeDisplacements[0].Data[num2 + 2] += result[num].g;
							writeDisplacements[1].Data[num2] += result[num].b;
							writeDisplacements[1].Data[num2 + 2] += result[num].a;
						}
						else if (index == 2)
						{
							writeDisplacements[2].Data[num2] += result[num].r;
							writeDisplacements[2].Data[num2 + 2] += result[num].g;
							writeDisplacements[3].Data[num2] += result[num].b;
							writeDisplacements[3].Data[num2 + 2] += result[num].a;
						}
					}
					else
					{
						result[num].r = 0f;
						result[num].g = 0f;
						result[num].b = 0f;
						result[num].a = 0f;
					}
				}
			}
		}
	}
}
