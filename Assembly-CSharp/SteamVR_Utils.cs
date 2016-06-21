using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using Valve.VR;

public static class SteamVR_Utils
{
	public class Event
	{
		public delegate void Handler(params object[] args);

		private static Hashtable listeners = new Hashtable();

		public static void Listen(string message, SteamVR_Utils.Event.Handler action)
		{
			SteamVR_Utils.Event.Handler handler = SteamVR_Utils.Event.listeners[message] as SteamVR_Utils.Event.Handler;
			if (handler != null)
			{
				SteamVR_Utils.Event.listeners[message] = (SteamVR_Utils.Event.Handler)Delegate.Combine(handler, action);
			}
			else
			{
				SteamVR_Utils.Event.listeners[message] = action;
			}
		}

		public static void Remove(string message, SteamVR_Utils.Event.Handler action)
		{
			SteamVR_Utils.Event.Handler handler = SteamVR_Utils.Event.listeners[message] as SteamVR_Utils.Event.Handler;
			if (handler != null)
			{
				SteamVR_Utils.Event.listeners[message] = (SteamVR_Utils.Event.Handler)Delegate.Remove(handler, action);
			}
		}

		public static void Send(string message, params object[] args)
		{
			SteamVR_Utils.Event.Handler handler = SteamVR_Utils.Event.listeners[message] as SteamVR_Utils.Event.Handler;
			if (handler != null)
			{
				handler(args);
			}
		}
	}

	[Serializable]
	public struct RigidTransform
	{
		public Vector3 pos;

		public Quaternion rot;

		public static SteamVR_Utils.RigidTransform identity
		{
			get
			{
				return new SteamVR_Utils.RigidTransform(Vector3.zero, Quaternion.identity);
			}
		}

		public RigidTransform(Vector3 pos, Quaternion rot)
		{
			this.pos = pos;
			this.rot = rot;
		}

		public RigidTransform(Transform t)
		{
			this.pos = t.position;
			this.rot = t.rotation;
		}

		public RigidTransform(Transform from, Transform to)
		{
			Quaternion quaternion = Quaternion.Inverse(from.rotation);
			this.rot = quaternion * to.rotation;
			this.pos = quaternion * (to.position - from.position);
		}

		public RigidTransform(HmdMatrix34_t pose)
		{
			Matrix4x4 identity = Matrix4x4.identity;
			identity[0, 0] = pose.m0;
			identity[0, 1] = pose.m1;
			identity[0, 2] = -pose.m2;
			identity[0, 3] = pose.m3;
			identity[1, 0] = pose.m4;
			identity[1, 1] = pose.m5;
			identity[1, 2] = -pose.m6;
			identity[1, 3] = pose.m7;
			identity[2, 0] = -pose.m8;
			identity[2, 1] = -pose.m9;
			identity[2, 2] = pose.m10;
			identity[2, 3] = -pose.m11;
			this.pos = identity.GetPosition();
			this.rot = identity.GetRotation();
		}

		public RigidTransform(HmdMatrix44_t pose)
		{
			Matrix4x4 identity = Matrix4x4.identity;
			identity[0, 0] = pose.m0;
			identity[0, 1] = pose.m1;
			identity[0, 2] = -pose.m2;
			identity[0, 3] = pose.m3;
			identity[1, 0] = pose.m4;
			identity[1, 1] = pose.m5;
			identity[1, 2] = -pose.m6;
			identity[1, 3] = pose.m7;
			identity[2, 0] = -pose.m8;
			identity[2, 1] = -pose.m9;
			identity[2, 2] = pose.m10;
			identity[2, 3] = -pose.m11;
			identity[3, 0] = pose.m12;
			identity[3, 1] = pose.m13;
			identity[3, 2] = -pose.m14;
			identity[3, 3] = pose.m15;
			this.pos = identity.GetPosition();
			this.rot = identity.GetRotation();
		}

		public static SteamVR_Utils.RigidTransform FromLocal(Transform t)
		{
			return new SteamVR_Utils.RigidTransform(t.localPosition, t.localRotation);
		}

