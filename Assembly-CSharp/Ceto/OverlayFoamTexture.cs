using System;
using UnityEngine;

namespace Ceto
{
	[Serializable]
	public class OverlayFoamTexture
	{
		public Texture tex;

		public Vector2 scaleUV = Vector2.one;

		public Vector2 offsetUV;

		public bool textureFoam = true;

		[Range(0f, 4f)]
		public float alpha = 1f;

		public Texture mask;

		public OVERLAY_MASK_MODE maskMode;

		[Range(0f, 1f)]
		public float maskAlpha = 1f;

		public bool IsDrawable
		{
			get
			{
				return (this.alpha != 0f && this.tex != null) || (this.maskAlpha != 0f && this.mask != null);
			}
		}
	}
}
