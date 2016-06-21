using System;

namespace RadicalLibrary
{
	public static class Easing
	{
		private static class Sine
		{
			public static float EaseIn(double s)
			{
				return (float)Math.Sin(s * 1.5707963705062866 - 1.5707963705062866) + 1f;
			}

			public static float EaseOut(double s)
			{
				return (float)Math.Sin(s * 1.5707963705062866);
			}

			public static float EaseInOut(double s)
			{
				return (float)(Math.Sin(s * 3.1415927410125732 - 1.5707963705062866) + 1.0) / 2f;
			}
		}

		private static class Power
		{
			public static float EaseIn(double s, int power)
			{
				return (float)Math.Pow(s, (double)power);
			}

			public static float EaseOut(double s, int power)
			{
				int num = (power % 2 != 0) ? 1 : -1;
				return (float)((double)num * (Math.Pow(s - 1.0, (double)power) + (double)num));
			}

			public static float EaseInOut(double s, int power)
			{
				if (s < 0.5)
				{
					return Easing.Power.EaseIn(s * 2.0, power) / 2f;
				}
				return Easing.Power.EaseOut((s - 0.5) * 2.0, power) / 2f + 0.5f;
			}
		}

		public static float Ease(double linearStep, float acceleration, EasingType type)
		{
			float num = (acceleration <= 0f) ? ((acceleration >= 0f) ? ((float)linearStep) : Easing.EaseOut(linearStep, type)) : Easing.EaseIn(linearStep, type);
			return MathHelper.Lerp(linearStep, (double)num, (double)Math.Abs(acceleration));
		}

		public static float EaseIn(double linearStep, EasingType type)
		{
			switch (type)
			{
			case EasingType.Step:
				return (float)((linearStep >= 0.5) ? 1 : 0);
			case EasingType.Linear:
				return (float)linearStep;
			case EasingType.Sine:
				return Easing.Sine.EaseIn(linearStep);
			case EasingType.Quadratic:
				return Easing.Power.EaseIn(linearStep, 2);
			case EasingType.Cubic:
				return Easing.Power.EaseIn(linearStep, 3);
			case EasingType.Quartic:
				return Easing.Power.EaseIn(linearStep, 4);
			case EasingType.Quintic:
				return Easing.Power.EaseIn(linearStep, 5);
			default:
				throw new NotImplementedException();
			}
		}

		public static float EaseOut(double linearStep, EasingType type)
		{
			switch (type)
			{
			case EasingType.Step:
				return (float)((linearStep >= 0.5) ? 1 : 0);
			case EasingType.Linear:
				return (float)linearStep;
			case EasingType.Sine:
				return Easing.Sine.EaseOut(linearStep);
			case EasingType.Quadratic:
				return Easing.Power.EaseOut(linearStep, 2);
			case EasingType.Cubic:
				return Easing.Power.EaseOut(linearStep, 3);
			case EasingType.Quartic:
				return Easing.Power.EaseOut(linearStep, 4);
			case EasingType.Quintic:
				return Easing.Power.EaseOut(linearStep, 5);
			default:
				throw new NotImplementedException();
			}
		}

		public static float EaseInOut(double linearStep, EasingType easeInType, EasingType easeOutType)
		{
			return (linearStep >= 0.5) ? Easing.EaseInOut(linearStep, easeOutType) : Easing.EaseInOut(linearStep, easeInType);
		}

		public static float EaseInOut(double linearStep, EasingType type)
		{
			switch (type)
			{
			case EasingType.Step:
				return (float)((linearStep >= 0.5) ? 1 : 0);
			case EasingType.Linear:
				return (float)linearStep;
			case EasingType.Sine:
				return Easing.Sine.EaseInOut(linearStep);
			case EasingType.Quadratic:
				return Easing.Power.EaseInOut(linearStep, 2);
			case EasingType.Cubic:
				return Easing.Power.EaseInOut(linearStep, 3);
			case EasingType.Quartic:
				return Easing.Power.EaseInOut(linearStep, 4);
			case EasingType.Quintic:
				return Easing.Power.EaseInOut(linearStep, 5);
			default:
				throw new NotImplementedException();
			}
		}
	}
}
