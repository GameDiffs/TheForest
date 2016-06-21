using Ceto.Common.Unity.Utility;
using System;
using UnityEngine;

namespace Ceto
{
	public class FourierGPU
	{
		private const int PASS_X_1 = 0;

		private const int PASS_Y_1 = 1;

		private const int PASS_X_2 = 2;

		private const int PASS_Y_2 = 3;

		private const int PASS_X_3 = 4;

		private const int PASS_Y_3 = 5;

		private const int PASS_X_4 = 6;

		private const int PASS_Y_4 = 7;

		private int m_size;

		private float m_fsize;

		private int m_passes;

		private Texture2D[] m_butterflyLookupTable;

		private Material m_fourier;

		private RenderBuffer[] m_pass0RT2;

		private RenderBuffer[] m_pass1RT2;

		private RenderBuffer[] m_pass0RT3;

		private RenderBuffer[] m_pass1RT3;

		private RenderBuffer[] m_pass0RT4;

		private RenderBuffer[] m_pass1RT4;

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

		public FourierGPU(int size, Shader sdr)
		{
			if (!Mathf.IsPowerOfTwo(size))
			{
				throw new ArgumentException("Fourier grid size must be pow2 number");
			}
			this.m_fourier = new Material(sdr);
			this.m_size = size;
			this.m_fsize = (float)this.m_size;
			this.m_passes = (int)(Mathf.Log(this.m_fsize) / Mathf.Log(2f));
			this.m_butterflyLookupTable = new Texture2D[this.m_passes];
			this.ComputeButterflyLookupTable();
			this.m_fourier.SetFloat("Ceto_FourierSize", this.m_fsize);
			this.m_pass0RT2 = new RenderBuffer[2];
			this.m_pass1RT2 = new RenderBuffer[2];
			this.m_pass0RT3 = new RenderBuffer[3];
			this.m_pass1RT3 = new RenderBuffer[3];
			this.m_pass0RT4 = new RenderBuffer[4];
			this.m_pass1RT4 = new RenderBuffer[4];
		}

