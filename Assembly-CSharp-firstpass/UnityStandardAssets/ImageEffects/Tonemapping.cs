using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[AddComponentMenu("Image Effects/Color Adjustments/Tonemapping"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
	public class Tonemapping : PostEffectsBase
	{
		public enum TonemapperType
		{
			SimpleReinhard,
			UserCurve,
			Hable,
			Photographic,
			OptimizedHejiDawson,
			AdaptiveReinhard,
			AdaptiveReinhardAutoWhite
		}

		public enum AdaptiveTexSize
		{
			Square16 = 16,
			Square32 = 32,
			Square64 = 64,
			Square128 = 128,
			Square256 = 256,
			Square512 = 512,
			Square1024 = 1024
		}

		public Tonemapping.TonemapperType type = Tonemapping.TonemapperType.Photographic;

		public Tonemapping.AdaptiveTexSize adaptiveTextureSize = Tonemapping.AdaptiveTexSize.Square256;

		public AnimationCurve remapCurve;

		private Texture2D curveTex;

		public float exposureAdjustment = 1.5f;

		public float middleGrey = 0.4f;

		public float white = 2f;

		public float adaptionSpeed = 1.5f;

		public Shader tonemapper;

		public bool validRenderTextureFormat = true;

		private Material tonemapMaterial;

		private RenderTexture rt;

		private RenderTextureFormat rtFormat = RenderTextureFormat.ARGBHalf;

		public override bool CheckResources()
		{
			base.CheckSupport(false, true);
			this.tonemapMaterial = base.CheckShaderAndCreateMaterial(this.tonemapper, this.tonemapMaterial);
			if (!this.curveTex && this.type == Tonemapping.TonemapperType.UserCurve)
			{
				this.curveTex = new Texture2D(256, 1, TextureFormat.ARGB32, false, true);
				this.curveTex.filterMode = FilterMode.Bilinear;
				this.curveTex.wrapMode = TextureWrapMode.Clamp;
				this.curveTex.hideFlags = HideFlags.DontSave;
			}
			if (!this.isSupported)
			{
				base.ReportAutoDisable();
			}
			return this.isSupported;
		}

		public float UpdateCurve()
		{
			float num = 1f;
			if (this.remapCurve.keys.Length < 1)
			{
				this.remapCurve = new AnimationCurve(new Keyframe[]
				{
					new Keyframe(0f, 0f),
					new Keyframe(2f, 1f)
				});
			}
			if (this.remapCurve != null)
			{
				if (this.remapCurve.length > 0)
				{
					num = this.remapCurve[this.remapCurve.length - 1].time;
				}
				for (float num2 = 0f; num2 <= 1f; num2 += 0.003921569f)
				{
					float num3 = this.remapCurve.Evaluate(num2 * 1f * num);
					this.curveTex.SetPixel((int)Mathf.Floor(num2 * 255f), 0, new Color(num3, num3, num3));
				}
				this.curveTex.Apply();
			}
			return 1f / num;
		}

		private void OnDisable()
		{
			if (this.rt)
			{
				UnityEngine.Object.DestroyImmediate(this.rt);
				this.rt = null;
			}
			if (this.tonemapMaterial)
			{
				UnityEngine.Object.DestroyImmediate(this.tonemapMaterial);
				this.tonemapMaterial = null;
			}
			if (this.curveTex)
			{
				UnityEngine.Object.DestroyImmediate(this.curveTex);
				this.curveTex = null;
			}
		}

		private bool CreateInternalRenderTexture()
		{
			if (this.rt)
			{
				return false;
			}
			this.rtFormat = ((!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGHalf)) ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.RGHalf);
			this.rt = new RenderTexture(1, 1, 0, this.rtFormat);
			this.rt.hideFlags = HideFlags.DontSave;
			return true;
		}

		[ImageEffectTransformsToLDR]
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!this.CheckResources())
			{
				Graphics.Blit(source, destination);
				return;
			}
			this.exposureAdjustment = ((this.exposureAdjustment >= 0.001f) ? this.exposureAdjustment : 0.001f);
			if (this.type == Tonemapping.TonemapperType.UserCurve)
			{
				float value = this.UpdateCurve();
				this.tonemapMaterial.SetFloat("_RangeScale", value);
				this.tonemapMaterial.SetTexture("_Curve", this.curveTex);
				Graphics.Blit(source, destination, this.tonemapMaterial, 4);
				return;
			}
			if (this.type == Tonemapping.TonemapperType.SimpleReinhard)
			{
				this.tonemapMaterial.SetFloat("_ExposureAdjustment", this.exposureAdjustment);
				Graphics.Blit(source, destination, this.tonemapMaterial, 6);
				return;
			}
			if (this.type == Tonemapping.TonemapperType.Hable)
			{
				this.tonemapMaterial.SetFloat("_ExposureAdjustment", this.exposureAdjustment);
				Graphics.Blit(source, destination, this.tonemapMaterial, 5);
				return;
			}
			if (this.type == Tonemapping.TonemapperType.Photographic)
			{
				this.tonemapMaterial.SetFloat("_ExposureAdjustment", this.exposureAdjustment);
				Graphics.Blit(source, destination, this.tonemapMaterial, 8);
				return;
			}
			if (this.type == Tonemapping.TonemapperType.OptimizedHejiDawson)
			{
				this.tonemapMaterial.SetFloat("_ExposureAdjustment", 0.5f * this.exposureAdjustment);
				Graphics.Blit(source, destination, this.tonemapMaterial, 7);
				return;
			}
			bool flag = this.CreateInternalRenderTexture();
			RenderTexture temporary = RenderTexture.GetTemporary((int)this.adaptiveTextureSize, (int)this.adaptiveTextureSize, 0, this.rtFormat);
			Graphics.Blit(source, temporary);
			int num = (int)Mathf.Log((float)temporary.width * 1f, 2f);
			int num2 = 2;
			RenderTexture[] array = new RenderTexture[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = RenderTexture.GetTemporary(temporary.width / num2, temporary.width / num2, 0, this.rtFormat);
				num2 *= 2;
			}
			RenderTexture source2 = array[num - 1];
			Graphics.Blit(temporary, array[0], this.tonemapMaterial, 1);
			if (this.type == Tonemapping.TonemapperType.AdaptiveReinhardAutoWhite)
			{
				for (int j = 0; j < num - 1; j++)
				{
					Graphics.Blit(array[j], array[j + 1], this.tonemapMaterial, 9);
					source2 = array[j + 1];
				}
			}
			else if (this.type == Tonemapping.TonemapperType.AdaptiveReinhard)
			{
				for (int k = 0; k < num - 1; k++)
				{
					Graphics.Blit(array[k], array[k + 1]);
					source2 = array[k + 1];
				}
			}
			this.adaptionSpeed = ((this.adaptionSpeed >= 0.001f) ? this.adaptionSpeed : 0.001f);
			this.tonemapMaterial.SetFloat("_AdaptionSpeed", this.adaptionSpeed);
			this.rt.MarkRestoreExpected();
			Graphics.Blit(source2, this.rt, this.tonemapMaterial, (!flag) ? 2 : 3);
			this.middleGrey = ((this.middleGrey >= 0.001f) ? this.middleGrey : 0.001f);
			this.tonemapMaterial.SetVector("_HdrParams", new Vector4(this.middleGrey, this.middleGrey, this.middleGrey, this.white * this.white));
			this.tonemapMaterial.SetTexture("_SmallTex", this.rt);
			if (this.type == Tonemapping.TonemapperType.AdaptiveReinhard)
			{
				Graphics.Blit(source, destination, this.tonemapMaterial, 0);
			}
			else if (this.type == Tonemapping.TonemapperType.AdaptiveReinhardAutoWhite)
			{
				Graphics.Blit(source, destination, this.tonemapMaterial, 10);
			}
			else
			{
				Debug.LogError("No valid adaptive tonemapper type found!");
				Graphics.Blit(source, destination);
			}
			for (int l = 0; l < num; l++)
			{
				RenderTexture.ReleaseTemporary(array[l]);
			}
			RenderTexture.ReleaseTemporary(temporary);
		}
	}
}
