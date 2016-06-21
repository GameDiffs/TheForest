using System;
using UnityEngine;

namespace OrbCreationExtensions
{
	public static class Vector2Extensions
	{
		public static string MakeString(this Vector2 v)
		{
			return string.Concat(new object[]
			{
				"<",
				v.x,
				",",
				v.y,
				">"
			});
		}

		public static string MakeString(this Vector2 v, int decimals)
		{
			if (decimals <= 0)
			{
				return string.Concat(new object[]
				{
					"<",
					Mathf.RoundToInt(v.x),
					",",
					Mathf.RoundToInt(v.y),
					">"
				});
			}
			string format = "{0:F" + decimals + "}";
			return string.Concat(new string[]
			{
				"<",
				string.Format(format, v.x),
				",",
				string.Format(format, v.y),
				">"
			});
		}

		public static Vector2 Product(this Vector2 v1, Vector2 v2)
		{
			return new Vector2(v1.x * v2.x, v1.y * v2.y);
		}

		public static float InProduct(this Vector2 v1, Vector2 v2)
		{
			return v1.x * v2.x + v1.y * v2.y;
		}

		public static Vector2 Abs(this Vector2 v)
		{
			return new Vector2(Mathf.Abs(v.x), Mathf.Abs(v.y));
		}

		public static bool IsEqual(this Vector2 v1, Vector2 v2)
		{
			return v1.x == v2.x && v1.y == v2.y;
		}

		public static bool DiffBetween(this Vector2 v1, Vector2 v2, float from, float to)
		{
			return v1.x - v2.x < to && v1.x - v2.x > from && v1.y - v2.y < to && v1.y - v2.y > from;
		}

		public static bool IsDiffSmallEnough(this Vector2 v1, Vector2 v2, float maxDiff)
		{
			return v1.x - v2.x < maxDiff && v2.x - v1.x < maxDiff && v1.y - v2.y < maxDiff && v2.y - v1.y < maxDiff;
		}

		public static bool IsAllSmaller(this Vector2 v1, Vector2 v2)
		{
			return v1.x < v2.x && v1.y < v2.y;
		}

		public static bool IsBarycentricInTriangle(this Vector2 v)
		{
			return v.x >= 0f && v.y >= 0f && v.x + v.y <= 1.01f;
		}
	}
}
