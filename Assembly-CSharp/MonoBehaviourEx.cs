using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class MonoBehaviourEx : MonoBehaviour
{
	public new RadicalRoutine StartCoroutine(IEnumerator func)
	{
		return this.StartExtendedCoroutine(func);
	}

	public new RadicalRoutine StartCoroutine(string func)
	{
		MethodInfo method = base.GetType().GetMethod(func, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		if (method.ReturnType == typeof(IEnumerator))
		{
			return this.StartCoroutine((IEnumerator)method.Invoke(this, null));
		}
		return null;
	}
}
