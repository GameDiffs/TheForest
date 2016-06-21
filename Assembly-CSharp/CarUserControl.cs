using System;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class CarUserControl : MonoBehaviour
{
	private CarController car;

	private void Awake()
	{
		this.car = base.GetComponent<CarController>();
	}

	private void FixedUpdate()
	{
		float axis = Input.GetAxis("Horizontal");
		float axis2 = Input.GetAxis("Vertical");
		this.car.Move(axis, axis2);
	}
}
