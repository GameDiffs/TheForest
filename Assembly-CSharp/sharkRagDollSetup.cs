using System;
using UnityEngine;

public class sharkRagDollSetup : MonoBehaviour
{
	private Rigidbody[] allRb;

	private void Start()
	{
		this.allRb = base.transform.GetComponentsInChildren<Rigidbody>();
		this.inWater();
	}

	private void inWater()
	{
		for (int i = 0; i < this.allRb.Length; i++)
		{
			this.allRb[i].drag = 10f;
			this.allRb[i].angularDrag = 10f;
		}
	}
}
