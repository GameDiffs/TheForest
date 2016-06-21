using Ceto.Common.Containers.Interpolation;
using System;
using UnityEngine;

namespace Ceto
{
	public static class ShoreMaskGenerator
	{
		public static float[] CreateHeightMap(Terrain terrain)
		{
			TerrainData terrainData = terrain.terrainData;
			int heightmapResolution = terrainData.heightmapResolution;
			Vector3 heightmapScale = terrainData.heightmapScale;
			float[,] heights = terrainData.GetHeights(0, 0, heightmapResolution, heightmapResolution);
			float[] array = new float[heightmapResolution * heightmapResolution];
			for (int i = 0; i < heightmapResolution; i++)
			{
				for (int j = 0; j < heightmapResolution; j++)
				{
					array[j + i * heightmapResolution] = heights[i, j] * heightmapScale.y + terrain.transform.position.y;
				}
			}
			return array;
		}

		public static Texture2D CreateMask(float[] heightMap, int size, float shoreLevel, float spread, TextureFormat format)
		{
			Texture2D texture2D = new Texture2D(size, size, format, false, true);
			texture2D.filterMode = FilterMode.Bilinear;
			int num = size * size;
			Color[] array = new Color[num];
			for (int i = 0; i < num; i++)
			{
				float num2 = Mathf.Clamp(shoreLevel - heightMap[i], 0f, spread);
				num2 = 1f - num2 / spread;
				array[i].r = num2;
				array[i].g = num2;
				array[i].b = num2;
				array[i].a = num2;
			}
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}

		public static Texture2D CreateMask(InterpolatedArray2f heightMap, int width, int height, float shoreLevel, float spread, TextureFormat format)
		{
			Texture2D texture2D = new Texture2D(width, height, format, false, true);
			texture2D.filterMode = FilterMode.Bilinear;
			Color[] array = new Color[width * height];
			bool flag = width == heightMap.SX && height == heightMap.SY;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					int num = j + i * height;
					float num2;
					if (flag)
					{
						num2 = Mathf.Clamp(shoreLevel - heightMap.Data[num], 0f, spread);
					}
					else
					{
						float x = (float)j / ((float)width - 1f);
						float y = (float)i / ((float)height - 1f);
						num2 = Mathf.Clamp(shoreLevel - heightMap.Get(x, y, 0), 0f, spread);
					}
					num2 = 1f - num2 / spread;
					array[num].r = num2;
					array[num].g = num2;
					array[num].b = num2;
					array[num].a = num2;
				}
			}
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}

		public static Texture2D CreateClipMask(float[] heightMap, int size, float shoreLevel, TextureFormat format)
		{
			Texture2D texture2D = new Texture2D(size, size, format, false, true);
			texture2D.filterMode = FilterMode.Bilinear;
			int num = size * size;
			Color[] array = new Color[num];
			for (int i = 0; i < num; i++)
			{
				float num2 = Mathf.Clamp(heightMap[i] - shoreLevel, 0f, 1f);
				if (num2 > 0f)
				{
					num2 = 1f;
				}
				array[i].r = num2;
				array[i].g = num2;
				array[i].b = num2;
				array[i].a = num2;
			}
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}

		public static Texture2D CreateClipMask(InterpolatedArray2f heightMap, int width, int height, float shoreLevel, TextureFormat format)
		{
			Texture2D texture2D = new Texture2D(width, height, format, false, true);
			texture2D.filterMode = FilterMode.Bilinear;
			Color[] array = new Color[width * height];
			bool flag = width == heightMap.SX && height == heightMap.SY;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					int num = j + i * height;
					float num2;
					if (flag)
					{
						num2 = Mathf.Clamp(heightMap.Data[num] - shoreLevel, 0f, 1f);
					}
					else
					{
						float x = (float)j / ((float)width - 1f);
						float y = (float)i / ((float)height - 1f);
						num2 = Mathf.Clamp(heightMap.Get(x, y, 0) - shoreLevel, 0f, 1f);
					}
					if (num2 > 0f)
					{
						num2 = 1f;
					}
					array[num].r = num2;
					array[num].g = num2;
					array[num].b = num2;
					array[num].a = num2;
				}
			}
			texture2D.SetPixels(array);
			texture2D.Apply();
			return texture2D;
		}
	}
}
