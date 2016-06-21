using System;
using UnityEngine;

[RequireComponent(typeof(AeroplaneController))]
public class AeroplaneUserControl : MonoBehaviour
{
	public float maxRollAngle = 80f;

	public float maxPitchAngle = 80f;

	private AeroplaneController aeroplane;

	private void Awake()
	{
		this.aeroplane = base.GetComponent<AeroplaneController>();
	}

	private void FixedUpdate()
	{
		float axis = CrossPlatformInput.GetAxis("Mouse X");
		float axis2 = CrossPlatformInput.GetAxis("Mouse Y");
		float axis3 = CrossPlatformInput.GetAxis("Horizontal");
		float axis4 = CrossPlatformInput.GetAxis("Vertical");
		this.AdjustInputForMobileControls(ref axis, ref axis2, ref axis4);
		bool button = CrossPlatformInput.GetButton("Fire1");
		this.aeroplane.Move(axis, axis2, axis3, axis4, button);
	}

	private void AdjustInputForMobileControls(ref float roll, ref float pitch, ref float throttle)
	{
	}
}
