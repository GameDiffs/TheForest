using System;
using UnityEngine;

namespace ScionEngine
{
	public struct ColorGradingParameters
	{
		public ColorGradingMode colorGradingMode;

		public ColorGradingCompatibility colorGradingCompatibility;

		public Texture2D colorGradingTex1;

		public Texture2D colorGradingTex2;

		public float colorGradingBlendFactor;
	}
}
