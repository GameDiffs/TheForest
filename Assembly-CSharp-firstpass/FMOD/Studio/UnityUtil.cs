using System;
using UnityEngine;

namespace FMOD.Studio
{
	public static class UnityUtil
	{
		public const float MINIMUM_DECIBEL_VALUE = -80f;

		public static float LinearToDecibels(float linearGain)
		{
			if (linearGain <= 0f)
			{
				return -80f;
			}
			float a = Mathf.Log10(linearGain) * 20f;
			return Mathf.Max(a, -80f);
		}

		public static float DecibelsToLinear(float decibelGain)
		{
			if (decibelGain <= -80f)
			{
				return 0f;
			}
			return Mathf.Pow(10f, decibelGain / 20f);
		}

		public static VECTOR toFMODVector(this Vector3 vec)
		{
			VECTOR result;
			result.x = vec.x;
			result.y = vec.y;
			result.z = vec.z;
			return result;
		}

		public static Vector3 toUnityVector(this VECTOR vec)
		{
			Vector3 result;
			result.x = vec.x;
			result.y = vec.y;
			result.z = vec.z;
			return result;
		}

		public static ATTRIBUTES_3D to3DAttributes(this Vector3 pos)
		{
			return new ATTRIBUTES_3D
			{
				forward = Vector3.forward.toFMODVector(),
				up = Vector3.up.toFMODVector(),
				position = pos.toFMODVector()
			};
		}

		public static ATTRIBUTES_3D to3DAttributes(this Transform transform)
		{
			return new ATTRIBUTES_3D
			{
				forward = transform.forward.toFMODVector(),
				up = transform.up.toFMODVector(),
				position = transform.position.toFMODVector()
			};
		}

		public static ATTRIBUTES_3D to3DAttributes(GameObject go, Rigidbody rigidbody = null)
		{
			ATTRIBUTES_3D result = go.transform.to3DAttributes();
			if (rigidbody)
			{
				result.velocity = rigidbody.velocity.toFMODVector();
			}
			return result;
		}

		public static bool isPlaying(this PLAYBACK_STATE state)
		{
			return state == PLAYBACK_STATE.PLAYING || state == PLAYBACK_STATE.STARTING;
		}

		public static void Log(string msg)
		{
		}

		public static void LogWarning(string msg)
		{
			UnityEngine.Debug.LogWarning(msg);
		}

		public static void LogError(string msg)
		{
			UnityEngine.Debug.LogError(msg);
		}

		public static bool ForceLoadLowLevelBinary()
		{
			UnityUtil.Log("Attempting to call Memory_GetStats");
			int num;
			int num2;
			if (!UnityUtil.ERRCHECK(Memory.GetStats(out num, out num2)))
			{
				UnityUtil.LogError("Memory_GetStats returned an error");
				return false;
			}
			UnityUtil.Log("Calling Memory_GetStats succeeded!");
			return true;
		}

		public static bool ERRCHECK(RESULT result)
		{
			if (result != RESULT.OK)
			{
				UnityUtil.LogWarning("FMOD Error (" + result.ToString() + "): " + Error.String(result));
			}
			return result == RESULT.OK;
		}
	}
}
