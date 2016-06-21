using System;
using UniLinq;
using UnityEngine;

namespace RadicalLibrary
{
	public static class Spline
	{
		public class Path
		{
			private Vector3[] _path;

			public Vector3[] path
			{
				get
				{
					return this._path;
				}
				set
				{
					this._path = value;
				}
			}

			public int Length
			{
				get
				{
					return (this.path == null) ? 0 : this.path.Length;
				}
			}

			public Vector3 this[int index]
			{
				get
				{
					return this.path[index];
				}
			}

			public static implicit operator Spline.Path(Vector3[] path)
			{
				return new Spline.Path
				{
					path = path
				};
			}

			public static implicit operator Vector3[](Spline.Path p)
			{
				return (p == null) ? new Vector3[0] : p.path;
			}

			public static implicit operator Spline.Path(Transform[] path)
			{
				Spline.Path path2 = new Spline.Path();
				path2.path = (from p in path
				select p.position).ToArray<Vector3>();
				return path2;
			}

			public static implicit operator Spline.Path(GameObject[] path)
			{
				Spline.Path path2 = new Spline.Path();
				path2.path = (from p in path
				select p.transform.position).ToArray<Vector3>();
				return path2;
			}
		}

		public static Vector3 Interp(Spline.Path pts, float t)
		{
			return Spline.Interp(pts, t, EasingType.Linear);
		}

		public static Vector3 Interp(Spline.Path pts, float t, EasingType ease)
		{
			return Spline.Interp(pts, t, ease, true);
		}

		public static Vector3 Interp(Spline.Path pts, float t, EasingType ease, bool easeIn)
		{
			return Spline.Interp(pts, t, ease, easeIn, true);
		}

		public static Vector3 Interp(Spline.Path pts, float t, EasingType ease, bool easeIn, bool easeOut)
		{
			t = Spline.Ease(t, ease, easeIn, easeOut);
			if (pts.Length == 0)
			{
				return Vector3.zero;
			}
			if (pts.Length == 1)
			{
				return pts[0];
			}
			if (pts.Length == 2)
			{
				return Vector3.Lerp(pts[0], pts[1], t);
			}
			if (pts.Length == 3)
			{
				return QuadBez.Interp(pts[0], pts[2], pts[1], t);
			}
			if (pts.Length == 4)
			{
				return CubicBez.Interp(pts[0], pts[3], pts[1], pts[2], t);
			}
			return CRSpline.Interp(Spline.Wrap(pts), t);
		}

		private static float Ease(float t)
		{
			return Spline.Ease(t, EasingType.Linear);
		}

		private static float Ease(float t, EasingType ease)
		{
			return Spline.Ease(t, ease, true);
		}

		private static float Ease(float t, EasingType ease, bool easeIn)
		{
			return Spline.Ease(t, ease, easeIn, true);
		}

		private static float Ease(float t, EasingType ease, bool easeIn, bool easeOut)
		{
			t = Mathf.Clamp01(t);
			if (easeIn && easeOut)
			{
				t = Easing.EaseInOut((double)t, ease);
			}
			else if (easeIn)
			{
				t = Easing.EaseIn((double)t, ease);
			}
			else if (easeOut)
			{
				t = Easing.EaseOut((double)t, ease);
			}
			return t;
		}

		public static Vector3 InterpConstantSpeed(Spline.Path pts, float t)
		{
			return Spline.InterpConstantSpeed(pts, t, EasingType.Linear);
		}

		public static Vector3 InterpConstantSpeed(Spline.Path pts, float t, EasingType ease)
		{
			return Spline.InterpConstantSpeed(pts, t, ease, true);
		}

		public static Vector3 InterpConstantSpeed(Spline.Path pts, float t, EasingType ease, bool easeIn)
		{
			return Spline.InterpConstantSpeed(pts, t, ease, easeIn, true);
		}

		public static Vector3 InterpConstantSpeed(Spline.Path pts, float t, EasingType ease, bool easeIn, bool easeOut)
		{
			t = Spline.Ease(t, ease, easeIn, easeOut);
			if (pts.Length == 0)
			{
				return Vector3.zero;
			}
			if (pts.Length == 1)
			{
				return pts[0];
			}
			if (pts.Length == 2)
			{
				return Vector3.Lerp(pts[0], pts[1], t);
			}
			if (pts.Length == 3)
			{
				return QuadBez.Interp(pts[0], pts[2], pts[1], t);
			}
			if (pts.Length == 4)
			{
				return CubicBez.Interp(pts[0], pts[3], pts[1], pts[2], t);
			}
			return CRSpline.InterpConstantSpeed(Spline.Wrap(pts), t);
		}

