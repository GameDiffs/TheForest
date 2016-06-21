using System;
using UnityEngine;

public class ControlReflectionIntensity : MonoBehaviour
{
	private ReflectionProbe MyIntensity;

	private void Start()
	{
		this.MyIntensity = base.gameObject.GetComponent<ReflectionProbe>();
	}

	private void FixedUpdate()
	{
		this.MyIntensity.intensity = TheForestAtmosphere.ReflectionAmount;
	}
}
