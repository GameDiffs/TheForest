using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[AddComponentMenu("Image Effects/Blur/Motion Blur (Color Accumulation)"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
	public class MotionBlur : ImageEffectBase
	{
		public float blurAmount = 0.8f;

		public bool extraBlur;

		private RenderTexture accumTexture;

		protected override void Start()
		{
			if (!SystemInfo.supportsRenderTextures)
			{
				base.enabled = false;
				return;
			}
			base.Start();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			UnityEngine.Object.DestroyImmediate(this.accumTexture);
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (this.accumTexture == null || this.accumTexture.width != source.width || this.accumTexture.height != source.height)
			{
				UnityEngine.Object.DestroyImmediate(this.accumTexture);
				this.accumTexture = new RenderTexture(source.width, source.height, 0);
				this.accumTexture.hideFlags = HideFlags.HideAndDontSave;
				Graphics.Blit(source, this.accumTexture);
			}
			if (this.extraBlur)
			{
				RenderTexture temporary = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
				this.accumTexture.MarkRestoreExpected();
				Graphics.Blit(this.accumTexture, temporary);
				Graphics.Blit(temporary, this.accumTexture);
				RenderTexture.ReleaseTemporary(temporary);
			}
			this.blurAmount = Mathf.Clamp(this.blurAmount, 0f, 0.92f);
			base.material.SetTexture("_MainTex", this.accumTexture);
			base.material.SetFloat("_AccumOrig", 1f - this.blurAmount);
			this.accumTexture.MarkRestoreExpected();
			Graphics.Blit(source, this.accumTexture, base.material);
			Graphics.Blit(this.accumTexture, destination);
		}
	}
}
