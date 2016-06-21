using System;
using UnityEngine;

public class CarSelfRighting : MonoBehaviour
{
	[SerializeField]
	private float waitTime = 3f;

	[SerializeField]
	private float velocityThreshold = 1f;

	private float lastOkTime;

	private void Update()
	{
		if (base.transform.up.y > 0f || base.GetComponent<Rigidbody>().velocity.magnitude > this.velocityThreshold)
		{
			this.lastOkTime = Time.time;
		}
		if (Time.time > this.lastOkTime + this.waitTime)
		{
			this.RightCar();
		}
	}

	private void RightCar()
	{
		base.transform.position += Vector3.up;
		base.transform.rotation = Quaternion.LookRotation(base.transform.forward);
	}
}
