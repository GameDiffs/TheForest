using System;
using UnityEngine;

namespace ScionEngine
{
	public class Bloom
	{
		private Material m_bloomMat;

		private RenderTexture[] m_bloomTextures;

		private int numDownsamples = -1;

		private int iteratedTextures = 1;

		public Bloom()
		{
			this.m_bloomMat = new Material(Shader.Find("Hidden/ScionBloom"));
			this.m_bloomMat.hideFlags = HideFlags.HideAndDontSave;
		}

		public void ReleaseResources()
		{
			if (this.m_bloomMat != null)
			{
				UnityEngine.Object.Destroy(this.m_bloomMat);
				this.m_bloomMat = null;
			}
		}

		public bool PlatformCompatibility()
		{
			return Shader.Find("Hidden/ScionBloom").isSupported;
		}

		public RenderTexture TryGetSmallBloomTexture(int minimumReqPixels)
		{
			this.iteratedTextures = 1;
			for (int i = this.numDownsamples - 1; i >= 0; i--)
			{
				int num = (this.m_bloomTextures[i].width <= this.m_bloomTextures[i].height) ? this.m_bloomTextures[i].width : this.m_bloomTextures[i].height;
				if (num >= minimumReqPixels)
				{
					return this.m_bloomTextures[i];
				}
				this.iteratedTextures++;
			}
			return null;
		}

		public float GetEnergyNormalizer()
		{
			if (this.iteratedTextures == this.numDownsamples)
			{
				return 1f;
			}
			return 1f / (float)this.iteratedTextures;
		}

		public void EndOfFrameCleanup()
		{
			if (this.m_bloomTextures == null)
			{
				return;
			}
			for (int i = 0; i < this.numDownsamples; i++)
			{
				RenderTexture.ReleaseTemporary(this.m_bloomTextures[i]);
				this.m_bloomTextures[i] = null;
			}
		}

		public RenderTexture CreateBloomTexture(RenderTexture halfResSource, BloomParameters bloomParams)
		{
			if (this.numDownsamples != bloomParams.downsamples)
			{
				this.numDownsamples = bloomParams.downsamples;
				this.m_bloomTextures = new RenderTexture[this.numDownsamples];
			}
			halfResSource.filterMode = FilterMode.Bilinear;
			RenderTextureFormat format = halfResSource.format;
			int num = halfResSource.width;
			int num2 = halfResSource.height;
			for (int i = 0; i < this.numDownsamples; i++)
			{
				this.m_bloomTextures[i] = RenderTexture.GetTemporary(num, num2, 0, format);
				this.m_bloomTextures[i].filterMode = FilterMode.Bilinear;
				this.m_bloomTextures[i].wrapMode = TextureWrapMode.Clamp;
				num /= 2;
				num2 /= 2;
			}
			halfResSource.filterMode = FilterMode.Bilinear;
			RenderTexture source = halfResSource;
			for (int j = 1; j < this.numDownsamples; j++)
			{
				Graphics.Blit(source, this.m_bloomTextures[j], this.m_bloomMat, 0);
				source = this.m_bloomTextures[j];
			}
			for (int k = this.numDownsamples - 1; k > 1; k--)
			{
				Graphics.Blit(this.m_bloomTextures[k], this.m_bloomTextures[k - 1], this.m_bloomMat, 1);
			}
			this.m_bloomMat.SetFloat("_EnergyNormalizer", 1f / (float)this.numDownsamples);
			Graphics.Blit(this.m_bloomTextures[1], this.m_bloomTextures[0], this.m_bloomMat, 2);
			return this.m_bloomTextures[0];
		}
	}
}
