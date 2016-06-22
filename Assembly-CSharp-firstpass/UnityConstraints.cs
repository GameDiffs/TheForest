using System;
using UnityEngine;

[AddComponentMenu("")]
public static class UnityConstraints
{
	public enum MODE_OPTIONS
	{
		Align,
		Constrain
	}

	public enum NO_TARGET_OPTIONS
	{
		Error,
		DoNothing,
		ReturnToDefault,
		SetByScript
	}

	public enum INTERP_OPTIONS
	{
		Linear,
		Spherical,
		SphericalLimited
	}

	public enum OUTPUT_ROT_OPTIONS
	{
		WorldAll,
		WorldX,
		WorldY,
		WorldZ,
		LocalX,
		LocalY,
		LocalZ
	}

	private static float lastRealtimeSinceStartup;

	public static void InterpolateRotationTo(Transform xform, Quaternion targetRot, UnityConstraints.INTERP_OPTIONS interpolation, float speed)
	{
		Quaternion rotation = xform.rotation;
		Quaternion rotation2 = Quaternion.identity;
		float deltaTime = Time.deltaTime;
		switch (interpolation)
		{
		case UnityConstraints.INTERP_OPTIONS.Linear:
			rotation2 = Quaternion.Lerp(rotation, targetRot, deltaTime * speed);
			break;
		case UnityConstraints.INTERP_OPTIONS.Spherical:
			rotation2 = Quaternion.Slerp(rotation, targetRot, deltaTime * speed);
			break;
		case UnityConstraints.INTERP_OPTIONS.SphericalLimited:
			rotation2 = Quaternion.RotateTowards(rotation, targetRot, speed * Time.timeScale);
			break;
		}
		xform.rotation = rotation2;
	}

	public static void MaskOutputRotations(Transform xform, UnityConstraints.OUTPUT_ROT_OPTIONS option)
	{
		switch (option)
		{
		case UnityConstraints.OUTPUT_ROT_OPTIONS.WorldX:
		{
			Vector3 vector = xform.eulerAngles;
			vector.y = 0f;
			vector.z = 0f;
			xform.eulerAngles = vector;
			break;
		}
		case UnityConstraints.OUTPUT_ROT_OPTIONS.WorldY:
		{
			Vector3 vector = xform.eulerAngles;
			vector.x = 0f;
			vector.z = 0f;
			xform.eulerAngles = vector;
			break;
		}
		case UnityConstraints.OUTPUT_ROT_OPTIONS.WorldZ:
		{
			Vector3 vector = xform.eulerAngles;
			vector.x = 0f;
			vector.y = 0f;
			xform.eulerAngles = vector;
			break;
		}
		case UnityConstraints.OUTPUT_ROT_OPTIONS.LocalX:
		{
			Vector3 vector = xform.localEulerAngles;
			vector.y = 0f;
			vector.z = 0f;
			xform.localEulerAngles = vector;
			break;
		}
		case UnityConstraints.OUTPUT_ROT_OPTIONS.LocalY:
		{
			Vector3 vector = xform.localEulerAngles;
			vector.x = 0f;
			vector.z = 0f;
			xform.localEulerAngles = vector;
			break;
		}
		case UnityConstraints.OUTPUT_ROT_OPTIONS.LocalZ:
		{
			Vector3 vector = xform.localEulerAngles;
			vector.x = 0f;
			vector.y = 0f;
			xform.localEulerAngles = vector;
			break;
		}
		}
	}
}