		private static float Clamp(float f)
		{
			return Mathf.Clamp01(f);
		}

		public static Vector3 MoveOnPath(Spline.Path pts, Vector3 currentPosition, ref float pathPosition)
		{
			return Spline.MoveOnPath(pts, currentPosition, ref pathPosition, 1f);
		}

		public static Vector3 MoveOnPath(Spline.Path pts, Vector3 currentPosition, ref float pathPosition, float maxSpeed)
		{
			return Spline.MoveOnPath(pts, currentPosition, ref pathPosition, maxSpeed, 100f);
		}

		public static Vector3 MoveOnPath(Spline.Path pts, Vector3 currentPosition, ref float pathPosition, float maxSpeed, float smoothnessFactor)
		{
			return Spline.MoveOnPath(pts, currentPosition, ref pathPosition, maxSpeed, smoothnessFactor, EasingType.Linear);
		}

		public static Vector3 MoveOnPath(Spline.Path pts, Vector3 currentPosition, ref float pathPosition, float maxSpeed, float smoothnessFactor, EasingType ease)
		{
			return Spline.MoveOnPath(pts, currentPosition, ref pathPosition, maxSpeed, smoothnessFactor, ease, true);
		}

		public static Vector3 MoveOnPath(Spline.Path pts, Vector3 currentPosition, ref float pathPosition, float maxSpeed, float smoothnessFactor, EasingType ease, bool easeIn)
		{
			return Spline.MoveOnPath(pts, currentPosition, ref pathPosition, maxSpeed, smoothnessFactor, ease, easeIn, true);
		}

		public static Vector3 MoveOnPath(Spline.Path pts, Vector3 currentPosition, ref float pathPosition, float maxSpeed, float smoothnessFactor, EasingType ease, bool easeIn, bool easeOut)
		{
			maxSpeed *= Time.deltaTime;
			pathPosition = Spline.Clamp(pathPosition);
			Vector3 vector = Spline.InterpConstantSpeed(pts, pathPosition, ease, easeIn, easeOut);
			float magnitude;
			while ((magnitude = (vector - currentPosition).magnitude) <= maxSpeed && pathPosition != 1f)
			{
				pathPosition = Spline.Clamp(pathPosition + 1f / smoothnessFactor);
				vector = Spline.InterpConstantSpeed(pts, pathPosition, ease, easeIn, easeOut);
			}
			if (magnitude != 0f)
			{
				currentPosition = Vector3.MoveTowards(currentPosition, vector, maxSpeed);
			}
			return currentPosition;
		}

		public static Vector3 MoveOnPath(Spline.Path pts, Vector3 currentPosition, ref float pathPosition, ref Quaternion rotation)
		{
			return Spline.MoveOnPath(pts, currentPosition, ref pathPosition, ref rotation, 1f);
		}

		public static Vector3 MoveOnPath(Spline.Path pts, Vector3 currentPosition, ref float pathPosition, ref Quaternion rotation, float maxSpeed)
		{
			return Spline.MoveOnPath(pts, currentPosition, ref pathPosition, ref rotation, maxSpeed, 100f);
		}

		public static Vector3 MoveOnPath(Spline.Path pts, Vector3 currentPosition, ref float pathPosition, ref Quaternion rotation, float maxSpeed, float smoothnessFactor)
		{
			return Spline.MoveOnPath(pts, currentPosition, ref pathPosition, ref rotation, maxSpeed, smoothnessFactor, EasingType.Linear);
		}

		public static Vector3 MoveOnPath(Spline.Path pts, Vector3 currentPosition, ref float pathPosition, ref Quaternion rotation, float maxSpeed, float smoothnessFactor, EasingType ease)
		{
			return Spline.MoveOnPath(pts, currentPosition, ref pathPosition, ref rotation, maxSpeed, smoothnessFactor, ease, true);
		}

		public static Vector3 MoveOnPath(Spline.Path pts, Vector3 currentPosition, ref float pathPosition, ref Quaternion rotation, float maxSpeed, float smoothnessFactor, EasingType ease, bool easeIn)
		{
			return Spline.MoveOnPath(pts, currentPosition, ref pathPosition, ref rotation, maxSpeed, smoothnessFactor, ease, easeIn, true);
		}

