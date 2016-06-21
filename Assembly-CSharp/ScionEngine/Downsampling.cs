using System;
using UnityEngine;

namespace ScionEngine
{
	public class Downsampling
	{
		private const int FireflyRemovingPass = 0;

		private const int FireflyRemovingBilateralPass = 1;

		private const int DepthPass = 2;

		private Material m_downsampleMat;

		public Downsampling()
		{
			this.m_downsampleMat = new Material(Shader.Find("Hidden/ScionDownsampling"));
			this.m_downsampleMat.hideFlags = HideFlags.HideAndDontSave;
		}

		public RenderTexture DownsampleFireflyRemoving(RenderTexture source)
		{
			int width = source.width / 2;
			int height = source.height / 2;
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary.filterMode = FilterMode.Bilinear;
			temporary.wrapMode = TextureWrapMode.Clamp;
			source.filterMode = FilterMode.Bilinear;
			source.wrapMode = TextureWrapMode.Clamp;
			Graphics.Blit(source, temporary, this.m_downsampleMat, 0);
			return temporary;
		}

		public RenderTexture DownsampleFireflyRemovingBilateral(RenderTexture source, RenderTexture halfResDepth)
		{
			int width = source.width / 2;
			int height = source.height / 2;
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary.filterMode = FilterMode.Bilinear;
			temporary.wrapMode = TextureWrapMode.Clamp;
			source.filterMode = FilterMode.Point;
			source.wrapMode = TextureWrapMode.Clamp;
			halfResDepth.filterMode = FilterMode.Point;
			halfResDepth.wrapMode = TextureWrapMode.Clamp;
			this.m_downsampleMat.SetTexture("_HalfResDepth", halfResDepth);
			Graphics.Blit(source, temporary, this.m_downsampleMat, 1);
			return temporary;
		}

		public RenderTexture DownsampleDepthTexture(int width, int height)
		{
			int width2 = width / 2;
			int height2 = height / 2;
			RenderTexture temporary = RenderTexture.GetTemporary(width2, height2, 0, RenderTextureFormat.RHalf);
			temporary.filterMode = FilterMode.Point;
			temporary.wrapMode = TextureWrapMode.Clamp;
			ScionGraphics.Blit(temporary, this.m_downsampleMat, 2);
			return temporary;
		}

		public RenderTexture Downsample(RenderTexture source)
		{
			int width = source.width / 2;
			int height = source.height / 2;
			FilterMode filterMode = source.filterMode;
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary.filterMode = source.filterMode;
			temporary.wrapMode = source.wrapMode;
			source.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, temporary);
			source.filterMode = filterMode;
			return temporary;
		}
	}
}