		public void Release()
		{
			int num = this.m_butterflyLookupTable.Length;
			for (int i = 0; i < num; i++)
			{
				UnityEngine.Object.Destroy(this.m_butterflyLookupTable[i]);
			}
			this.m_butterflyLookupTable = null;
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

		private Texture2D Make1DTex(int i)
		{
			return new Texture2D(this.m_size, 1, TextureFormat.RGBAFloat, false, true)
			{
				filterMode = FilterMode.Point,
				wrapMode = TextureWrapMode.Clamp,
				hideFlags = HideFlags.HideAndDontSave,
				name = "Ceto Fouier GPU Butterfly Lookup"
			};
		}

		private void ComputeButterflyLookupTable()
		{
			float num = (float)this.m_size;
			float num2 = (float)(this.m_size - 1);
			for (int i = 0; i < this.m_passes; i++)
			{
				int num3 = (int)Mathf.Pow(2f, (float)(this.m_passes - 1 - i));
				int num4 = (int)Mathf.Pow(2f, (float)i);
				this.m_butterflyLookupTable[i] = this.Make1DTex(i);
				for (int j = 0; j < num3; j++)
				{
					for (int k = 0; k < num4; k++)
					{
						int num5;
						int num6;
						int num7;
						int num8;
						if (i == 0)
						{
							num5 = j * num4 * 2 + k;
							num6 = j * num4 * 2 + num4 + k;
							num7 = this.BitReverse(num5);
							num8 = this.BitReverse(num6);
						}
						else
						{
							num5 = j * num4 * 2 + k;
							num6 = j * num4 * 2 + num4 + k;
							num7 = num5;
							num8 = num6;
						}
						float num9 = Mathf.Cos(6.28318548f * (float)(k * num3) / num);
						float num10 = Mathf.Sin(6.28318548f * (float)(k * num3) / num);
						this.m_butterflyLookupTable[i].SetPixel(num5, 0, new Color((float)num7 / num2, (float)num8 / num2, num9, num10));
						this.m_butterflyLookupTable[i].SetPixel(num6, 0, new Color((float)num7 / num2, (float)num8 / num2, -num9, -num10));
					}
				}
				this.m_butterflyLookupTable[i].Apply();
			}
		}

		public int PeformFFT(RenderTexture[] data0)
		{
			if (this.m_butterflyLookupTable == null)
			{
				return -1;
			}
			RenderTexture dest = data0[0];
			RenderTexture dest2 = data0[1];
			int num = 0;
			int num2 = 0;
			int i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int num3 = (num2 + 1) % 2;
				this.m_fourier.SetTexture("Ceto_ButterFlyLookUp", this.m_butterflyLookupTable[i]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer0", data0[num3]);
				if (num == 0)
				{
					Graphics.Blit(null, dest, this.m_fourier, 0);
				}
				else
				{
					Graphics.Blit(null, dest2, this.m_fourier, 0);
				}
				i++;
				num2++;
			}
			i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int num3 = (num2 + 1) % 2;
				this.m_fourier.SetTexture("Ceto_ButterFlyLookUp", this.m_butterflyLookupTable[i]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer0", data0[num3]);
				if (num == 0)
				{
					Graphics.Blit(null, dest, this.m_fourier, 1);
				}
				else
				{
					Graphics.Blit(null, dest2, this.m_fourier, 1);
				}
				i++;
				num2++;
			}
			return num;
		}

		public int PeformFFT(RenderTexture[] data0, RenderTexture[] data1)
		{
			if (this.m_butterflyLookupTable == null)
			{
				return -1;
			}
			if (SystemInfo.supportedRenderTargetCount < 2)
			{
				throw new InvalidOperationException("System does not support at least 2 render targets");
			}
			this.m_pass0RT2[0] = data0[0].colorBuffer;
			this.m_pass0RT2[1] = data1[0].colorBuffer;
			this.m_pass1RT2[0] = data0[1].colorBuffer;
			this.m_pass1RT2[1] = data1[1].colorBuffer;
			RenderBuffer depthBuffer = data0[0].depthBuffer;
			RenderBuffer depthBuffer2 = data0[1].depthBuffer;
			int num = 0;
			int num2 = 0;
			int i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int num3 = (num2 + 1) % 2;
				this.m_fourier.SetTexture("Ceto_ButterFlyLookUp", this.m_butterflyLookupTable[i]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer0", data0[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer1", data1[num3]);
				if (num == 0)
				{
					RTUtility.MultiTargetBlit(this.m_pass0RT2, depthBuffer, this.m_fourier, 2);
				}
				else
				{
					RTUtility.MultiTargetBlit(this.m_pass1RT2, depthBuffer2, this.m_fourier, 2);
				}
				i++;
				num2++;
			}
			i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int num3 = (num2 + 1) % 2;
				this.m_fourier.SetTexture("Ceto_ButterFlyLookUp", this.m_butterflyLookupTable[i]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer0", data0[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer1", data1[num3]);
				if (num == 0)
				{
					RTUtility.MultiTargetBlit(this.m_pass0RT2, depthBuffer, this.m_fourier, 3);
				}
				else
				{
					RTUtility.MultiTargetBlit(this.m_pass1RT2, depthBuffer2, this.m_fourier, 3);
				}
				i++;
				num2++;
			}
			return num;
		}

