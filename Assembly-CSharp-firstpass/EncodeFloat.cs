using System;
using UnityEngine;

public static class EncodeFloat
{
	private static Material m_decodeToFloat2D;

	public static void WriteIntoRenderTexture2D(RenderTexture tex, int channels, float[] data)
	{
		if (tex == null)
		{
			Debug.Log("EncodeFloat::WriteIntoRenderTexture2D - RenderTexture is null");
			return;
		}
		if (data == null)
		{
			Debug.Log("EncodeFloat::WriteIntoRenderTexture2D - Data is null");
			return;
		}
		if (channels < 1 || channels > 4)
		{
			Debug.Log("EncodeFloat::WriteIntoRenderTexture2D - Channels must be 1, 2, 3, or 4");
			return;
		}
		int width = tex.width;
		int height = tex.height;
		int num = width * height * channels;
		Color[] map = new Color[num];
		float max = 1f;
		float min = 0f;
		EncodeFloat.LoadData(data, map, num, ref min, ref max);
		EncodeFloat.DecodeFloat2D(width, height, channels, min, max, tex, map);
	}

	private static void DecodeFloat2D(int w, int h, int c, float min, float max, RenderTexture tex, Color[] map)
	{
		Color[] array = new Color[w * h];
		Color[] array2 = new Color[w * h];
		Color[] array3 = new Color[w * h];
		Color[] array4 = new Color[w * h];
		for (int i = 0; i < w; i++)
		{
			for (int j = 0; j < h; j++)
			{
				array[i + j * w] = new Color(0f, 0f, 0f, 0f);
				array2[i + j * w] = new Color(0f, 0f, 0f, 0f);
				array3[i + j * w] = new Color(0f, 0f, 0f, 0f);
				array4[i + j * w] = new Color(0f, 0f, 0f, 0f);
				if (c > 0)
				{
					array[i + j * w] = map[(i + j * w) * c];
				}
				if (c > 1)
				{
					array2[i + j * w] = map[(i + j * w) * c + 1];
				}
				if (c > 2)
				{
					array3[i + j * w] = map[(i + j * w) * c + 2];
				}
				if (c > 3)
				{
					array4[i + j * w] = map[(i + j * w) * c + 3];
				}
			}
		}
		Texture2D texture2D = new Texture2D(w, h, TextureFormat.ARGB32, false, true);
		texture2D.hideFlags = HideFlags.HideAndDontSave;
		texture2D.filterMode = FilterMode.Point;
		texture2D.wrapMode = TextureWrapMode.Clamp;
		texture2D.SetPixels(array);
		texture2D.Apply();
		Texture2D texture2D2 = new Texture2D(w, h, TextureFormat.ARGB32, false, true);
		texture2D2.hideFlags = HideFlags.HideAndDontSave;
		texture2D2.filterMode = FilterMode.Point;
		texture2D2.wrapMode = TextureWrapMode.Clamp;
		texture2D2.SetPixels(array2);
		texture2D2.Apply();
		Texture2D texture2D3 = new Texture2D(w, h, TextureFormat.ARGB32, false, true);
		texture2D3.hideFlags = HideFlags.HideAndDontSave;
		texture2D3.filterMode = FilterMode.Point;
		texture2D3.wrapMode = TextureWrapMode.Clamp;
		texture2D3.SetPixels(array3);
		texture2D3.Apply();
		Texture2D texture2D4 = new Texture2D(w, h, TextureFormat.ARGB32, false, true);
		texture2D4.hideFlags = HideFlags.HideAndDontSave;
		texture2D4.filterMode = FilterMode.Point;
		texture2D4.wrapMode = TextureWrapMode.Clamp;
		texture2D4.SetPixels(array4);
		texture2D4.Apply();
		if (EncodeFloat.m_decodeToFloat2D == null)
		{
			Shader shader = Shader.Find("EncodeFloat/DecodeToFloat2D");
			if (shader == null)
			{
				Debug.Log("EncodeFloat::WriteIntoRenderTexture2D - could not find shader EncodeToFloat/DecodeToFloat2D");
				return;
			}
			EncodeFloat.m_decodeToFloat2D = new Material(shader);
			EncodeFloat.m_decodeToFloat2D.hideFlags = HideFlags.HideAndDontSave;
		}
		EncodeFloat.m_decodeToFloat2D.SetFloat("_Max", max);
		EncodeFloat.m_decodeToFloat2D.SetFloat("_Min", min);
		EncodeFloat.m_decodeToFloat2D.SetTexture("_TexR", texture2D);
		EncodeFloat.m_decodeToFloat2D.SetTexture("_TexG", texture2D2);
		EncodeFloat.m_decodeToFloat2D.SetTexture("_TexB", texture2D3);
		EncodeFloat.m_decodeToFloat2D.SetTexture("_TexA", texture2D4);
		Graphics.Blit(null, tex, EncodeFloat.m_decodeToFloat2D);
		UnityEngine.Object.Destroy(texture2D);
		UnityEngine.Object.Destroy(texture2D2);
		UnityEngine.Object.Destroy(texture2D3);
		UnityEngine.Object.Destroy(texture2D4);
	}

