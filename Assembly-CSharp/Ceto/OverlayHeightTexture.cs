using System;
using UnityEngine;

namespace Ceto
{
	[Serializable]
	public class OverlayHeightTexture
	{
		public Texture tex;

		public Vector2 scaleUV = Vector2.one;

		public Vector2 offsetUV;

		[Range(-20f, 20f)]
		public float alpha = 1f;

		public Texture mask;

		public OVERLAY_MASK_MODE maskMode = OVERLAY_MASK_MODE.WAVES_AND_OVERLAY_BLEND;

		[Range(0f, 1f)]
		public float maskAlpha = 1f;

		public bool ignoreQuerys;

		public bool IsDrawable
		{
			get
			{
				return (this.alpha != 0f && this.tex != null) || (this.maskAlpha != 0f && this.mask != null);
			}
		}
	}
}
