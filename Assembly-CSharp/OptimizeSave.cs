using System;
using UnityEngine;

public class OptimizeSave : MonoBehaviour
{
	private void Start()
	{
		foreach (Transform transform in base.transform)
		{
			transform.gameObject.SetActive(false);
		}
	}
}
