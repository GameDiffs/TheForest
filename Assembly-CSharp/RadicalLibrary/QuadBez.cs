using System;
using UnityEngine;

namespace RadicalLibrary
{
	public static class QuadBez
	{
		public static Vector3 Interp(Vector3 st, Vector3 en, Vector3 ctrl, float t)
		{
			float num = 1f - t;
			return num * num * st + 2f * num * t * ctrl + t * t * en;
		}

		public static Vector3 Velocity(Vector3 st, Vector3 en, Vector3 ctrl, float t)
		{
			return (2f * st - 4f * ctrl + 2f * en) * t + 2f * ctrl - 2f * st;
		}

		public static void GizmoDraw(Vector3 st, Vector3 en, Vector3 ctrl, float t)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(st, ctrl);
			Gizmos.DrawLine(ctrl, en);
			Gizmos.color = Color.white;
			Vector3 to = st;
			for (int i = 1; i <= 20; i++)
			{
				float t2 = (float)i / 20f;
				Vector3 vector = QuadBez.Interp(st, en, ctrl, t2);
				Gizmos.DrawLine(vector, to);
				to = vector;
			}
			Gizmos.color = Color.blue;
			Vector3 vector2 = QuadBez.Interp(st, en, ctrl, t);
			Gizmos.DrawLine(vector2, vector2 + QuadBez.Velocity(st, en, ctrl, t));
		}
	}
}
