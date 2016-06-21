using System;
using UnityEngine;

public class rbSolve : MonoBehaviour
{
	private void Start()
	{
		Rigidbody[] allComponentsInChildren = base.transform.GetAllComponentsInChildren<Rigidbody>();
		Rigidbody[] array = allComponentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Rigidbody rigidbody = array[i];
			rigidbody.solverIterationCount = 128;
			Debug.Log("doing count set");
		}
	}
}
