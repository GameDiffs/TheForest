using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("Draws a GUI Texture. NOTE: Uses OnGUI so you need a PlayMakerGUI component in the scene.")]
	public class DrawTexture : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("Texture to draw.")]
		public FsmTexture texture;

		[Title("Position"), HutongGames.PlayMaker.Tooltip("Rectangle on the screen to draw the texture within. Alternatively, set or override individual properties below."), UIHint(UIHint.Variable)]
		public FsmRect screenRect;

		[HutongGames.PlayMaker.Tooltip("Left screen coordinate.")]
		public FsmFloat left;

		[HutongGames.PlayMaker.Tooltip("Top screen coordinate.")]
		public FsmFloat top;

		[HutongGames.PlayMaker.Tooltip("Width of texture on screen.")]
		public FsmFloat width;

		[HutongGames.PlayMaker.Tooltip("Height of texture on screen.")]
		public FsmFloat height;

		[HutongGames.PlayMaker.Tooltip("How to scale the image when the aspect ratio of it doesn't fit the aspect ratio to be drawn within.")]
		public ScaleMode scaleMode;

		[HutongGames.PlayMaker.Tooltip("Whether to alpha blend the image on to the display (the default). If false, the picture is drawn on to the display.")]
		public FsmBool alphaBlend;

		[HutongGames.PlayMaker.Tooltip("Aspect ratio to use for the source image. If 0 (the default), the aspect ratio from the image is used. Pass in w/h for the desired aspect ratio. This allows the aspect ratio of the source image to be adjusted without changing the pixel width and height.")]
		public FsmFloat imageAspect;

		[HutongGames.PlayMaker.Tooltip("Use normalized screen coordinates (0-1)")]
		public FsmBool normalized;

		private Rect rect;

		public override void Reset()
		{
			this.texture = null;
			this.screenRect = null;
			this.left = 0f;
			this.top = 0f;
			this.width = 1f;
			this.height = 1f;
			this.scaleMode = ScaleMode.StretchToFill;
			this.alphaBlend = true;
			this.imageAspect = 0f;
			this.normalized = true;
		}

		public override void OnGUI()
		{
			if (this.texture.Value == null)
			{
				return;
			}
			this.rect = (this.screenRect.IsNone ? default(Rect) : this.screenRect.Value);
			if (!this.left.IsNone)
			{
				this.rect.x = this.left.Value;
			}
			if (!this.top.IsNone)
			{
				this.rect.y = this.top.Value;
			}
			if (!this.width.IsNone)
			{
				this.rect.width = this.width.Value;
			}
			if (!this.height.IsNone)
			{
				this.rect.height = this.height.Value;
			}
			if (this.normalized.Value)
			{
				this.rect.x = this.rect.x * (float)Screen.width;
				this.rect.width = this.rect.width * (float)Screen.width;
				this.rect.y = this.rect.y * (float)Screen.height;
				this.rect.height = this.rect.height * (float)Screen.height;
			}
			GUI.DrawTexture(this.rect, this.texture.Value, this.scaleMode, this.alphaBlend.Value, this.imageAspect.Value);
		}
	}
}
