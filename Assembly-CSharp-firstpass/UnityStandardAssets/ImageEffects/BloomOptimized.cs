using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[AddComponentMenu("Image Effects/Bloom and Glow/Bloom (Optimized)"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
	public class BloomOptimized : PostEffectsBase
	{
		public enum Resolution
		{
			Low,
			High
		}

		public enum BlurType
		{
			Standard,
			Sgx
		}

		[Range(0f, 1.5f)]
		public float threshold = 0.25f;

		[Range(0f, 2.5f)]
		public float intensity = 0.75f;

		[Range(0.25f, 5.5f)]
		public float blurSize = 1f;

		private BloomOptimized.Resolution resolution;

		[Range(1f, 4f)]
		public int blurIterations = 1;

		public BloomOptimized.BlurType blurType;

		public Shader fastBloomShader;

		private Material fastBloomMaterial;

		public override bool CheckResources()
		{
			base.CheckSupport(false);
			this.fastBloomMaterial = base.CheckShaderAndCreateMaterial(this.fastBloomShader, this.fastBloomMaterial);
			if (!this.isSupported)
			{
				base.ReportAutoDisable();
			}
			return this.isSupported;
		}

		private void OnDisable()
		{
			if (this.fastBloomMaterial)
			{
				UnityEngine.Object.DestroyImmediate(this.fastBloomMaterial);
			}
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!this.CheckResources())
			{
				Graphics.Blit(source, destination);
				return;
			}
			int num = (this.resolution != BloomOptimized.Resolution.Low) ? 2 : 4;
			float num2 = (this.resolution != BloomOptimized.Resolution.Low) ? 1f : 0.5f;
			this.fastBloomMaterial.SetVector("_Parameter", new Vector4(this.blurSize * num2, 0f, this.threshold, this.intensity));
			source.filterMode = FilterMode.Bilinear;
			int width = source.width / num;
			int height = source.height / num;
			RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
			renderTexture.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, renderTexture, this.fastBloomMaterial, 1);
			int num3 = (this.blurType != BloomOptimized.BlurType.Standard) ? 2 : 0;
			for (int i = 0; i < this.blurIterations; i++)
			{
				this.fastBloomMaterial.SetVector("_Parameter", new Vector4(this.blurSize * num2 + (float)i * 1f, 0f, this.threshold, this.intensity));
				RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, this.fastBloomMaterial, 2 + num3);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
				temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, this.fastBloomMaterial, 3 + num3);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
			}
			this.fastBloomMaterial.SetTexture("_Bloom", renderTexture);
			Graphics.Blit(source, destination, this.fastBloomMaterial, 0);
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}
}
