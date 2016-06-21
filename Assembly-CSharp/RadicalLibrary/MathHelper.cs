using System;

namespace RadicalLibrary
{
	public static class MathHelper
	{
		public const float Pi = 3.14159274f;

		public const float HalfPi = 1.57079637f;

		public static float Lerp(double from, double to, double step)
		{
			return (float)((to - from) * step + from);
		}
	}
}
