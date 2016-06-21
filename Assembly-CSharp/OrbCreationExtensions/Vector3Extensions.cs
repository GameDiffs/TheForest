using System;
using UnityEngine;

namespace OrbCreationExtensions
{
	public static class Vector3Extensions
	{
		public static string MakeString(this Vector3 v)
		{
			return string.Concat(new object[]
			{
				"<",
				v.x,
				",",
				v.y,
				",",
				v.z,
				">"
			});
		}

		public static string MakeString(this Vector3 v, int decimals)
		{
			if (decimals <= 0)
			{
				return string.Concat(new object[]
				{
					"<",
					Mathf.RoundToInt(v.x),
					",",
					Mathf.RoundToInt(v.y),
					",",
					Mathf.RoundToInt(v.z),
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
				",",
				string.Format(format, v.z),
				">"
			});
		}

		public static Vector3 Product(this Vector3 v1, Vector3 v2)
		{
			return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
		}

		public static float Sum(this Vector3 v1)
		{
			return v1.x + v1.y + v1.z;
		}

		public static float InProduct(this Vector3 v1, Vector3 v2)
		{
			return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
		}

		public static Vector3 Abs(this Vector3 v)
		{
			return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
		}

		public static Vector3 VectorMax(this Vector3 v1, Vector3 v2)
		{
			return new Vector3(Mathf.Max(v1.x, v2.x), Mathf.Max(v1.y, v2.y), Mathf.Max(v1.z, v2.z));
		}

		public static bool IsEqual(this Vector3 v1, Vector3 v2)
		{
			return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
		}

		public static bool DiffBetween(this Vector3 v1, Vector3 v2, float from, float to)
		{
			return v1.x - v2.x < to && v1.x - v2.x > from && v1.y - v2.y < to && v1.y - v2.y > from && v1.z - v2.z < to && v1.z - v2.z > from;
		}

		public static bool IsDiffSmallEnough(this Vector3 v1, Vector3 v2, float maxDiff)
		{
			return v1.x - v2.x < maxDiff && v2.x - v1.x < maxDiff && v1.y - v2.y < maxDiff && v2.y - v1.y < maxDiff && v1.z - v2.z < maxDiff && v2.z - v1.z < maxDiff;
		}

		public static bool IsAllSmaller(this Vector3 v1, Vector3 v2)
		{
			return v1.x < v2.x && v1.y < v2.y && v1.z < v2.z;
		}

		public static Vector2 Barycentric(this Vector3 p, Vector3 a, Vector3 b, Vector3 c)
		{
			Vector3 vector = c - a;
			Vector3 vector2 = b - a;
			Vector3 rhs = p - a;
			float num = Vector3.Dot(vector, vector);
			float num2 = Vector3.Dot(vector, vector2);
			float num3 = Vector3.Dot(vector, rhs);
			float num4 = Vector3.Dot(vector2, vector2);
			float num5 = Vector3.Dot(vector2, rhs);
			float num6 = 1f / (num * num4 - num2 * num2);
			float x = (num4 * num3 - num2 * num5) * num6;
			float y = (num * num5 - num2 * num3) * num6;
			return new Vector2(x, y);
		}

		public static Vector3 To180Angle(this Vector3 v)
		{
			v.x = v.x.To180Angle();
			v.y = v.y.To180Angle();
			v.z = v.z.To180Angle();
			return v;
		}

		public static Vector3 To360Angle(this Vector3 v)
		{
			v.x = v.x.To360Angle();
			v.y = v.y.To360Angle();
			v.z = v.z.To360Angle();
			return v;
		}
	}
}
