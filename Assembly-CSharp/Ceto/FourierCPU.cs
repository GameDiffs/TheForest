using Ceto.Common.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	public class FourierCPU
	{
		private struct LookUp
		{
			public int j1;

			public int j2;

			public float wr;

			public float wi;
		}

		private int m_size;

		private float m_fsize;

		private int m_passes;

		private FourierCPU.LookUp[] m_butterflyLookupTable;

		public int size
		{
			get
			{
				return this.m_size;
			}
		}

		public int passes
		{
			get
			{
				return this.m_passes;
			}
		}

		public FourierCPU(int size)
		{
			if (!Mathf.IsPowerOfTwo(size))
			{
				throw new ArgumentException("Fourier grid size must be pow2 number");
			}
			this.m_size = size;
			this.m_fsize = (float)this.m_size;
			this.m_passes = (int)(Mathf.Log(this.m_fsize) / Mathf.Log(2f));
			this.ComputeButterflyLookupTable();
		}

		private int BitReverse(int i)
		{
			int num = 0;
			int num2 = 1;
			for (int num3 = this.m_size / 2; num3 != 0; num3 /= 2)
			{
				int num4 = ((i & num3) <= num3 - 1) ? 0 : 1;
				num += num4 * num2;
				num2 *= 2;
			}
			return num;
		}

		private void ComputeButterflyLookupTable()
		{
			this.m_butterflyLookupTable = new FourierCPU.LookUp[this.m_size * this.m_passes];
			for (int i = 0; i < this.m_passes; i++)
			{
				int num = (int)Mathf.Pow(2f, (float)(this.m_passes - 1 - i));
				int num2 = (int)Mathf.Pow(2f, (float)i);
				for (int j = 0; j < num; j++)
				{
					for (int k = 0; k < num2; k++)
					{
						int num3;
						int num4;
						int j2;
						int j3;
						if (i == 0)
						{
							num3 = j * num2 * 2 + k;
							num4 = j * num2 * 2 + num2 + k;
							j2 = this.BitReverse(num3);
							j3 = this.BitReverse(num4);
						}
						else
						{
							num3 = j * num2 * 2 + k;
							num4 = j * num2 * 2 + num2 + k;
							j2 = num3;
							j3 = num4;
						}
						float num5 = Mathf.Cos(6.28318548f * (float)(k * num) / this.m_fsize);
						float num6 = Mathf.Sin(6.28318548f * (float)(k * num) / this.m_fsize);
						int num7 = num3 + i * this.m_size;
						this.m_butterflyLookupTable[num7].j1 = j2;
						this.m_butterflyLookupTable[num7].j2 = j3;
						this.m_butterflyLookupTable[num7].wr = num5;
						this.m_butterflyLookupTable[num7].wi = num6;
						int num8 = num4 + i * this.m_size;
						this.m_butterflyLookupTable[num8].j1 = j2;
						this.m_butterflyLookupTable[num8].j2 = j3;
						this.m_butterflyLookupTable[num8].wr = -num5;
						this.m_butterflyLookupTable[num8].wi = -num6;
					}
				}
			}
		}

		private Vector4 FFT(Vector2 w, Vector4 input1, Vector4 input2)
		{
			input1.x += w.x * input2.x - w.y * input2.y;
			input1.y += w.y * input2.x + w.x * input2.y;
			input1.z += w.x * input2.z - w.y * input2.w;
			input1.w += w.y * input2.z + w.x * input2.w;
			return input1;
		}

		private Vector2 FFT(Vector2 w, Vector2 input1, Vector2 input2)
		{
			input1.x += w.x * input2.x - w.y * input2.y;
			input1.y += w.y * input2.x + w.x * input2.y;
			return input1;
		}

		public int PeformFFT_SinglePacked(int startIdx, IList<Vector4[]> data0, ICancelToken token)
		{
			int num = 0;
			int num2 = startIdx;
			int i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int index = (num2 + 1) % 2;
				Vector4[] array = data0[num];
				Vector4[] array2 = data0[index];
				int num3 = i * this.m_size;
				for (int j = 0; j < this.m_size; j++)
				{
					int num4 = j + num3;
					int num5 = this.m_butterflyLookupTable[num4].j1;
					int num6 = this.m_butterflyLookupTable[num4].j2;
					float wr = this.m_butterflyLookupTable[num4].wr;
					float wi = this.m_butterflyLookupTable[num4].wi;
					for (int k = 0; k < this.m_size; k++)
					{
						if (token.Cancelled)
						{
							return -1;
						}
						int num7 = k * this.m_size;
						int num8 = j + num7;
						int num9 = num5 + num7;
						int num10 = num6 + num7;
						array[num8].x = array2[num9].x + wr * array2[num10].x - wi * array2[num10].y;
						array[num8].y = array2[num9].y + wi * array2[num10].x + wr * array2[num10].y;
					}
				}
				i++;
				num2++;
			}
			i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int index = (num2 + 1) % 2;
				Vector4[] array3 = data0[num];
				Vector4[] array4 = data0[index];
				int num3 = i * this.m_size;
				for (int k = 0; k < this.m_size; k++)
				{
					int num4 = k + num3;
					int num5 = this.m_butterflyLookupTable[num4].j1 * this.m_size;
					int num6 = this.m_butterflyLookupTable[num4].j2 * this.m_size;
					float wr = this.m_butterflyLookupTable[num4].wr;
					float wi = this.m_butterflyLookupTable[num4].wi;
					for (int j = 0; j < this.m_size; j++)
					{
						if (token.Cancelled)
						{
							return -1;
						}
						int num8 = j + k * this.m_size;
						int num9 = j + num5;
						int num10 = j + num6;
						array3[num8].x = array4[num9].x + wr * array4[num10].x - wi * array4[num10].y;
						array3[num8].y = array4[num9].y + wi * array4[num10].x + wr * array4[num10].y;
					}
				}
				i++;
				num2++;
			}
			return num;
		}

		public int PeformFFT_DoublePacked(int startIdx, IList<Vector4[]> data0, ICancelToken token)
		{
			int num = 0;
			int num2 = startIdx;
			int i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int index = (num2 + 1) % 2;
				Vector4[] array = data0[num];
				Vector4[] array2 = data0[index];
				int num3 = i * this.m_size;
				for (int j = 0; j < this.m_size; j++)
				{
					int num4 = j + num3;
					int num5 = this.m_butterflyLookupTable[num4].j1;
					int num6 = this.m_butterflyLookupTable[num4].j2;
					float wr = this.m_butterflyLookupTable[num4].wr;
					float wi = this.m_butterflyLookupTable[num4].wi;
					for (int k = 0; k < this.m_size; k++)
					{
						if (token.Cancelled)
						{
							return -1;
						}
						int num7 = k * this.m_size;
						int num8 = j + num7;
						int num9 = num5 + num7;
						int num10 = num6 + num7;
						array[num8].x = array2[num9].x + wr * array2[num10].x - wi * array2[num10].y;
						array[num8].y = array2[num9].y + wi * array2[num10].x + wr * array2[num10].y;
						array[num8].z = array2[num9].z + wr * array2[num10].z - wi * array2[num10].w;
						array[num8].w = array2[num9].w + wi * array2[num10].z + wr * array2[num10].w;
					}
				}
				i++;
				num2++;
			}
			i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int index = (num2 + 1) % 2;
				Vector4[] array3 = data0[num];
				Vector4[] array4 = data0[index];
				int num3 = i * this.m_size;
				for (int k = 0; k < this.m_size; k++)
				{
					int num4 = k + num3;
					int num5 = this.m_butterflyLookupTable[num4].j1 * this.m_size;
					int num6 = this.m_butterflyLookupTable[num4].j2 * this.m_size;
					float wr = this.m_butterflyLookupTable[num4].wr;
					float wi = this.m_butterflyLookupTable[num4].wi;
					for (int j = 0; j < this.m_size; j++)
					{
						if (token.Cancelled)
						{
							return -1;
						}
						int num8 = j + k * this.m_size;
						int num9 = j + num5;
						int num10 = j + num6;
						array3[num8].x = array4[num9].x + wr * array4[num10].x - wi * array4[num10].y;
						array3[num8].y = array4[num9].y + wi * array4[num10].x + wr * array4[num10].y;
						array3[num8].z = array4[num9].z + wr * array4[num10].z - wi * array4[num10].w;
						array3[num8].w = array4[num9].w + wi * array4[num10].z + wr * array4[num10].w;
					}
				}
				i++;
				num2++;
			}
			return num;
		}
	}
}
