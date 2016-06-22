using System;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	public static class ImageEffectHelper
	{
		public static bool supportsDX11
		{
			get
			{
				return SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
			}
		}

		public static bool IsSupported(Shader s, bool needDepth, bool needHdr, MonoBehaviour effect)
		{
			if (s == null || !s.isSupported)
			{
				Debug.LogWarningFormat("Missing shader for image effect {0}", new object[]
				{
					effect
				});
				return false;
			}
			return SystemInfo.supportsImageEffects && SystemInfo.supportsRenderTextures && (!needDepth || SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth)) && (!needHdr || SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf));
		}

		public static Material CheckShaderAndCreateMaterial(Shader s)
		{
			if (s == null || !s.isSupported)
			{
				return null;
			}
			return new Material(s)
			{
				hideFlags = HideFlags.DontSave
			};
		}
	}
}