		public HmdMatrix44_t ToHmdMatrix44()
		{
			Matrix4x4 matrix4x = Matrix4x4.TRS(this.pos, this.rot, Vector3.one);
			return new HmdMatrix44_t
			{
				m0 = matrix4x[0, 0],
				m1 = matrix4x[0, 1],
				m2 = -matrix4x[0, 2],
				m3 = matrix4x[0, 3],
				m4 = matrix4x[1, 0],
				m5 = matrix4x[1, 1],
				m6 = -matrix4x[1, 2],
				m7 = matrix4x[1, 3],
				m8 = -matrix4x[2, 0],
				m9 = -matrix4x[2, 1],
				m10 = matrix4x[2, 2],
				m11 = -matrix4x[2, 3],
				m12 = matrix4x[3, 0],
				m13 = matrix4x[3, 1],
				m14 = -matrix4x[3, 2],
				m15 = matrix4x[3, 3]
			};
		}

		public HmdMatrix34_t ToHmdMatrix34()
		{
			Matrix4x4 matrix4x = Matrix4x4.TRS(this.pos, this.rot, Vector3.one);
			return new HmdMatrix34_t
			{
				m0 = matrix4x[0, 0],
				m1 = matrix4x[0, 1],
				m2 = -matrix4x[0, 2],
				m3 = matrix4x[0, 3],
				m4 = matrix4x[1, 0],
				m5 = matrix4x[1, 1],
				m6 = -matrix4x[1, 2],
				m7 = matrix4x[1, 3],
				m8 = -matrix4x[2, 0],
				m9 = -matrix4x[2, 1],
				m10 = matrix4x[2, 2],
				m11 = -matrix4x[2, 3]
			};
		}

		public override bool Equals(object o)
		{
			if (o is SteamVR_Utils.RigidTransform)
			{
				SteamVR_Utils.RigidTransform rigidTransform = (SteamVR_Utils.RigidTransform)o;
				return this.pos == rigidTransform.pos && this.rot == rigidTransform.rot;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.pos.GetHashCode() ^ this.rot.GetHashCode();
		}

		public void Inverse()
		{
			this.rot = Quaternion.Inverse(this.rot);
			this.pos = -(this.rot * this.pos);
		}

		public SteamVR_Utils.RigidTransform GetInverse()
		{
			SteamVR_Utils.RigidTransform result = new SteamVR_Utils.RigidTransform(this.pos, this.rot);
			result.Inverse();
			return result;
		}

		public void Multiply(SteamVR_Utils.RigidTransform a, SteamVR_Utils.RigidTransform b)
		{
			this.rot = a.rot * b.rot;
			this.pos = a.pos + a.rot * b.pos;
		}

		public Vector3 InverseTransformPoint(Vector3 point)
		{
			return Quaternion.Inverse(this.rot) * (point - this.pos);
		}

		public Vector3 TransformPoint(Vector3 point)
		{
			return this.pos + this.rot * point;
		}

		public static SteamVR_Utils.RigidTransform Interpolate(SteamVR_Utils.RigidTransform a, SteamVR_Utils.RigidTransform b, float t)
		{
			return new SteamVR_Utils.RigidTransform(Vector3.Lerp(a.pos, b.pos, t), Quaternion.Slerp(a.rot, b.rot, t));
		}

		public void Interpolate(SteamVR_Utils.RigidTransform to, float t)
		{
			this.pos = SteamVR_Utils.Lerp(this.pos, to.pos, t);
			this.rot = SteamVR_Utils.Slerp(this.rot, to.rot, t);
		}

		public static bool operator ==(SteamVR_Utils.RigidTransform a, SteamVR_Utils.RigidTransform b)
		{
			return a.pos == b.pos && a.rot == b.rot;
		}

		public static bool operator !=(SteamVR_Utils.RigidTransform a, SteamVR_Utils.RigidTransform b)
		{
			return a.pos != b.pos || a.rot != b.rot;
		}

		public static SteamVR_Utils.RigidTransform operator *(SteamVR_Utils.RigidTransform a, SteamVR_Utils.RigidTransform b)
		{
			return new SteamVR_Utils.RigidTransform
			{
				rot = a.rot * b.rot,
				pos = a.pos + a.rot * b.pos
			};
		}

		public static Vector3 operator *(SteamVR_Utils.RigidTransform t, Vector3 v)
		{
			return t.TransformPoint(v);
		}
	}

