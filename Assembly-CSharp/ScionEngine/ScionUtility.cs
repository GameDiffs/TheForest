using System;

namespace ScionEngine
{
	public static class ScionUtility
	{
		public const float DefaultWhitePoint = 5f;

		public static float CoCToPixels(float widthInPixels)
		{
			return widthInPixels / 0.07f;
		}

		public static float ComputeApertureDiameter(float fNumber, float focalLength)
		{
			return focalLength / fNumber;
		}

		public static float Square(float val)
		{
			return val * val;
		}

		public static float GetFocalLength(float tanHalfFoV)
		{
			return 35f / tanHalfFoV * 0.001f;
		}
	}
}
