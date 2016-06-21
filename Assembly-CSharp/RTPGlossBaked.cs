using System;
using UnityEngine;

[Serializable]
public class RTPGlossBaked : ScriptableObject
{
	public byte[] glossDataMIP1;

	public byte[] glossDataMIP2;

	public byte[] glossDataMIP3;

	public byte[] glossDataMIP4;

	public byte[] glossDataMIP5;

	public byte[] glossDataMIP6;

	public byte[] glossDataMIP7;

	public byte[] glossDataMIP8;

	public byte[] glossDataMIP9;

	public byte[] glossDataMIP10;

	public byte[] glossDataMIP11;

	public byte[] glossDataMIP12;

	public int size;

	public bool baked;

	public float gloss_mult;

	public float gloss_shaping;

	public bool used_in_atlas;

	public RTPGlossBaked()
	{
		this.Init(0);
	}

	public RTPGlossBaked(int size)
	{
		this.Init(size);
	}

	public void Init(int size)
	{
		this.size = size;
		this.baked = false;
		this.used_in_atlas = false;
		this.glossDataMIP1 = (this.glossDataMIP2 = (this.glossDataMIP3 = (this.glossDataMIP4 = (this.glossDataMIP5 = (this.glossDataMIP6 = null)))));
		this.glossDataMIP7 = (this.glossDataMIP8 = (this.glossDataMIP9 = (this.glossDataMIP10 = (this.glossDataMIP11 = (this.glossDataMIP12 = null)))));
		if (size <= 1)
		{
			return;
		}
		size >>= 1;
		this.glossDataMIP1 = new byte[size * size];
		if (size <= 1)
		{
			return;
		}
		size >>= 1;
		this.glossDataMIP2 = new byte[size * size];
		if (size <= 1)
		{
			return;
		}
		size >>= 1;
		this.glossDataMIP3 = new byte[size * size];
		if (size <= 1)
		{
			return;
		}
		size >>= 1;
		this.glossDataMIP4 = new byte[size * size];
		if (size <= 1)
		{
			return;
		}
		size >>= 1;
		this.glossDataMIP5 = new byte[size * size];
		if (size <= 1)
		{
			return;
		}
		size >>= 1;
		this.glossDataMIP6 = new byte[size * size];
		if (size <= 1)
		{
			return;
		}
		size >>= 1;
		this.glossDataMIP7 = new byte[size * size];
		if (size <= 1)
		{
			return;
		}
		size >>= 1;
		this.glossDataMIP8 = new byte[size * size];
		if (size <= 1)
		{
			return;
		}
		size >>= 1;
		this.glossDataMIP9 = new byte[size * size];
		if (size <= 1)
		{
			return;
		}
		size >>= 1;
		this.glossDataMIP10 = new byte[size * size];
		if (size <= 1)
		{
			return;
		}
		size >>= 1;
		this.glossDataMIP11 = new byte[size * size];
		if (size <= 1)
		{
			return;
		}
		size >>= 1;
		this.glossDataMIP12 = new byte[size * size];
		if (size > 1)
		{
			size >>= 1;
			return;
		}
	}

	public byte[] GetGlossMipLevel(int mipLevel)
	{
		switch (mipLevel)
		{
		case 1:
			return this.glossDataMIP1;
		case 2:
			return this.glossDataMIP2;
		case 3:
			return this.glossDataMIP3;
		case 4:
			return this.glossDataMIP4;
		case 5:
			return this.glossDataMIP5;
		case 6:
			return this.glossDataMIP6;
		case 7:
			return this.glossDataMIP7;
		case 8:
			return this.glossDataMIP8;
		case 9:
			return this.glossDataMIP9;
		case 10:
			return this.glossDataMIP10;
		case 11:
			return this.glossDataMIP11;
		case 12:
			return this.glossDataMIP12;
		default:
			return null;
		}
	}

	public bool CheckSize(Texture2D tex)
	{
		return this.baked && !(tex == null) && this.glossDataMIP1 != null && tex.width == this.size;
	}

