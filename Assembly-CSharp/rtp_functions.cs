using System;
using UnityEngine;

public class rtp_functions
{
	public static Texture2D PrepareHeights(int num, int numLayers, Texture2D[] Heights)
	{
		if (Heights == null)
		{
			return null;
		}
		for (int i = 0; i < Heights.Length; i++)
		{
			int num2 = 9999;
			int num3 = i / 4 * 4;
			while (num3 < i / 4 * 4 + 4 && num3 < Heights.Length)
			{
				if (Heights[num3] && Heights[num3].width < num2)
				{
					num2 = Heights[num3].width;
				}
				num3++;
			}
		}
		Texture2D[] array = new Texture2D[4];
		int num4 = 256;
		int num5 = (numLayers >= 12) ? 12 : numLayers;
		if (num >= num5)
		{
			return null;
		}
		num = (num >> 2) * 4;
		for (int j = num; j < num + 4; j++)
		{
			if (num < 4)
			{
				array[j] = ((j >= Heights.Length) ? null : Heights[j]);
				if (array[j])
				{
					num4 = array[j].width;
				}
			}
			else if (num < 8)
			{
				array[j - 4] = ((j >= Heights.Length) ? null : Heights[j]);
				if (array[j - 4])
				{
					num4 = array[j - 4].width;
				}
			}
			else
			{
				array[j - 8] = ((j >= Heights.Length) ? null : Heights[j]);
				if (array[j - 8])
				{
					num4 = array[j - 8].width;
				}
			}
		}
		for (int j = 0; j < 4; j++)
		{
			if (!array[j])
			{
				array[j] = new Texture2D(num4, num4);
				rtp_functions.FillTex(array[j], new Color32(255, 255, 255, 255));
			}
		}
		return rtp_functions.CombineHeights(array[0], array[1], array[2], array[3]);
	}

	public static Texture2D CombineHeights(Texture2D source_tex0, Texture2D source_tex1, Texture2D source_tex2, Texture2D source_tex3)
	{
		Texture2D texture2D = new Texture2D(source_tex0.width, source_tex0.height, TextureFormat.ARGB32, true, true);
		byte[] array = rtp_functions.get_alpha_channel(source_tex0);
		byte[] array2 = rtp_functions.get_alpha_channel(source_tex1);
		byte[] array3 = rtp_functions.get_alpha_channel(source_tex2);
		byte[] array4 = rtp_functions.get_alpha_channel(source_tex3);
		Color32[] pixels = texture2D.GetPixels32();
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i].r = array[i];
			pixels[i].g = array2[i];
			pixels[i].b = array3[i];
			pixels[i].a = array4[i];
		}
		texture2D.SetPixels32(pixels);
		texture2D.Apply(true, false);
		texture2D.Compress(true);
		texture2D.filterMode = FilterMode.Trilinear;
		return texture2D;
	}

	private static byte[] get_alpha_channel(Texture2D source_tex)
	{
		Color32[] pixels = source_tex.GetPixels32();
		byte[] array = new byte[pixels.Length];
		for (int i = 0; i < pixels.Length; i++)
		{
			array[i] = pixels[i].a;
		}
		return array;
	}

	private static void FillTex(Texture2D tex, Color32 col)
	{
		Color32[] pixels = tex.GetPixels32();
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i].r = col.r;
			pixels[i].g = col.g;
			pixels[i].b = col.b;
			pixels[i].a = col.a;
		}
		tex.SetPixels32(pixels);
	}
}
