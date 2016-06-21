using System;
using UnityEngine;

public static class TaskHelpers
{
	public static WaitForAnimation WaitForAnimation(this GameObject go, string name)
	{
		return go.WaitForAnimation(name, 1f);
	}

	public static WaitForAnimation WaitForAnimation(this GameObject go, string name, float time)
	{
		return new WaitForAnimation(go, name, time, -1f);
	}

	public static WaitForAnimation WaitForAnimationWeight(this GameObject go, string name)
	{
		return go.WaitForAnimationWeight(name, 0f);
	}

	public static WaitForAnimation WaitForAnimationWeight(this GameObject go, string name, float weight)
	{
		return new WaitForAnimation(go, name, 0f, weight);
	}
}
