using System;
using UnityEngine;

public class FTEM_DeadTime : MonoBehaviour
{
	public float deadTime;

	private void Awake()
	{
		UnityEngine.Object.Destroy(base.gameObject, this.deadTime);
	}

	private void Update()
	{
	}
}
