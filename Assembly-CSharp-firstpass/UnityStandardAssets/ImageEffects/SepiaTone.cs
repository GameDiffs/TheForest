using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[AddComponentMenu("Image Effects/Color Adjustments/Sepia Tone"), ExecuteInEditMode]
	public class SepiaTone : ImageEffectBase
	{
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit(source, destination, base.material);
		}
	}
}
