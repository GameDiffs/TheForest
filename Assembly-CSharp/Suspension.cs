using System;
using UnityEngine;

public class Suspension : MonoBehaviour
{
	public Wheel wheel;

	private Vector3 originalPosition;

	private void Start()
	{
		this.originalPosition = base.transform.localPosition;
	}

	private void Update()
	{
		base.transform.localPosition = this.originalPosition + this.wheel.suspensionSpringPos * base.transform.up;
	}
}
