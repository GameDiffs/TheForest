using System;
using UnityEngine;

public class turtleAvoidance : MonoBehaviour
{
	public GameObject ControllerGo;

	private turtleAnimatorControl turtleControl;

	private Rigidbody rb;

	private Collider currCollider;

	public bool locked;

	private Vector3 localPos;

	private Vector3 localRot;

	public int axisDir;

	private void Start()
	{
		this.turtleControl = base.transform.root.GetComponentInChildren<turtleAnimatorControl>();
		this.localPos = base.transform.localPosition;
		this.localRot = base.transform.localEulerAngles;
		this.rb = base.GetComponent<Rigidbody>();
	}

	private void OnEnable()
	{
		this.locked = false;
	}

	private void FixedUpdate()
	{
		if (this.locked)
		{
			this.turtleControl.blockedCollider = this.currCollider;
			this.turtleControl.blocked = true;
		}
		else
		{
			this.turtleControl.blocked = false;
		}
		this.locked = false;
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("Shell") && other.gameObject != base.gameObject)
		{
			this.currCollider = other;
			this.locked = true;
		}
	}

	private void doCollisionCheck(Collider other)
	{
		if (other.gameObject.CompareTag("Shell"))
		{
			if (other == null)
			{
				return;
			}
			this.currCollider = other;
			this.locked = true;
		}
	}
}