	public static Texture2D MakeTexture(Texture2D sourceTexture, RTPGlossBaked[] tilesData)
	{
		Texture2D texture2D = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.ARGB32, true);
		for (int i = 0; i < 4; i++)
		{
			int num = sourceTexture.width >> 1;
			int num2 = i % 2 * num;
			int num3 = (i % 4 >= 2) ? num : 0;
			Color[] array = sourceTexture.GetPixels(num2, num3, num, num, 0);
			for (int j = 0; j < array.Length; j++)
			{
				float num4 = Mathf.Clamp01(array[j].a * tilesData[i].gloss_mult);
				float num5 = num4;
				float num6 = 1f - num4;
				num5 = num5 * num5 * num5;
				num6 = num6 * num6 * num6;
				float num7 = Mathf.Lerp(num5, 1f - num6, tilesData[i].gloss_shaping);
				num7 = Mathf.Clamp01(num7);
				array[j].a = num7;
			}
			texture2D.SetPixels(num2, num3, num, num, array, 0);
			for (int k = 1; k < sourceTexture.mipmapCount - 1; k++)
			{
				int num8 = texture2D.width >> k + 1;
				Color[] array2 = new Color[num8 * num8];
				byte[] glossMipLevel = tilesData[i].GetGlossMipLevel(k);
				int num9 = glossMipLevel.Length;
				for (int l = 0; l < num9; l++)
				{
					int num10 = l % num8;
					int num11 = l / num8;
					int num12 = num11 * num8 * 4 + num10 * 2;
					int num13 = num12 + 1;
					int num14 = num12 + num8 * 2;
					int num15 = num14 + 1;
					array2[l].r = (array[num12].r + array[num13].r + array[num14].r + array[num15].r) / 4f;
					array2[l].g = (array[num12].g + array[num13].g + array[num14].g + array[num15].g) / 4f;
					array2[l].b = (array[num12].b + array[num13].b + array[num14].b + array[num15].b) / 4f;
					array2[l].a = (float)glossMipLevel[l] / 255f;
				}
				array = array2;
				num2 >>= 1;
				num3 >>= 1;
				num >>= 1;
				texture2D.SetPixels(num2, num3, num, num, array2, k);
			}
		}
		texture2D.Apply(false, false);
		texture2D.Compress(true);
		texture2D.Apply(false, true);
		texture2D.wrapMode = sourceTexture.wrapMode;
		texture2D.anisoLevel = sourceTexture.anisoLevel;
		texture2D.filterMode = sourceTexture.filterMode;
		return texture2D;
	}

	public Texture2D MakeTexture(Texture2D sourceTexture)
	{
		Texture2D texture2D = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.ARGB32, true);
		Color32[] array = sourceTexture.GetPixels32(0);
		for (int i = 0; i < array.Length; i++)
		{
			float num = Mathf.Clamp01((float)array[i].a / 255f * this.gloss_mult);
			float num2 = num;
			float num3 = 1f - num;
			num2 = num2 * num2 * num2;
			num3 = num3 * num3 * num3;
			float num4 = Mathf.Lerp(num2, 1f - num3, this.gloss_shaping);
			num4 = Mathf.Clamp01(num4);
			array[i].a = (byte)(num4 * 255f);
		}
		texture2D.SetPixels32(array, 0);
		for (int j = 1; j < sourceTexture.mipmapCount; j++)
		{
			int num5 = texture2D.width >> j;
			Color32[] array2 = new Color32[num5 * num5];
			byte[] glossMipLevel = this.GetGlossMipLevel(j);
			int num6 = glossMipLevel.Length;
			for (int k = 0; k < num6; k++)
			{
				int num7 = k % num5;
				int num8 = k / num5;
				int num9 = num8 * num5 * 4 + num7 * 2;
				int num10 = num9 + 1;
				int num11 = num9 + num5 * 2;
				int num12 = num11 + 1;
				array2[k].r = (byte)(array[num9].r + array[num10].r + array[num11].r + array[num12].r >> 2);
				array2[k].g = (byte)(array[num9].g + array[num10].g + array[num11].g + array[num12].g >> 2);
				array2[k].b = (byte)(array[num9].b + array[num10].b + array[num11].b + array[num12].b >> 2);
				array2[k].a = glossMipLevel[k];
			}
			array = array2;
			texture2D.SetPixels32(array2, j);
		}
		texture2D.Apply(false, false);
		texture2D.Compress(true);
		texture2D.Apply(false, true);
		texture2D.wrapMode = sourceTexture.wrapMode;
		texture2D.anisoLevel = sourceTexture.anisoLevel;
		texture2D.filterMode = sourceTexture.filterMode;
		return texture2D;
	}

	public void GetMIPGlossMapsFromAtlas(Texture2D atlasTex, int tile)
	{
		if (atlasTex == null)
		{
			return;
		}
		int num = atlasTex.width >> 1;
		this.Init(num);
		this.gloss_mult = 1f;
		this.gloss_shaping = 0.5f;
		int num2 = tile % 2 * num;
		int num3 = (tile % 4 >= 2) ? num : 0;
		Color[] pixels = atlasTex.GetPixels(num2, num3, num, num, 0);
		byte[] array = new byte[num * num];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (byte)(pixels[i].a * 255f);
		}
		for (int j = 1; j < atlasTex.mipmapCount - 1; j++)
		{
			int num4 = atlasTex.width >> j + 1;
			byte[] glossMipLevel = this.GetGlossMipLevel(j);
			int num5 = glossMipLevel.Length;
			for (int k = 0; k < num5; k++)
			{
				int num6 = k % num4;
				int num7 = k / num4;
				int num8 = num7 * num4 * 4 + num6 * 2;
				int num9 = num8 + 1;
				int num10 = num8 + num4 * 2;
				int num11 = num10 + 1;
				glossMipLevel[k] = (byte)(array[num8] + array[num9] + array[num10] + array[num11] >> 2);
			}
			array = glossMipLevel;
			num2 >>= 1;
			num3 >>= 1;
			num >>= 1;
		}
		this.baked = true;
	}

	public void PrepareMIPGlossMap(Texture2D DiffuseSpecTexture, Texture2D NormalMap, float gloss_mult, float gloss_shaping, int layerNumForAtlas = -1)
	{
		if (DiffuseSpecTexture == null)
		{
			return;
		}
		this.Init(DiffuseSpecTexture.width);
		this.gloss_mult = gloss_mult;
		this.gloss_shaping = gloss_shaping;
		Color32[] pixels = DiffuseSpecTexture.GetPixels32(0);
		for (int i = 0; i < pixels.Length; i++)
		{
			float num = Mathf.Clamp01((float)pixels[i].a / 255f * gloss_mult);
			float num2 = num;
			float num3 = 1f - num;
			num2 = num2 * num2 * num2;
			num3 = num3 * num3 * num3;
			float num4 = Mathf.Lerp(num2, 1f - num3, gloss_shaping);
			num4 = Mathf.Clamp01(num4);
			pixels[i].a = (byte)(num4 * 255f);
		}
		Color32[] normalMap = new Color32[1];
		int nSize = 0;
		if (NormalMap != null)
		{
			if (layerNumForAtlas == -1)
			{
				Debug.Log(string.Concat(new string[]
				{
					"Baking MIP gloss data for \"",
					DiffuseSpecTexture.name,
					"\" with normal variance from \"",
					NormalMap.name,
					"\""
				}));
			}
			else
			{
				Debug.Log(string.Concat(new object[]
				{
					"Baking MIP gloss data (atlased) for layer ",
					layerNumForAtlas,
					" with normal variance from \"",
					NormalMap.name,
					"\""
				}));
			}
			nSize = NormalMap.width;
			normalMap = NormalMap.GetPixels32(0);
		}
		int j = DiffuseSpecTexture.width >> 1;
		int num5 = 1;
		while (j > 0)
		{
			int num6 = 0;
			byte[] glossMipLevel = this.GetGlossMipLevel(num5);
			for (int k = j - 1; k >= 0; k--)
			{
				for (int l = 0; l < j; l++)
				{
					float num7 = this.MedianGlossiness(l, k, num5, pixels);
					if (NormalMap != null)
					{
						glossMipLevel[num6] = (byte)(this.BakeGlossinessVsVariance((float)l * 1f / (float)j, (float)k * 1f / (float)j, num5, num7, normalMap, nSize) * 255f);
					}
					else
					{
						glossMipLevel[num6] = (byte)(num7 * 255f);
					}
					num6++;
				}
			}
			j >>= 1;
			num5++;
		}
		this.baked = true;
	}

	private float MedianGlossiness(int texelPosX, int texelPosY, int mipLevel, Color32[] cols)
	{
		int num = 1 << mipLevel;
		texelPosX *= num;
		texelPosY *= num;
		int num2 = Mathf.FloorToInt(Mathf.Sqrt(1f * (float)cols.Length));
		texelPosY = num2 - texelPosY - num;
		if (mipLevel == 0)
		{
			return (float)cols[texelPosY * num2 + texelPosX].a / 255f;
		}
		int num3 = texelPosX;
		int num4 = texelPosY;
		float num5 = 0f;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int num6 = num3 + j;
				int num7 = num4 + i;
				float num8 = (float)cols[num7 * num2 + num6].a / 255f;
				num5 += Mathf.Pow(2f, num8 * 10f + 1f);
			}
		}
		num5 -= 1.75f * (float)(num * num);
		num5 /= (float)(num * num);
		num5 += 1.75f;
		return (Mathf.Log(num5) - Mathf.Log(2f)) / (10f * Mathf.Log(2f));
	}

	private float BakeGlossinessVsVariance(float texelPosX, float texelPosY, int mipLevel, float glossiness, Color32[] NormalMap, int nSize)
	{
		if (mipLevel == 0)
		{
			return glossiness;
		}
		int num = 1 << mipLevel;
		int num2 = Mathf.FloorToInt(texelPosX * (float)nSize);
		int num3 = Mathf.FloorToInt((1f - texelPosY) * (float)nSize - (float)num);
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int num4 = num2 + j;
				int num5 = num3 + i;
				Color32 color = NormalMap[(nSize - num5 - 1) * nSize + num4];
				Vector3 b = new Vector3((float)color.a / 255f * 2f - 1f, (float)color.g / 255f * 2f - 1f, 0f);
				b.z = 1f - b.x * b.x - b.y * b.y;
				if (b.z < 0f)
				{
					b.z = 0f;
				}
				else
				{
					b.z = Mathf.Sqrt(b.z);
				}
				b.Normalize();
				vector += b;
			}
		}
		vector /= (float)(num * num);
		float num6 = 0f;
		for (int k = 0; k < num; k++)
		{
			for (int l = 0; l < num; l++)
			{
				int num7 = num2 + l;
				int num8 = num3 + k;
				Color32 color2 = NormalMap[(nSize - num8 - 1) * nSize + num7];
				Vector3 rhs = new Vector3((float)color2.a / 255f * 2f - 1f, (float)color2.g / 255f * 2f - 1f, 0f);
				rhs.z = 1f - rhs.x * rhs.x - rhs.y * rhs.y;
				if (rhs.z < 0f)
				{
					rhs.z = 0f;
				}
				else
				{
					rhs.z = Mathf.Sqrt(rhs.z);
				}
				rhs.Normalize();
				float num9 = Vector3.Dot(vector, rhs);
				num6 += 1f - num9 * num9;
			}
		}
		num6 /= (float)(num * num);
		float num10 = Mathf.Pow(2f, glossiness * 10f + 1f) - 1.75f;
		float num11 = Mathf.Pow(2f, 11f) - 1.75f;
		float value = num6 + 1f / (1f + num10);
		float num12 = Mathf.Clamp(value, 1f / (1f + num11), 0.5f);
		return Mathf.Log(1f / num12 - 1f) / Mathf.Log(num11);
	}
}
