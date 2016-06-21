using System;
using UnityEngine;

namespace RadicalLibrary
{
	public static class CubicBez
	{
		public static Vector3 Interp(Vector3 st, Vector3 en, Vector3 ctrl1, Vector3 ctrl2, float t)
		{
			float num = 1f - t;
			return num * num * num * st + 3f * num * num * t * ctrl1 + 3f * num * t * t * ctrl2 + t * t * t * en;
		}

		public static Vector3 Velocity(Vector3 st, Vector3 en, Vector3 ctrl1, Vector3 ctrl2, float t)
		{
			return (-3f * st + 9f * ctrl1 - 9f * ctrl2 + 3f * en) * t * t + (6f * st - 12f * ctrl1 + 6f * ctrl2) * t - 3f * st + 3f * ctrl1;
		}

		public static void GizmoDraw(Vector3 st, Vector3 en, Vector3 ctrl1, Vector3 ctrl2, float t)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(st, ctrl1);
			Gizmos.DrawLine(ctrl2, en);
			Gizmos.color = Color.white;
			Vector3 to = st;
			for (int i = 1; i <= 20; i++)
			{
				float t2 = (float)i / 20f;
				Vector3 vector = CubicBez.Interp(st, en, ctrl1, ctrl2, t2);
				Gizmos.DrawLine(vector, to);
				to = vector;
			}
			Gizmos.color = Color.blue;
			Vector3 vector2 = CubicBez.Interp(st, en, ctrl1, ctrl2, t);
			Gizmos.DrawLine(vector2, vector2 + CubicBez.Velocity(st, en, ctrl1, ctrl2, t));
		}
	}
}
