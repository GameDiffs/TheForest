using System;
using U_r_g_utils;
using UnityEngine;

public class clsbike : MonoBehaviour
{
	private const float cnsspring = 2500f;

	private const float cnsdamper = 200f;

	private const float cnssuspension = 0.5f;

	public float vargammotormax = 500f;

	public float vargamspeedmax = 15f;

	public float vargambrakemax = 150f;

	public Rigidbody vargamchassisbody;

	private int varwheelscount;

	private WheelCollider[] varwheels;

	private float varpower;

	private float varsteering;

	private float varbrake;

	private float varspeedmax;

	private bool varpassenger = true;

	private Transform vargampassenger;

	private clsdismemberator varD;

	private Transform[] varbones;

	private void Start()
	{
		if (this.vargamchassisbody == null)
		{
			Debug.LogError("The bike needs a rigidbody to function.");
			base.enabled = false;
		}
		this.varwheels = base.GetComponentsInChildren<WheelCollider>();
		this.varwheelscount = this.varwheels.Length;
		for (int i = 0; i < this.varwheelscount; i++)
		{
			JointSpring suspensionSpring = default(JointSpring);
			suspensionSpring.spring = 2500f;
			suspensionSpring.damper = 200f;
			suspensionSpring.targetPosition = 0f;
			this.varwheels[i].suspensionSpring = suspensionSpring;
			this.varwheels[i].suspensionDistance = 0.5f;
			this.varpower = 1f;
			this.varbrake = 0f;
		}
		this.vargamchassisbody.centerOfMass = new Vector3(0f, -0.05f, 0f);
		this.varspeedmax = this.vargamspeedmax * this.vargamspeedmax;
		this.varD = base.transform.root.GetComponentInChildren<clsdismemberator>();
		if (this.varD != null)
		{
			this.varbones = this.varD.vargamskinnedmeshrenderer.bones;
		}
	}

	private void FixedUpdate()
	{
		Debug.DrawRay(base.transform.position, Vector3.down, Color.yellow);
		if (this.varpassenger && !Physics.Raycast(base.transform.position, Vector3.down, 1f))
		{
			this.vargampassenger = base.transform.Find("Lerpz_kinematic");
			if (this.vargampassenger != null)
			{
				base.Invoke("metfalling", 0.3f);
			}
			this.varpassenger = false;
		}
		if (this.vargamchassisbody.velocity.sqrMagnitude > this.varspeedmax)
		{
			this.varpower = 0f;
		}
		if (this.varwheels[0] != null)
		{
			this.varwheels[0].motorTorque = this.vargammotormax * this.varpower;
			this.varwheels[0].brakeTorque = this.vargambrakemax * this.varbrake;
		}
		if (this.varwheels[1] != null)
		{
			this.varwheels[1].motorTorque = this.vargammotormax * this.varpower;
			this.varwheels[1].brakeTorque = this.vargambrakemax * this.varbrake;
		}
	}

	private void OnTriggerEnter(Collider varpother)
	{
		if (varpother.CompareTag("terrain"))
		{
			base.GetComponent<Collider>().enabled = false;
			if (this.varD != null)
			{
				int count = this.varD.vargamboneindexes.Count;
				float num = this.vargamchassisbody.velocity.sqrMagnitude / (this.vargamspeedmax * this.vargamspeedmax);
				int num2 = (int)((float)count * num);
				int num3 = 0;
				for (int i = 0; i < this.varbones.Length; i++)
				{
					float num4 = UnityEngine.Random.Range(0f, 0.99f);
					if (num4 > 1f - num && this.varbones[i].GetComponent<Collider>() != null)
					{
						clsurgutils.metdismember(this.varbones[i], null, this.varD, null, null, true, true);
						num3++;
					}
					if (num3 > num2)
					{
						break;
					}
				}
			}
		}
	}

	private void metfalling()
	{
		this.vargampassenger.parent = null;
		clsurgutils.metgotangible(this.vargampassenger, true);
		clsurgutils.metgodriven(this.vargampassenger, base.GetComponent<Rigidbody>().velocity);
	}
}
