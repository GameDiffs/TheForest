using System;
using System.Collections;
using UnityEngine;

public static class RadicalRoutineExtensions
{
	public class RadicalRoutineBehaviour : MonoBehaviour
	{
	}

	public static RadicalRoutine StartExtendedCoroutine(this MonoBehaviour behaviour, IEnumerator coRoutine)
	{
		RadicalRoutine radicalRoutine = RadicalRoutine.Create(coRoutine);
		radicalRoutine.trackedObject = behaviour;
		radicalRoutine.Run();
		return radicalRoutine;
	}

	public static RadicalRoutine StartExtendedCoroutine(this GameObject go, IEnumerator coRoutine)
	{
		MonoBehaviour behaviour = go.GetComponent<MonoBehaviour>() ?? go.AddComponent<RadicalRoutineExtensions.RadicalRoutineBehaviour>();
		return behaviour.StartExtendedCoroutine(coRoutine);
	}

	public static RadicalRoutine StartExtendedCoroutine(this Component co, IEnumerator coRoutine)
	{
		return co.gameObject.StartExtendedCoroutine(coRoutine);
	}
}