	public delegate object SystemFn(CVRSystem system, params object[] args);

	public static Quaternion Slerp(Quaternion A, Quaternion B, float t)
	{
		float num = Mathf.Clamp(A.x * B.x + A.y * B.y + A.z * B.z + A.w * B.w, -1f, 1f);
		if (num < 0f)
		{
			B = new Quaternion(-B.x, -B.y, -B.z, -B.w);
			num = -num;
		}
		float num4;
		float num5;
		if (1f - num > 0.0001f)
		{
			float num2 = Mathf.Acos(num);
			float num3 = Mathf.Sin(num2);
			num4 = Mathf.Sin((1f - t) * num2) / num3;
			num5 = Mathf.Sin(t * num2) / num3;
		}
		else
		{
			num4 = 1f - t;
			num5 = t;
		}
		return new Quaternion(num4 * A.x + num5 * B.x, num4 * A.y + num5 * B.y, num4 * A.z + num5 * B.z, num4 * A.w + num5 * B.w);
	}

	public static Vector3 Lerp(Vector3 A, Vector3 B, float t)
	{
		return new Vector3(SteamVR_Utils.Lerp(A.x, B.x, t), SteamVR_Utils.Lerp(A.y, B.y, t), SteamVR_Utils.Lerp(A.z, B.z, t));
	}

	public static float Lerp(float A, float B, float t)
	{
		return A + (B - A) * t;
	}

	public static double Lerp(double A, double B, double t)
	{
		return A + (B - A) * t;
	}

	public static float InverseLerp(Vector3 A, Vector3 B, Vector3 result)
	{
		return Vector3.Dot(result - A, B - A);
	}

	public static float InverseLerp(float A, float B, float result)
	{
		return (result - A) / (B - A);
	}

	public static double InverseLerp(double A, double B, double result)
	{
		return (result - A) / (B - A);
	}

	public static float Saturate(float A)
	{
		return (A >= 0f) ? ((A <= 1f) ? A : 1f) : 0f;
	}

	public static Vector2 Saturate(Vector2 A)
	{
		return new Vector2(SteamVR_Utils.Saturate(A.x), SteamVR_Utils.Saturate(A.y));
	}

	public static float Abs(float A)
	{
		return (A >= 0f) ? A : (-A);
	}

	public static Vector2 Abs(Vector2 A)
	{
		return new Vector2(SteamVR_Utils.Abs(A.x), SteamVR_Utils.Abs(A.y));
	}

	private static float _copysign(float sizeval, float signval)
	{
		return (Mathf.Sign(signval) != 1f) ? (-Mathf.Abs(sizeval)) : Mathf.Abs(sizeval);
	}

	public static Quaternion GetRotation(this Matrix4x4 matrix)
	{
		Quaternion result = default(Quaternion);
		result.w = Mathf.Sqrt(Mathf.Max(0f, 1f + matrix.m00 + matrix.m11 + matrix.m22)) / 2f;
		result.x = Mathf.Sqrt(Mathf.Max(0f, 1f + matrix.m00 - matrix.m11 - matrix.m22)) / 2f;
		result.y = Mathf.Sqrt(Mathf.Max(0f, 1f - matrix.m00 + matrix.m11 - matrix.m22)) / 2f;
		result.z = Mathf.Sqrt(Mathf.Max(0f, 1f - matrix.m00 - matrix.m11 + matrix.m22)) / 2f;
		result.x = SteamVR_Utils._copysign(result.x, matrix.m21 - matrix.m12);
		result.y = SteamVR_Utils._copysign(result.y, matrix.m02 - matrix.m20);
		result.z = SteamVR_Utils._copysign(result.z, matrix.m10 - matrix.m01);
		return result;
	}

	public static Vector3 GetPosition(this Matrix4x4 matrix)
	{
		float m = matrix.m03;
		float m2 = matrix.m13;
		float m3 = matrix.m23;
		return new Vector3(m, m2, m3);
	}

