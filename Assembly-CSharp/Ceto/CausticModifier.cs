using System;
using UnityEngine;

namespace Ceto
{
	[Serializable]
	public struct CausticModifier
	{
		[Range(0f, 3f)]
		public float distortion;

		[Range(0f, 1f)]
		public float depthFade;

		[Range(0f, 3f)]
		public float intensity;

		public Color tint;

		public CausticModifier(float distortion, float depthFade, float intensity, Color tint)
		{
			this.distortion = distortion;
			this.depthFade = depthFade;
			this.intensity = intensity;
			this.tint = tint;
		}
	}
}