	private static void DecodeFloat3D(int w, int h, int d, int c, float min, float max, RenderTexture tex, Color[] map, ComputeShader shader)
	{
		Color[] array = new Color[w * h * d];
		Color[] array2 = new Color[w * h * d];
		Color[] array3 = new Color[w * h * d];
		Color[] array4 = new Color[w * h * d];
		for (int i = 0; i < w; i++)
		{
			for (int j = 0; j < h; j++)
			{
				for (int k = 0; k < d; k++)
				{
					array[i + j * w + k * w * h] = new Color(0f, 0f, 0f, 0f);
					array2[i + j * w + k * w * h] = new Color(0f, 0f, 0f, 0f);
					array3[i + j * w + k * w * h] = new Color(0f, 0f, 0f, 0f);
					array4[i + j * w + k * w * h] = new Color(0f, 0f, 0f, 0f);
					if (c > 0)
					{
						array[i + j * w + k * w * h] = map[(i + j * w + k * w * h) * c];
					}
					if (c > 1)
					{
						array2[i + j * w + k * w * h] = map[(i + j * w + k * w * h) * c + 1];
					}
					if (c > 2)
					{
						array3[i + j * w + k * w * h] = map[(i + j * w + k * w * h) * c + 2];
					}
					if (c > 3)
					{
						array4[i + j * w + k * w * h] = map[(i + j * w + k * w * h) * c + 3];
					}
				}
			}
		}
		Texture3D texture3D = new Texture3D(w, h, d, TextureFormat.ARGB32, false);
		texture3D.hideFlags = HideFlags.HideAndDontSave;
		texture3D.filterMode = FilterMode.Point;
		texture3D.wrapMode = TextureWrapMode.Clamp;
		texture3D.SetPixels(array);
		texture3D.Apply();
		Texture3D texture3D2 = new Texture3D(w, h, d, TextureFormat.ARGB32, false);
		texture3D2.hideFlags = HideFlags.HideAndDontSave;
		texture3D2.filterMode = FilterMode.Point;
		texture3D2.wrapMode = TextureWrapMode.Clamp;
		texture3D2.SetPixels(array2);
		texture3D2.Apply();
		Texture3D texture3D3 = new Texture3D(w, h, d, TextureFormat.ARGB32, false);
		texture3D3.hideFlags = HideFlags.HideAndDontSave;
		texture3D3.filterMode = FilterMode.Point;
		texture3D3.wrapMode = TextureWrapMode.Clamp;
		texture3D3.SetPixels(array3);
		texture3D3.Apply();
		Texture3D texture3D4 = new Texture3D(w, h, d, TextureFormat.ARGB32, false);
		texture3D4.hideFlags = HideFlags.HideAndDontSave;
		texture3D4.filterMode = FilterMode.Point;
		texture3D4.wrapMode = TextureWrapMode.Clamp;
		texture3D4.SetPixels(array4);
		texture3D4.Apply();
		shader.SetFloat("_Min", min);
		shader.SetFloat("_Max", max);
		shader.SetTexture(0, "_TexR", texture3D);
		shader.SetTexture(0, "_TexG", texture3D2);
		shader.SetTexture(0, "_TexB", texture3D3);
		shader.SetTexture(0, "_TexA", texture3D4);
		shader.SetTexture(0, "des", tex);
		shader.Dispatch(0, w, h, d);
		UnityEngine.Object.Destroy(texture3D);
		UnityEngine.Object.Destroy(texture3D2);
		UnityEngine.Object.Destroy(texture3D3);
		UnityEngine.Object.Destroy(texture3D4);
	}

	private static float[] EncodeFloatRGBA(float val)
	{
		float[] array = new float[]
		{
			1f,
			255f,
			65025f,
			160581376f
		};
		float num = 0.003921569f;
		for (int i = 0; i < array.Length; i++)
		{
			array[i] *= val;
			array[i] = (float)((double)array[i] - Math.Truncate((double)array[i]));
		}
		float[] array2 = new float[]
		{
			array[1],
			array[2],
			array[3],
			array[3]
		};
		for (int j = 0; j < array.Length; j++)
		{
			array[j] -= array2[j] * num;
		}
		return array;
	}

	private static void LoadData(float[] data, Color[] map, int size, ref float min, ref float max)
	{
		for (int i = 0; i < size; i++)
		{
			if (data[i] > max)
			{
				max = data[i];
			}
			if (data[i] < min)
			{
				min = data[i];
			}
		}
		min = Mathf.Abs(min);
		max += min;
		for (int j = 0; j < size; j++)
		{
			float val = (data[j] + min) / max;
			float[] array = EncodeFloat.EncodeFloatRGBA(val);
			map[j] = new Color(array[0], array[1], array[2], array[3]);
		}
	}
}
