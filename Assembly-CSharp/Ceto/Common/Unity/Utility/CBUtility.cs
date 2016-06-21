using System;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	public static class CBUtility
	{
		private static string[,] readNames2D;

		static CBUtility()
		{
			// Note: this type is marked as 'beforefieldinit'.
			string[,] expr_07 = new string[4, 3];
			expr_07[0, 0] = "read2DC1";
			expr_07[0, 1] = "_Tex2D";
			expr_07[0, 2] = "_Buffer2DC1";
			expr_07[1, 0] = "read2DC2";
			expr_07[1, 1] = "_Tex2D";
			expr_07[1, 2] = "_Buffer2DC2";
			expr_07[2, 0] = "read2DC3";
			expr_07[2, 1] = "_Tex2D";
			expr_07[2, 2] = "_Buffer2DC3";
			expr_07[3, 0] = "read2DC4";
			expr_07[3, 1] = "_Tex2D";
			expr_07[3, 2] = "_Buffer2DC4";
			CBUtility.readNames2D = expr_07;
		}

		public static void ReadFromRenderTexture(RenderTexture tex, int channels, ComputeBuffer buffer, ComputeShader readData)
		{
			if (tex == null)
			{
				Debug.Log("CBUtility::ReadFromRenderTexture - RenderTexture is null");
				return;
			}
			if (buffer == null)
			{
				Debug.Log("CBUtility::ReadFromRenderTexture - buffer is null");
				return;
			}
			if (readData == null)
			{
				Debug.Log("CBUtility::ReadFromRenderTexture - Computer shader is null");
				return;
			}
			if (channels < 1 || channels > 4)
			{
				Debug.Log("CBUtility::ReadFromRenderTexture - Channels must be 1, 2, 3, or 4");
				return;
			}
			if (!tex.IsCreated())
			{
				Debug.Log("CBUtility::ReadFromRenderTexture - tex has not been created (Call Create() on tex)");
				return;
			}
			int num = 1;
			int num2 = readData.FindKernel(CBUtility.readNames2D[channels - 1, 0]);
			readData.SetTexture(num2, CBUtility.readNames2D[channels - 1, 1], tex);
			readData.SetBuffer(num2, CBUtility.readNames2D[channels - 1, 2], buffer);
			if (num2 == -1)
			{
				Debug.Log("CBUtility::ReadFromRenderTexture - could not find kernels");
				return;
			}
			int width = tex.width;
			int height = tex.height;
			readData.SetInt("_Width", width);
			readData.SetInt("_Height", height);
			readData.SetInt("_Depth", num);
			int num3 = (width % 8 != 0) ? 1 : 0;
			int num4 = (height % 8 != 0) ? 1 : 0;
			int num5 = (num % 8 != 0) ? 1 : 0;
			readData.Dispatch(num2, Mathf.Max(1, width / 8 + num3), Mathf.Max(1, height / 8 + num4), Mathf.Max(1, num / 8 + num5));
		}

		public static void ReadSingleFromRenderTexture(RenderTexture tex, float x, float y, float z, ComputeBuffer buffer, ComputeShader readData, bool useBilinear)
		{
			if (tex == null)
			{
				Debug.Log("CBUtility::ReadSingleFromRenderTexture - RenderTexture is null");
				return;
			}
			if (buffer == null)
			{
				Debug.Log("CBUtility::ReadSingleFromRenderTexture - buffer is null");
				return;
			}
			if (readData == null)
			{
				Debug.Log("CBUtility::ReadSingleFromRenderTexture - Computer shader is null");
				return;
			}
			if (!tex.IsCreated())
			{
				Debug.Log("CBUtility::ReadSingleFromRenderTexture - tex has not been created (Call Create() on tex)");
				return;
			}
			int num = 1;
			int num2;
			if (useBilinear)
			{
				num2 = readData.FindKernel("readSingle2D");
			}
			else
			{
				num2 = readData.FindKernel("readSingleBilinear2D");
			}
			readData.SetTexture(num2, "_Tex2D", tex);
			readData.SetBuffer(num2, "BufferSingle2D", buffer);
			if (num2 == -1)
			{
				Debug.Log("CBUtility::ReadSingleFromRenderTexture - could not find kernels");
				return;
			}
			int width = tex.width;
			int height = tex.height;
			readData.SetInt("_IdxX", (int)x);
			readData.SetInt("_IdxY", (int)y);
			readData.SetInt("_IdxZ", (int)z);
			readData.SetVector("_UV", new Vector4(x / (float)(width - 1), y / (float)(height - 1), z / (float)(num - 1), 0f));
			readData.Dispatch(num2, 1, 1, 1);
		}
	}
}
