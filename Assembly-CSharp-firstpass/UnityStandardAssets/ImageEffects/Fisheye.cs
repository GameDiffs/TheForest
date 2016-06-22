using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[AddComponentMenu("Image Effects/Displacement/Fisheye"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
	internal class Fisheye : PostEffectsBase
	{
		public float strengthX = 0.05f;

		public float strengthY = 0.05f;

		public Shader fishEyeShader;

		private Material fisheyeMaterial;

		public override bool CheckResources()
		{
			base.CheckSupport(false);
			this.fisheyeMaterial = base.CheckShaderAndCreateMaterial(this.fishEyeShader, this.fisheyeMaterial);
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
			float num = 0.15625f;
			float num2 = (float)source.width * 1f / ((float)source.height * 1f);
			this.fisheyeMaterial.SetVector("intensity", new Vector4(this.strengthX * num2 * num, this.strengthY * num, this.strengthX * num2 * num, this.strengthY * num));
			Graphics.Blit(source, destination, this.fisheyeMaterial);
		}
	}
}
