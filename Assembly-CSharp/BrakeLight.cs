using System;
using UnityEngine;

public class BrakeLight : MonoBehaviour
{
	public CarController car;

	private void Update()
	{
		base.GetComponent<Renderer>().enabled = (this.car.BrakeInput > 0f);
	}
}
