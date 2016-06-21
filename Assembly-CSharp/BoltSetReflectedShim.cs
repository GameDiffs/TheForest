using System;
using UnityEngine;

public static class BoltSetReflectedShim
{
	public const float FLOAT_MIN_DELTA = 0.001f;

	public static void SetFloatReflected(this Animator animator, string name, float value)
	{
		animator.SetFloat(name, value);
	}

	public static void SetFloatReflected(this Animator animator, string name, float value, float dampTime, float deltaTime)
	{
		animator.SetFloat(name, value, dampTime, deltaTime);
	}

	public static void SetFloatReflected(this Animator animator, int name, float value)
	{
		animator.SetFloat(name, value);
	}

	public static void SetFloatReflected(this Animator animator, int name, float value, float dampTime, float deltaTime)
	{
		animator.SetFloat(name, value, dampTime, deltaTime);
	}

	public static void SetIntegerReflected(this Animator animator, string name, int value)
	{
		animator.SetInteger(name, value);
	}

	public static void SetBoolReflected(this Animator animator, string name, bool value)
	{
		animator.SetBool(name, value);
	}

	public static void SetTriggerReflected(this Animator animator, string name)
	{
		animator.SetTrigger(name);
	}

	public static void SetLayerWeightReflected(this Animator animator, int index, float value)
	{
		animator.SetLayerWeight(index, value);
	}
}