		public int PeformFFT(RenderTexture[] data0, RenderTexture[] data1, RenderTexture[] data2)
		{
			if (this.m_butterflyLookupTable == null)
			{
				return -1;
			}
			if (SystemInfo.supportedRenderTargetCount < 3)
			{
				throw new InvalidOperationException("System does not support at least 3 render targets");
			}
			this.m_pass0RT3[0] = data0[0].colorBuffer;
			this.m_pass0RT3[1] = data1[0].colorBuffer;
			this.m_pass0RT3[2] = data2[0].colorBuffer;
			this.m_pass1RT3[0] = data0[1].colorBuffer;
			this.m_pass1RT3[1] = data1[1].colorBuffer;
			this.m_pass1RT3[2] = data2[1].colorBuffer;
			RenderBuffer depthBuffer = data0[0].depthBuffer;
			RenderBuffer depthBuffer2 = data0[1].depthBuffer;
			int num = 0;
			int num2 = 0;
			int i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int num3 = (num2 + 1) % 2;
				this.m_fourier.SetTexture("Ceto_ButterFlyLookUp", this.m_butterflyLookupTable[i]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer0", data0[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer1", data1[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer2", data2[num3]);
				if (num == 0)
				{
					RTUtility.MultiTargetBlit(this.m_pass0RT3, depthBuffer, this.m_fourier, 4);
				}
				else
				{
					RTUtility.MultiTargetBlit(this.m_pass1RT3, depthBuffer2, this.m_fourier, 4);
				}
				i++;
				num2++;
			}
			i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int num3 = (num2 + 1) % 2;
				this.m_fourier.SetTexture("Ceto_ButterFlyLookUp", this.m_butterflyLookupTable[i]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer0", data0[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer1", data1[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer2", data2[num3]);
				if (num == 0)
				{
					RTUtility.MultiTargetBlit(this.m_pass0RT3, depthBuffer, this.m_fourier, 5);
				}
				else
				{
					RTUtility.MultiTargetBlit(this.m_pass1RT3, depthBuffer2, this.m_fourier, 5);
				}
				i++;
				num2++;
			}
			return num;
		}

		public int PeformFFT(RenderTexture[] data0, RenderTexture[] data1, RenderTexture[] data2, RenderTexture[] data3)
		{
			if (this.m_butterflyLookupTable == null)
			{
				return -1;
			}
			if (SystemInfo.supportedRenderTargetCount < 4)
			{
				throw new InvalidOperationException("System does not support at least 4 render targets");
			}
			this.m_pass0RT4[0] = data0[0].colorBuffer;
			this.m_pass0RT4[1] = data1[0].colorBuffer;
			this.m_pass0RT4[2] = data2[0].colorBuffer;
			this.m_pass0RT4[3] = data3[0].colorBuffer;
			this.m_pass1RT4[0] = data0[1].colorBuffer;
			this.m_pass1RT4[1] = data1[1].colorBuffer;
			this.m_pass1RT4[2] = data2[1].colorBuffer;
			this.m_pass1RT4[3] = data3[1].colorBuffer;
			RenderBuffer depthBuffer = data0[0].depthBuffer;
			RenderBuffer depthBuffer2 = data0[1].depthBuffer;
			int num = 0;
			int num2 = 0;
			int i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int num3 = (num2 + 1) % 2;
				this.m_fourier.SetTexture("Ceto_ButterFlyLookUp", this.m_butterflyLookupTable[i]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer0", data0[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer1", data1[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer2", data2[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer3", data3[num3]);
				if (num == 0)
				{
					RTUtility.MultiTargetBlit(this.m_pass0RT4, depthBuffer, this.m_fourier, 6);
				}
				else
				{
					RTUtility.MultiTargetBlit(this.m_pass1RT4, depthBuffer2, this.m_fourier, 6);
				}
				i++;
				num2++;
			}
			i = 0;
			while (i < this.m_passes)
			{
				num = num2 % 2;
				int num3 = (num2 + 1) % 2;
				this.m_fourier.SetTexture("Ceto_ButterFlyLookUp", this.m_butterflyLookupTable[i]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer0", data0[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer1", data1[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer2", data2[num3]);
				this.m_fourier.SetTexture("Ceto_ReadBuffer3", data3[num3]);
				if (num == 0)
				{
					RTUtility.MultiTargetBlit(this.m_pass0RT4, depthBuffer, this.m_fourier, 7);
				}
				else
				{
					RTUtility.MultiTargetBlit(this.m_pass1RT4, depthBuffer2, this.m_fourier, 7);
				}
				i++;
				num2++;
			}
			return num;
		}
	}
}