		public static Vector3 MoveOnPath(Spline.Path pts, Vector3 currentPosition, ref float pathPosition, ref Quaternion rotation, float maxSpeed, float smoothnessFactor, EasingType ease, bool easeIn, bool easeOut)
		{
			Vector3 vector = Spline.MoveOnPath(pts, currentPosition, ref pathPosition, maxSpeed, smoothnessFactor, ease, easeIn, easeOut);
			rotation = ((!vector.Equals(currentPosition)) ? Quaternion.LookRotation(vector - currentPosition) : Quaternion.identity);
			return vector;
		}

		public static Quaternion RotationBetween(Spline.Path pts, float t1, float t2)
		{
			return Spline.RotationBetween(pts, t1, t2, EasingType.Linear);
		}

		public static Quaternion RotationBetween(Spline.Path pts, float t1, float t2, EasingType ease)
		{
			return Spline.RotationBetween(pts, t1, t2, ease, true);
		}

		public static Quaternion RotationBetween(Spline.Path pts, float t1, float t2, EasingType ease, bool easeIn)
		{
			return Spline.RotationBetween(pts, t1, t2, ease, easeIn, true);
		}

		public static Quaternion RotationBetween(Spline.Path pts, float t1, float t2, EasingType ease, bool easeIn, bool easeOut)
		{
			return Quaternion.LookRotation(Spline.Interp(pts, t2, ease, easeIn, easeOut) - Spline.Interp(pts, t1, ease, easeIn, easeOut));
		}

		public static Vector3 Velocity(Spline.Path pts, float t)
		{
			return Spline.Velocity(pts, t, EasingType.Linear);
		}

		public static Vector3 Velocity(Spline.Path pts, float t, EasingType ease)
		{
			return Spline.Velocity(pts, t, ease, true);
		}

		public static Vector3 Velocity(Spline.Path pts, float t, EasingType ease, bool easeIn)
		{
			return Spline.Velocity(pts, t, ease, easeIn, true);
		}

		public static Vector3 Velocity(Spline.Path pts, float t, EasingType ease, bool easeIn, bool easeOut)
		{
			t = Spline.Ease(t);
			if (pts.Length == 0)
			{
				return Vector3.zero;
			}
			if (pts.Length == 1)
			{
				return pts[0];
			}
			if (pts.Length == 2)
			{
				return Vector3.Lerp(pts[0], pts[1], t);
			}
			if (pts.Length == 3)
			{
				return QuadBez.Velocity(pts[0], pts[2], pts[1], t);
			}
			if (pts.Length == 3)
			{
				return CubicBez.Velocity(pts[0], pts[3], pts[1], pts[2], t);
			}
			return CRSpline.Velocity(Spline.Wrap(pts), t);
		}

		public static Vector3[] Wrap(Vector3[] path)
		{
			return new Vector3[]
			{
				path[0]
			}.Concat(path).Concat(new Vector3[]
			{
				path[path.Length - 1]
			}).ToArray<Vector3>();
		}

		public static void GizmoDraw(Vector3[] pts, float t)
		{
			Spline.GizmoDraw(pts, t, EasingType.Linear);
		}

		public static void GizmoDraw(Vector3[] pts, float t, EasingType ease)
		{
			Spline.GizmoDraw(pts, t, ease, true);
		}

		public static void GizmoDraw(Vector3[] pts, float t, EasingType ease, bool easeIn)
		{
			Spline.GizmoDraw(pts, t, ease, easeIn, true);
		}

		public static void GizmoDraw(Vector3[] pts, float t, EasingType ease, bool easeIn, bool easeOut)
		{
			Gizmos.color = Color.white;
			Vector3 to = Spline.Interp(pts, 0f);
			for (int i = 1; i <= 20; i++)
			{
				float t2 = (float)i / 20f;
				Vector3 vector = Spline.Interp(pts, t2, ease, easeIn, easeOut);
				Gizmos.DrawLine(vector, to);
				to = vector;
			}
			Gizmos.color = Color.blue;
			Vector3 vector2 = Spline.Interp(pts, t, ease, easeIn, easeOut);
			Gizmos.DrawLine(vector2, vector2 + Spline.Velocity(pts, t, ease, easeIn, easeOut));
		}
	}
}
