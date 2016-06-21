using System;
using U_r_g_utils;
using UnityEngine;

public class clsragdollimbifier : MonoBehaviour
{
	public clsurgent vargamurgentities;

	[HideInInspector]
	public bool varla;

	[HideInInspector]
	public bool varra;

	[HideInInspector]
	public bool varll;

	[HideInInspector]
	public bool varrl;

	private void Start()
	{
		if (this.vargamurgentities == null)
		{
			this.vargamurgentities = base.GetComponent<clsurgent>();
		}
		if (base.transform.root.GetComponent<Animation>())
		{
			base.transform.root.GetComponent<Animation>().animatePhysics = true;
		}
	}

	private void OnGUI()
	{
		if (this.vargamurgentities != null)
		{
			GUILayout.Label("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n", new GUILayoutOption[0]);
			if (this.varla)
			{
				if (GUILayout.Button("- Restore left arm", new GUILayoutOption[0]))
				{
					clsurgentactuator component = this.vargamurgentities.vargamnodes.vargamarmleft[0].GetComponent<clsurgentactuator>();
					this.vargamurgentities.vargamnodes.vargamarmleft[0].parent = component.vargamparent;
					clsurgutils.metdriveanimatebodypart(this.vargamurgentities, clsurgutils.enumparttypes.arm_left, 0, true);
					this.varla = false;
				}
			}
			else if (GUILayout.Button("Break left arm", new GUILayoutOption[0]))
			{
				this.vargamurgentities.vargamnodes.vargamarmleft[0].parent = this.vargamurgentities.transform;
				clsurgutils.metdrivebodypart(this.vargamurgentities, clsurgutils.enumparttypes.arm_left, 0);
				this.varla = true;
			}
			if (this.varra)
			{
				if (GUILayout.Button("- Restore right arm", new GUILayoutOption[0]))
				{
					clsurgentactuator component2 = this.vargamurgentities.vargamnodes.vargamarmright[0].GetComponent<clsurgentactuator>();
					if (component2 != null)
					{
						clsurgutils.metdriveanimatebodypart(this.vargamurgentities, clsurgutils.enumparttypes.arm_right, 0, true);
					}
					this.vargamurgentities.vargamnodes.vargamarmright[0].parent = component2.vargamparent;
					this.varra = false;
				}
			}
			else if (GUILayout.Button("Break right arm", new GUILayoutOption[0]))
			{
				this.vargamurgentities.vargamnodes.vargamarmright[0].parent = this.vargamurgentities.transform;
				clsurgutils.metdrivebodypart(this.vargamurgentities, clsurgutils.enumparttypes.arm_right, 0);
				clsdrop componentInChildren = base.GetComponentInChildren<clsdrop>();
				if (componentInChildren != null)
				{
					componentInChildren.metdrop(false);
				}
				this.varra = true;
			}
			if (this.varll)
			{
				if (GUILayout.Button("- Restore left leg", new GUILayoutOption[0]))
				{
					clsurgentactuator component3 = this.vargamurgentities.vargamnodes.vargamlegleft[0].GetComponent<clsurgentactuator>();
					if (component3 != null)
					{
						clsurgutils.metdriveanimatebodypart(this.vargamurgentities, clsurgutils.enumparttypes.leg_left, 0, true);
					}
					this.vargamurgentities.vargamnodes.vargamlegleft[0].parent = component3.vargamparent;
					this.varll = false;
				}
			}
			else if (GUILayout.Button("Break left leg", new GUILayoutOption[0]))
			{
				this.vargamurgentities.vargamnodes.vargamlegleft[0].parent = this.vargamurgentities.transform;
				clsurgutils.metdrivebodypart(this.vargamurgentities, clsurgutils.enumparttypes.leg_left, 0);
				this.varll = true;
			}
			if (this.varrl)
			{
				if (GUILayout.Button("- Restore right leg", new GUILayoutOption[0]))
				{
					clsurgentactuator component4 = this.vargamurgentities.vargamnodes.vargamlegright[0].GetComponent<clsurgentactuator>();
					if (component4 != null)
					{
						clsurgutils.metdriveanimatebodypart(this.vargamurgentities, clsurgutils.enumparttypes.leg_right, 0, true);
					}
					this.vargamurgentities.vargamnodes.vargamlegright[0].parent = component4.vargamparent;
					this.varrl = false;
				}
			}
			else if (GUILayout.Button("Break right leg", new GUILayoutOption[0]))
			{
				this.vargamurgentities.vargamnodes.vargamlegright[0].parent = this.vargamurgentities.transform;
				clsurgutils.metdrivebodypart(this.vargamurgentities, clsurgutils.enumparttypes.leg_right, 0);
				this.varrl = true;
			}
			if (GUILayout.Button("URG!", new GUILayoutOption[0]))
			{
				clsurgutils.metdriveurgent(this.vargamurgentities, null);
				this.vargamurgentities.transform.GetComponent<Animation>().Stop();
				this.vargamurgentities.transform.GetComponent<Animation>().animatePhysics = false;
				CharacterController component5 = this.vargamurgentities.transform.root.GetComponent<CharacterController>();
				Vector3 a = default(Vector3);
				if (component5 != null)
				{
					a = component5.velocity;
					a.y = 0.1f;
					UnityEngine.Object.Destroy(component5);
					this.vargamurgentities.vargamnodes.vargamspine[0].GetComponent<Rigidbody>().AddForce(a * 600f);
				}
				clsdrop componentInChildren2 = base.GetComponentInChildren<clsdrop>();
				if (componentInChildren2 != null)
				{
					componentInChildren2.metdrop(a * 60f, true);
				}
				base.enabled = false;
			}
		}
	}
}
