using System;
using UnityEngine;

namespace Ceto
{
	[Serializable]
	public struct InscatterModifier
	{
		[Range(0f, 5000f)]
		public float scale;

		[Range(0f, 2f)]
		public float intensity;

		public INSCATTER_MODE mode;

		public Color color;

		public InscatterModifier(float scale, float intensity, Color color, INSCATTER_MODE mode)
		{
			this.scale = scale;
			this.intensity = intensity;
			this.color = color;
			this.mode = mode;
		}
	}
}
