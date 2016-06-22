using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[AddComponentMenu("Image Effects/Other/Screen Overlay"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
	public class ScreenOverlay : PostEffectsBase
	{
		public enum OverlayBlendMode
		{
			Additive,
			ScreenBlend,
			Multiply,
			Overlay,
			AlphaBlend
		}

		public ScreenOverlay.OverlayBlendMode blendMode = ScreenOverlay.OverlayBlendMode.Overlay;

		public float intensity = 1f;

		public Texture2D texture;

		public Shader overlayShader;

		private Material overlayMaterial;

		public override bool CheckResources()
		{
			base.CheckSupport(false);
			this.overlayMaterial = base.CheckShaderAndCreateMaterial(this.overlayShader, this.overlayMaterial);
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
			Vector4 vector = new Vector4(1f, 0f, 0f, 1f);
			this.overlayMaterial.SetVector("_UV_Transform", vector);
			this.overlayMaterial.SetFloat("_Intensity", this.intensity);
			this.overlayMaterial.SetTexture("_Overlay", this.texture);
			Graphics.Blit(source, destination, this.overlayMaterial, (int)this.blendMode);
		}
	}
}
