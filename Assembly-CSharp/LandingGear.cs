using System;
using UnityEngine;

public class LandingGear : MonoBehaviour
{
	private enum GearState
	{
		Raised = -1,
		Lowered = 1
	}

	public float raiseAtAltitude = 40f;

	public float lowerAtAltitude = 40f;

	private LandingGear.GearState state = LandingGear.GearState.Lowered;

	private Animator animator;

	private AeroplaneController plane;

	private void Start()
	{
		this.plane = base.GetComponent<AeroplaneController>();
		this.animator = base.GetComponent<Animator>();
	}

	private void Update()
	{
		if (this.state == LandingGear.GearState.Lowered && this.plane.Altitude > this.raiseAtAltitude && base.GetComponent<Rigidbody>().velocity.y > 0f)
		{
			this.state = LandingGear.GearState.Raised;
		}
		if (this.state == LandingGear.GearState.Raised && this.plane.Altitude < this.lowerAtAltitude && base.GetComponent<Rigidbody>().velocity.y < 0f)
		{
			this.state = LandingGear.GearState.Lowered;
		}
		this.animator.SetIntegerReflected("GearState", (int)this.state);
	}
}
