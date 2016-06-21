using System;
using UnityEngine;

public class AddSomeForce : MonoBehaviour
{
	private bool done;

	public Transform aChild;

	private void Awake()
	{
		this.aChild = base.GetComponentsInChildren<Transform>()[1];
	}

	private void FixedUpdate()
	{
		if (this.done)
		{
			return;
		}
		this.done = true;
		base.GetComponent<Rigidbody>().angularVelocity = UnityEngine.Random.insideUnitSphere * 10f;
	}
}
