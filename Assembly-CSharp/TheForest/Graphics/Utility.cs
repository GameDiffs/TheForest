using System;
using UnityEngine;

namespace TheForest.Graphics
{
	public static class Utility
	{
		public enum TextureResolution
		{
			_32 = 32,
			_64 = 64,
			_128 = 128,
			_256 = 256,
			_512 = 512,
			_1024 = 1024,
			_2048 = 2048,
			_4096 = 4096,
			_8192 = 8192
		}

		public static RenderTexture CreateRenderTexture(int width, int height, int depth, RenderTextureFormat textureFormat, FilterMode filterMode, TextureWrapMode wrapMode, int anisoLevel = 1, bool useMipMap = false)
		{
			RenderTexture renderTexture = new RenderTexture(width, height, depth, textureFormat);
			renderTexture.filterMode = filterMode;
			renderTexture.wrapMode = wrapMode;
			renderTexture.anisoLevel = anisoLevel;
			renderTexture.useMipMap = useMipMap;
			renderTexture.Create();
			return renderTexture;
		}

		public static Texture2D CreateTexture2D(int width, int height, TextureFormat textureFormat, bool mipmap, bool linear, FilterMode filterMode, TextureWrapMode wrapMode, Color[] colors = null)
		{
			Texture2D texture2D = new Texture2D(width, height, textureFormat, mipmap, linear);
			texture2D.filterMode = filterMode;
			texture2D.wrapMode = wrapMode;
			if (colors != null)
			{
				texture2D.SetPixels(colors);
				texture2D.Apply();
			}
			return texture2D;
		}

		public static Texture3D CreateTexture3D(int width, int height, int depth, TextureFormat textureFormat, bool useMipMaps, TextureWrapMode wrapMode, FilterMode filterMode)
		{
			return new Texture3D(width, height, depth, textureFormat, useMipMaps)
			{
				wrapMode = wrapMode,
				filterMode = filterMode
			};
		}

		public static GameObject CreateGameObject(string name, Mesh mesh = null, Material material = null)
		{
			GameObject gameObject = new GameObject(name);
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			meshFilter.mesh = mesh;
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.material = material;
			return gameObject;
		}

		public static void CopyTo(this Transform source, Transform dest)
		{
			dest.position = source.position;
			dest.rotation = source.rotation;
			dest.localScale = source.localScale;
		}

		public static void CopyFrom(this Transform source, Transform dest)
		{
			source.position = dest.position;
			source.rotation = dest.rotation;
			source.localScale = dest.localScale;
		}

		public static void Reset(this Transform source)
		{
			source.localPosition = Vector3.zero;
			source.localEulerAngles = Vector3.zero;
			source.localScale = Vector3.one;
		}
	}
}
