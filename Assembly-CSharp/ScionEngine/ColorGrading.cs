using System;
using UnityEngine;

namespace ScionEngine
{
	public class ColorGrading
	{
		public Texture2D Convert(Texture2D lut2D, ColorGradingCompatibility compatibilityMode)
		{
			return null;
		}

		private Texture2D ConvertUnity(Texture2D lut2D)
		{
			int height = lut2D.height;
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < height; j++)
				{
					for (int k = 0; k < height; k++)
					{
					}
				}
			}
			return null;
		}
	}
}
