using System;
using UnityEngine;

public class setupPassengers : MonoBehaviour
{
	public bool p1;

	public bool p2;

	public bool p3;

	public bool p4;

	public bool doFlyBack;

	public bool frontSeats;

	private Animator animator;

	private void Start()
	{
		this.animator = base.GetComponent<Animator>();
		if (this.p1)
		{
			this.animator.SetBoolReflected("p1", true);
		}
		if (this.p2)
		{
			this.animator.SetBoolReflected("p2", true);
		}
		if (this.p3)
		{
			this.animator.SetBoolReflected("p3", true);
		}
	}

	public void triggerFall1()
	{
		if (!this.frontSeats)
		{
			this.animator.SetTriggerReflected("fallForward");
		}
	}

	public void triggerFlyBack()
	{
		if (this.doFlyBack)
		{
			this.animator.SetBoolReflected("flyBack", true);
		}
	}

	public void triggerFrontSeats()
	{
		if (this.frontSeats)
		{
			this.animator.SetBoolReflected("frontSeats", true);
		}
	}
}
