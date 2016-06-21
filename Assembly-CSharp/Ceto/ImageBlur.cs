using System;
using UnityEngine;

namespace Ceto
{
	public class ImageBlur
	{
		public enum BLUR_MODE
		{
			OFF,
			NO_DOWNSAMPLE,
			DOWNSAMPLE_2,
			DOWNSAMPLE_4 = 4
		}

		private Vector2[] m_offsets = new Vector2[4];

		public Material m_blurMaterial;

		public ImageBlur.BLUR_MODE BlurMode
		{
			get;
			set;
		}

		public int BlurIterations
		{
			get;
			set;
		}

		public float BlurSpread
		{
			get;
			set;
		}

		public ImageBlur(Shader blurShader)
		{
			this.BlurIterations = 1;
			this.BlurSpread = 0.6f;
			this.BlurMode = ImageBlur.BLUR_MODE.DOWNSAMPLE_2;
			if (blurShader != null)
			{
				this.m_blurMaterial = new Material(blurShader);
			}
		}

		public void Blur(RenderTexture source)
		{
			int blurMode = (int)this.BlurMode;
			if (this.BlurIterations > 0 && this.m_blurMaterial != null && blurMode > 0)
			{
				int width = source.width / blurMode;
				int height = source.height / blurMode;
				RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format, RenderTextureReadWrite.Default);
				this.DownSample(source, renderTexture);
				for (int i = 0; i < this.BlurIterations; i++)
				{
					RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format, RenderTextureReadWrite.Default);
					this.FourTapCone(renderTexture, temporary, i);
					RenderTexture.ReleaseTemporary(renderTexture);
					renderTexture = temporary;
				}
				Graphics.Blit(renderTexture, source);
				RenderTexture.ReleaseTemporary(renderTexture);
			}
		}

		private void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
		{
			float num = 0.5f + (float)iteration * this.BlurSpread;
			this.m_offsets[0].x = -num;
			this.m_offsets[0].y = -num;
			this.m_offsets[1].x = -num;
			this.m_offsets[1].y = num;
			this.m_offsets[2].x = num;
			this.m_offsets[2].y = num;
			this.m_offsets[3].x = num;
			this.m_offsets[3].y = -num;
			Graphics.BlitMultiTap(source, dest, this.m_blurMaterial, this.m_offsets);
		}

		private void DownSample(RenderTexture source, RenderTexture dest)
		{
			float num = 1f;
			this.m_offsets[0].x = -num;
			this.m_offsets[0].y = -num;
			this.m_offsets[1].x = -num;
			this.m_offsets[1].y = num;
			this.m_offsets[2].x = num;
			this.m_offsets[2].y = num;
			this.m_offsets[3].x = num;
			this.m_offsets[3].y = -num;
			Graphics.BlitMultiTap(source, dest, this.m_blurMaterial, this.m_offsets);
		}
	}
}
