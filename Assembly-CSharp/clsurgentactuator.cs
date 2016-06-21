using System;
using U_r_g_utils;
using UnityEngine;

public class clsurgentactuator : MonoBehaviour
{
	public clsurgutils.enumparttypes vargamparttype;

	public string vargampartinstancename = string.Empty;

	public int vargampartindex;

	public bool vargamactuatorenabled = true;

	public clsurgent vargamurgentsource;

	public Transform vargamsource;

	public Transform vargamparent;

	private void metmanagekinematic()
	{
		if (this.vargamurgentsource == null)
		{
			base.GetComponent<Rigidbody>().isKinematic = false;
			base.transform.parent = null;
		}
		else
		{
			this.vargamurgentsource.GetComponent<Animation>().Stop();
			if (base.transform == this.vargamsource || this.vargamparttype == clsurgutils.enumparttypes.head || this.vargamparttype == clsurgutils.enumparttypes.spine)
			{
				clsurgutils.metdriveurgent(this.vargamurgentsource, null);
				this.vargamurgentsource.transform.GetComponent<Animation>().Stop();
				this.vargamurgentsource.transform.GetComponent<Animation>().animatePhysics = false;
				CharacterController component = base.transform.root.GetComponent<CharacterController>();
				if (component != null)
				{
					Vector3 velocity = component.velocity;
					UnityEngine.Object.Destroy(component);
					this.vargamurgentsource.vargamnodes.vargamspine[0].GetComponent<Rigidbody>().AddForce(velocity * 7500f);
				}
			}
			else
			{
				clsurgutils.metdriveurgent(this.vargamurgentsource, this);
				base.transform.parent = this.vargamurgentsource.transform;
			}
		}
	}

	private void OnCollisionEnter(Collision varpsource)
	{
		int vargamcasemanager = this.vargamurgentsource.vargamcasemanager;
		switch (vargamcasemanager + 3)
		{
		case 0:
			if (this.vargamactuatorenabled && varpsource.gameObject.CompareTag("missile") && this.vargamparttype != clsurgutils.enumparttypes.spine)
			{
				this.vargamactuatorenabled = false;
				base.transform.parent = this.vargamurgentsource.transform;
				clsurgutils.metdrivebodypart(this.vargamurgentsource, this.vargamparttype, 0);
				clsragdollimbifier componentInChildren = base.transform.root.GetComponentInChildren<clsragdollimbifier>();
				if (componentInChildren != null)
				{
					switch (this.vargamparttype)
					{
					case clsurgutils.enumparttypes.arm_left:
						componentInChildren.varla = true;
						break;
					case clsurgutils.enumparttypes.arm_right:
					{
						componentInChildren.varra = true;
						clsdrop componentInChildren2 = base.transform.root.GetComponentInChildren<clsdrop>();
						if (componentInChildren2 != null)
						{
							componentInChildren2.metdrop(varpsource.impactForceSum, true);
						}
						break;
					}
					case clsurgutils.enumparttypes.leg_left:
						componentInChildren.varll = true;
						break;
					case clsurgutils.enumparttypes.leg_right:
						componentInChildren.varrl = true;
						break;
					}
				}
				else
				{
					Debug.LogError("No ragdollimbifier found. Part repair compromised.");
				}
				base.GetComponent<Rigidbody>().AddForceAtPosition(varpsource.impactForceSum, varpsource.contacts[0].point, ForceMode.VelocityChange);
			}
			break;
		case 1:
		{
			if (!this.vargamactuatorenabled || (!varpsource.gameObject.CompareTag("missile") && !varpsource.gameObject.CompareTag("terrain")))
			{
				return;
			}
			clsdismemberator componentInChildren3 = this.vargamurgentsource.GetComponentInChildren<clsdismemberator>();
			if (componentInChildren3 != null)
			{
				float num = UnityEngine.Random.Range(0f, 0.99f);
				if (num > 0.75f)
				{
					clsurgutils.metdismember(base.transform, componentInChildren3.vargamstumpmaterial, componentInChildren3, componentInChildren3.vargamparticleparent, componentInChildren3.vargamparticlechild, true, true);
				}
			}
			else
			{
				Debug.LogError("No Dismemberator Class in source D host.");
			}
			break;
		}
		case 2:
		{
			if (this.vargamactuatorenabled && varpsource.gameObject.CompareTag("missile"))
			{
				this.vargamactuatorenabled = false;
				clsurgutils.metdriveurgent(this.vargamurgentsource, null);
				base.GetComponent<Rigidbody>().AddForceAtPosition(varpsource.impactForceSum, varpsource.contacts[0].point, ForceMode.VelocityChange);
			}
			clsdrop componentInChildren2 = base.transform.root.GetComponentInChildren<clsdrop>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.metdrop(varpsource.impactForceSum, true);
			}
			break;
		}
		}
	}

	private void OnTriggerEnter(Collider varpother)
	{
		int vargamcasemanager = this.vargamurgentsource.vargamcasemanager;
		if (vargamcasemanager != -2)
		{
			if (vargamcasemanager == -1)
			{
				if (this.vargamactuatorenabled && varpother.gameObject.CompareTag("missile"))
				{
					if (base.GetComponent<Rigidbody>().isKinematic)
					{
						this.metmanagekinematic();
					}
					this.vargamactuatorenabled = false;
				}
			}
		}
		else
		{
			if (!this.vargamactuatorenabled || !varpother.CompareTag("missile"))
			{
				return;
			}
			this.metmanagekinematic();
			clsdismemberator componentInChildren = this.vargamurgentsource.GetComponentInChildren<clsdismemberator>();
			if (componentInChildren != null)
			{
				float num = UnityEngine.Random.Range(0f, 0.99f);
				if (num > 0.75f)
				{
					clsurgutils.metdismember(base.transform, componentInChildren.vargamstumpmaterial, componentInChildren, componentInChildren.vargamparticleparent, componentInChildren.vargamparticlechild, true, true);
				}
			}
			else
			{
				Debug.LogError("No Dismemberator Class in source D host.");
			}
		}
	}
}
