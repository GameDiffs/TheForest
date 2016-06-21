using System;
using UnityEngine;

namespace uSky
{
	[Serializable]
	public class uSkyAmbient
	{
		[Tooltip("Ambient lighting coming from above.")]
		public Gradient SkyColor = new Gradient
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(new Color32(28, 32, 40, 255), 0.225f),
				new GradientColorKey(new Color32(55, 65, 63, 255), 0.25f),
				new GradientColorKey(new Color32(148, 179, 219, 255), 0.28f),
				new GradientColorKey(new Color32(148, 179, 219, 255), 0.72f),
				new GradientColorKey(new Color32(55, 65, 63, 255), 0.75f),
				new GradientColorKey(new Color32(28, 32, 40, 255), 0.775f)
			},
			alphaKeys = new GradientAlphaKey[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			}
		};

		[Tooltip("Ambient lighting coming from side.")]
		public Gradient EquatorColor = new Gradient
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(new Color32(20, 25, 36, 255), 0.225f),
				new GradientColorKey(new Color32(80, 70, 50, 255), 0.25f),
				new GradientColorKey(new Color32(102, 138, 168, 255), 0.28f),
				new GradientColorKey(new Color32(102, 138, 168, 255), 0.72f),
				new GradientColorKey(new Color32(80, 70, 50, 255), 0.75f),
				new GradientColorKey(new Color32(20, 25, 36, 255), 0.775f)
			},
			alphaKeys = new GradientAlphaKey[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			}
		};

		[Tooltip("Ambient lighting coming from below.")]
		public Gradient GroundColor = new Gradient
		{
			colorKeys = new GradientColorKey[]
			{
				new GradientColorKey(new Color32(20, 20, 20, 255), 0.24f),
				new GradientColorKey(new Color32(51, 51, 51, 255), 0.27f),
				new GradientColorKey(new Color32(51, 51, 51, 255), 0.73f),
				new GradientColorKey(new Color32(20, 20, 20, 255), 0.76f)
			},
			alphaKeys = new GradientAlphaKey[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			}
		};
	}
}
