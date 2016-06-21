using System;
using UnityEngine;

namespace Ceto
{
	[Serializable]
	public class OverlayClipTexture
	{
		public Texture tex;

		public Vector2 scaleUV = Vector2.one;

		public Vector2 offsetUV;

		[Range(0f, 4f)]
		public float alpha = 1f;

		public bool ignoreQuerys;

		public bool IsDrawable
		{
			get
			{
				return this.alpha != 0f && this.tex != null;
			}
		}
	}
}
