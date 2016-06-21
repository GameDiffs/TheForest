using System;
using UnityEngine;

namespace OrbCreationExtensions
{
	public static class FloatExtensions
	{
		public static string MakeString(this float aFloat)
		{
			return string.Empty + aFloat;
		}

		public static string MakeString(this float aFloat, int decimals)
		{
			if (decimals <= 0)
			{
				return string.Empty + Mathf.RoundToInt(aFloat);
			}
			string format = "{0:F" + decimals + "}";
			return string.Format(format, aFloat);
		}

		public static float To180Angle(this float f)
		{
			while (f <= -180f)
			{
				f += 360f;
			}
			while (f > 180f)
			{
				f -= 360f;
			}
			return f;
		}

		public static float To360Angle(this float f)
		{
			while (f < 0f)
			{
				f += 360f;
			}
			while (f >= 360f)
			{
				f -= 360f;
			}
			return f;
		}

		public static float RadToCompassAngle(this float rad)
		{
			return (rad * 57.29578f).DegreesToCompassAngle();
		}

		public static float DegreesToCompassAngle(this float angle)
		{
			angle = 90f - angle;
			return angle.To360Angle();
		}

		public static float Distance(this float f1, float f2)
		{
			return Mathf.Abs(f1 - f2);
		}

		public static float RelativePositionBetweenAngles(this float angle, float from, float to)
		{
			from = from.To360Angle();
			to = to.To360Angle();
			if (from - to > 180f)
			{
				from -= 360f;
			}
			if (to - from > 180f)
			{
				to -= 360f;
			}
			angle = angle.To360Angle();
			if (from < to)
			{
				if (angle >= from && angle < to)
				{
					return (angle - from) / (to - from);
				}
				if (angle - 360f >= from && angle - 360f < to)
				{
					return (angle - 360f - from) / (to - from);
				}
			}
			if (from > to)
			{
				if (angle < from && angle >= to)
				{
					return (angle - to) / (from - to);
				}
				if (angle - 360f < from && angle - 360f >= to)
				{
					return (angle - 360f - to) / (from - to);
				}
			}
			return -1f;
		}

		public static float Round(this float f, int decimals)
		{
			float num = Mathf.Pow(10f, (float)decimals);
			f = Mathf.Round(f * num);
			return f / num;
		}
	}
}
