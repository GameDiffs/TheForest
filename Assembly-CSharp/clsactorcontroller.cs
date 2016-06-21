using System;
using UnityEngine;

public class clsactorcontroller : MonoBehaviour
{
	public float vargamspeed = 5f;

	public float vargamrotspeed = 10f;

	public float vargravity = 20f;

	private CharacterController varcontroller;

	private Vector3 varmovement;

	private float varfallstarttime;

	private float vartimer;

	private clsragdollhelper varhelper;

	private Ray vargravityray = default(Ray);

	private bool varfalling;

	private bool varfallen;

	private void Awake()
	{
		this.varcontroller = base.GetComponent<CharacterController>();
		this.varhelper = base.GetComponent<clsragdollhelper>();
		this.vargravityray = new Ray(base.transform.position, Vector3.down);
	}

	private void LateUpdate()
	{
		if (this.varcontroller != null)
		{
			base.transform.Rotate(Vector3.up * (this.vargamrotspeed * Time.deltaTime * Time.timeScale));
			this.varmovement = base.transform.forward * (this.vargamspeed * Time.deltaTime * Time.timeScale);
			if (!this.varcontroller.isGrounded)
			{
				if (!Physics.Raycast(base.transform.position, (this.varcontroller.velocity + this.vargravityray.direction).normalized, 1f))
				{
					this.varfalling = true;
				}
				Debug.DrawRay(base.transform.position, this.vargravityray.direction, Color.red);
				this.varmovement.y = -this.vargravity * Time.deltaTime;
			}
			else if (this.varfalling)
			{
				this.varfalling = false;
			}
			this.varcontroller.Move(this.varmovement);
			if (this.varhelper != null && this.varfalling && !this.varfallen)
			{
				Transform transform = this.varhelper.metgoragdoll(this.varcontroller.velocity);
				clscameratarget componentInChildren = Camera.main.GetComponentInChildren<clscameratarget>();
				if (componentInChildren.vargamcurrentscenario == 1)
				{
					componentInChildren.vargamtarget = transform.GetChild(0);
				}
				this.varcontroller.enabled = false;
				this.varfallen = false;
			}
		}
	}

	public void metactivate()
	{
		clsragdollimbifier componentInChildren = base.GetComponentInChildren<clsragdollimbifier>();
		if (componentInChildren != null)
		{
			componentInChildren.enabled = true;
		}
	}
}
