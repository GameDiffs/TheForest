using System;
using UnityEngine;

namespace Ceto
{
	[Serializable]
	public struct AbsorptionModifier
	{
		[Range(0f, 50f)]
		public float scale;

		[Range(0f, 10f)]
		public float intensity;

		public Color tint;

		public AbsorptionModifier(float scale, float intensity, Color tint)
		{
			this.scale = scale;
			this.intensity = intensity;
			this.tint = tint;
		}
	}
}