	public static Vector3 GetScale(this Matrix4x4 m)
	{
		float x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
		float y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
		float z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);
		return new Vector3(x, y, z);
	}

	public static Mesh CreateHiddenAreaMesh(HiddenAreaMesh_t src, VRTextureBounds_t bounds)
	{
		if (src.unTriangleCount == 0u)
		{
			return null;
		}
		float[] array = new float[src.unTriangleCount * 3u * 2u];
		Marshal.Copy(src.pVertexData, array, 0, array.Length);
		Vector3[] array2 = new Vector3[src.unTriangleCount * 3u + 12u];
		int[] array3 = new int[src.unTriangleCount * 3u + 24u];
		float num = 2f * bounds.uMin - 1f;
		float num2 = 2f * bounds.uMax - 1f;
		float num3 = 2f * bounds.vMin - 1f;
		float num4 = 2f * bounds.vMax - 1f;
		int num5 = 0;
		int num6 = 0;
		while ((long)num5 < (long)((ulong)(src.unTriangleCount * 3u)))
		{
			float x = SteamVR_Utils.Lerp(num, num2, array[num6++]);
			float y = SteamVR_Utils.Lerp(num3, num4, array[num6++]);
			array2[num5] = new Vector3(x, y, 0f);
			array3[num5] = num5;
			num5++;
		}
		int num7 = (int)(src.unTriangleCount * 3u);
		int num8 = num7;
		array2[num8++] = new Vector3(-1f, -1f, 0f);
		array2[num8++] = new Vector3(num, -1f, 0f);
		array2[num8++] = new Vector3(-1f, 1f, 0f);
		array2[num8++] = new Vector3(num, 1f, 0f);
		array2[num8++] = new Vector3(num2, -1f, 0f);
		array2[num8++] = new Vector3(1f, -1f, 0f);
		array2[num8++] = new Vector3(num2, 1f, 0f);
		array2[num8++] = new Vector3(1f, 1f, 0f);
		array2[num8++] = new Vector3(num, num3, 0f);
		array2[num8++] = new Vector3(num2, num3, 0f);
		array2[num8++] = new Vector3(num, num4, 0f);
		array2[num8++] = new Vector3(num2, num4, 0f);
		int num9 = num7;
		array3[num9++] = num7;
		array3[num9++] = num7 + 1;
		array3[num9++] = num7 + 2;
		array3[num9++] = num7 + 2;
		array3[num9++] = num7 + 1;
		array3[num9++] = num7 + 3;
		array3[num9++] = num7 + 4;
		array3[num9++] = num7 + 5;
		array3[num9++] = num7 + 6;
		array3[num9++] = num7 + 6;
		array3[num9++] = num7 + 5;
		array3[num9++] = num7 + 7;
		array3[num9++] = num7 + 1;
		array3[num9++] = num7 + 4;
		array3[num9++] = num7 + 8;
		array3[num9++] = num7 + 8;
		array3[num9++] = num7 + 4;
		array3[num9++] = num7 + 9;
		array3[num9++] = num7 + 10;
		array3[num9++] = num7 + 11;
		array3[num9++] = num7 + 3;
		array3[num9++] = num7 + 3;
		array3[num9++] = num7 + 11;
		array3[num9++] = num7 + 6;
		return new Mesh
		{
			vertices = array2,
			triangles = array3,
			bounds = new Bounds(Vector3.zero, new Vector3(3.40282347E+38f, 3.40282347E+38f, 3.40282347E+38f))
		};
	}

	public static object CallSystemFn(SteamVR_Utils.SystemFn fn, params object[] args)
	{
		bool flag = !SteamVR.active && !SteamVR.usingNativeSupport;
		if (flag)
		{
			EVRInitError eVRInitError = EVRInitError.None;
			OpenVR.Init(ref eVRInitError, EVRApplicationType.VRApplication_Other);
		}
		CVRSystem system = OpenVR.System;
		object result = (system == null) ? null : fn(system, args);
		if (flag)
		{
			OpenVR.Shutdown();
		}
		return result;
	}

	public static void QueueEventOnRenderThread(int eventID)
	{
		GL.IssuePluginEvent(eventID);
	}
}
