using System;
using UnityEngine;

public class Mudguard : MonoBehaviour
{
	public Wheel wheel;

	private Quaternion originalRotation;

	private Vector3 offset;

	private void Start()
	{
		this.originalRotation = base.transform.localRotation;
		this.offset = base.transform.position - this.wheel.wheelModel.transform.position;
	}

	private void Update()
	{
		base.transform.localRotation = this.originalRotation * Quaternion.Euler(0f, this.wheel.car.CurrentSteerAngle, 0f);
		base.transform.position = this.wheel.wheelModel.transform.position + this.offset;
	}
}
