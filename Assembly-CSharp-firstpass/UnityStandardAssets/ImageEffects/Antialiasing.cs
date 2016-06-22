using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[AddComponentMenu("Image Effects/Other/Antialiasing"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
	public class Antialiasing : PostEffectsBase
	{
		public AAMode mode = AAMode.FXAA3Console;

		public bool showGeneratedNormals;

		public float offsetScale = 0.2f;

		public float blurRadius = 18f;

		public float edgeThresholdMin = 0.05f;

		public float edgeThreshold = 0.2f;

		public float edgeSharpness = 4f;

		public bool dlaaSharp;

		public Shader ssaaShader;

		private Material ssaa;

		public Shader dlaaShader;

		private Material dlaa;

		public Shader nfaaShader;

		private Material nfaa;

		public Shader shaderFXAAPreset2;

		private Material materialFXAAPreset2;

		public Shader shaderFXAAPreset3;

		private Material materialFXAAPreset3;

		public Shader shaderFXAAII;

		private Material materialFXAAII;

		public Shader shaderFXAAIII;

		private Material materialFXAAIII;

		public Material CurrentAAMaterial()
		{
			Material result;
			switch (this.mode)
			{
			case AAMode.FXAA2:
				result = this.materialFXAAII;
				break;
			case AAMode.FXAA3Console:
				result = this.materialFXAAIII;
				break;
			case AAMode.FXAA1PresetA:
				result = this.materialFXAAPreset2;
				break;
			case AAMode.FXAA1PresetB:
				result = this.materialFXAAPreset3;
				break;
			case AAMode.NFAA:
				result = this.nfaa;
				break;
			case AAMode.SSAA:
				result = this.ssaa;
				break;
			case AAMode.DLAA:
				result = this.dlaa;
				break;
			default:
				result = null;
				break;
			}
			return result;
		}

		public override bool CheckResources()
		{
			base.CheckSupport(false);
			this.materialFXAAPreset2 = base.CreateMaterial(this.shaderFXAAPreset2, this.materialFXAAPreset2);
			this.materialFXAAPreset3 = base.CreateMaterial(this.shaderFXAAPreset3, this.materialFXAAPreset3);
			this.materialFXAAII = base.CreateMaterial(this.shaderFXAAII, this.materialFXAAII);
			this.materialFXAAIII = base.CreateMaterial(this.shaderFXAAIII, this.materialFXAAIII);
			this.nfaa = base.CreateMaterial(this.nfaaShader, this.nfaa);
			this.ssaa = base.CreateMaterial(this.ssaaShader, this.ssaa);
			this.dlaa = base.CreateMaterial(this.dlaaShader, this.dlaa);
			if (!this.ssaaShader.isSupported)
			{
				base.NotSupported();
				base.ReportAutoDisable();
			}
			return this.isSupported;
		}

		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!this.CheckResources())
			{
				Graphics.Blit(source, destination);
				return;
			}
			if (this.mode == AAMode.FXAA3Console && this.materialFXAAIII != null)
			{
				this.materialFXAAIII.SetFloat("_EdgeThresholdMin", this.edgeThresholdMin);
				this.materialFXAAIII.SetFloat("_EdgeThreshold", this.edgeThreshold);
				this.materialFXAAIII.SetFloat("_EdgeSharpness", this.edgeSharpness);
				Graphics.Blit(source, destination, this.materialFXAAIII);
			}
			else if (this.mode == AAMode.FXAA1PresetB && this.materialFXAAPreset3 != null)
			{
				Graphics.Blit(source, destination, this.materialFXAAPreset3);
			}
			else if (this.mode == AAMode.FXAA1PresetA && this.materialFXAAPreset2 != null)
			{
				source.anisoLevel = 4;
				Graphics.Blit(source, destination, this.materialFXAAPreset2);
				source.anisoLevel = 0;
			}
			else if (this.mode == AAMode.FXAA2 && this.materialFXAAII != null)
			{
				Graphics.Blit(source, destination, this.materialFXAAII);
			}
			else if (this.mode == AAMode.SSAA && this.ssaa != null)
			{
				Graphics.Blit(source, destination, this.ssaa);
			}
			else if (this.mode == AAMode.DLAA && this.dlaa != null)
			{
				source.anisoLevel = 0;
				RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height);
				Graphics.Blit(source, temporary, this.dlaa, 0);
				Graphics.Blit(temporary, destination, this.dlaa, (!this.dlaaSharp) ? 1 : 2);
				RenderTexture.ReleaseTemporary(temporary);
			}
			else if (this.mode == AAMode.NFAA && this.nfaa != null)
			{
				source.anisoLevel = 0;
				this.nfaa.SetFloat("_OffsetScale", this.offsetScale);
				this.nfaa.SetFloat("_BlurRadius", this.blurRadius);
				Graphics.Blit(source, destination, this.nfaa, (!this.showGeneratedNormals) ? 0 : 1);
			}
			else
			{
				Graphics.Blit(source, destination);
			}
		}
	}
}
