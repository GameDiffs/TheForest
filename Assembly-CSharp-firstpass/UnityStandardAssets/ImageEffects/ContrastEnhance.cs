using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[AddComponentMenu("Image Effects/Color Adjustments/Contrast Enhance (Unsharp Mask)"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
	internal class ContrastEnhance : PostEffectsBase
	{
		public float intensity = 0.5f;

		public float threshold;

		private Material separableBlurMaterial;

		private Material contrastCompositeMaterial;

		public float blurSpread = 1f;

		public Shader separableBlurShader;

		public Shader contrastCompositeShader;

		public override bool CheckResources()
		{
			base.CheckSupport(false);
			this.contrastCompositeMaterial = base.CheckShaderAndCreateMaterial(this.contrastCompositeShader, this.contrastCompositeMaterial);
			this.separableBlurMaterial = base.CheckShaderAndCreateMaterial(this.separableBlurShader, this.separableBlurMaterial);
			if (!this.isSupported)
			{
				base.ReportAutoDisable();
			}
			return this.isSupported;
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!this.CheckResources())
			{
				Graphics.Blit(source, destination);
				return;
			}
			int width = source.width;
			int height = source.height;
			RenderTexture temporary = RenderTexture.GetTemporary(width / 2, height / 2, 0);
			Graphics.Blit(source, temporary);
			RenderTexture temporary2 = RenderTexture.GetTemporary(width / 4, height / 4, 0);
			Graphics.Blit(temporary, temporary2);
			RenderTexture.ReleaseTemporary(temporary);
			this.separableBlurMaterial.SetVector("offsets", new Vector4(0f, this.blurSpread * 1f / (float)temporary2.height, 0f, 0f));
			RenderTexture temporary3 = RenderTexture.GetTemporary(width / 4, height / 4, 0);
			Graphics.Blit(temporary2, temporary3, this.separableBlurMaterial);
			RenderTexture.ReleaseTemporary(temporary2);
			this.separableBlurMaterial.SetVector("offsets", new Vector4(this.blurSpread * 1f / (float)temporary2.width, 0f, 0f, 0f));
			temporary2 = RenderTexture.GetTemporary(width / 4, height / 4, 0);
			Graphics.Blit(temporary3, temporary2, this.separableBlurMaterial);
			RenderTexture.ReleaseTemporary(temporary3);
			this.contrastCompositeMaterial.SetTexture("_MainTexBlurred", temporary2);
			this.contrastCompositeMaterial.SetFloat("intensity", this.intensity);
			this.contrastCompositeMaterial.SetFloat("threshhold", this.threshold);
			Graphics.Blit(source, destination, this.contrastCompositeMaterial);
			RenderTexture.ReleaseTemporary(temporary2);
		}
	}
}
