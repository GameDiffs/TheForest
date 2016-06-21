using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	public class RTSettings
	{
		public string name = string.Empty;

		public int width = 1;

		public int height = 1;

		public int depth;

		public int ansioLevel = 1;

		public bool mipmaps;

		public bool randomWrite;

		public RenderTextureReadWrite readWrite;

		public TextureWrapMode wrap = TextureWrapMode.Clamp;

		public FilterMode filer = FilterMode.Bilinear;

		public RenderTextureFormat format;

		public List<RenderTextureFormat> fallbackFormats = new List<RenderTextureFormat>();
	}
}
