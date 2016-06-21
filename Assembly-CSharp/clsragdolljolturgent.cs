using System;
using UnityEngine;

public class clsragdolljolturgent : MonoBehaviour
{
	public float vargamforcemin = -1f;

	public float vargamforcemax = 0.5f;

	public float vargamtorquemin = -200f;

	public float vargamtorquemax = 100f;

	public ForceMode vargamforcemode = ForceMode.VelocityChange;

	public bool vargamjoltspine = true;

	public bool vargamjolthead = true;

	public bool vargamjoltarmleft = true;

	public bool vargamjoltarmright = true;

	public bool vargamjoltlegleft = true;

	public bool vargamjoltlegright = true;

	private void Start()
	{
		this.metjolturgent(this.vargamforcemin, this.vargamforcemax, this.vargamtorquemin, this.vargamtorquemax, ForceMode.VelocityChange, this.vargamjoltspine, this.vargamjolthead, this.vargamjoltarmleft, this.vargamjoltarmright, this.vargamjoltlegleft, this.vargamjoltlegright);
	}

	private void metjolturgent(float varpforcemin, float varpforcemax, float varptorquemin, float varptorquemax, ForceMode varpforcemode, bool varpjoltspine, bool varpjolthead, bool varpjoltarmleft, bool varpjoltarmright, bool varpjoltlegright, bool varpjoltlegleft)
	{
		clsurgent componentInChildren = base.gameObject.GetComponentInChildren<clsurgent>();
		if (componentInChildren == null)
		{
			return;
		}
		Vector3 force = new Vector3(UnityEngine.Random.Range(varpforcemin, varpforcemax), UnityEngine.Random.Range(varpforcemin, varpforcemax), UnityEngine.Random.Range(varpforcemin, varpforcemax));
		Vector3 torque = new Vector3(UnityEngine.Random.Range(varptorquemin, varptorquemax), UnityEngine.Random.Range(varptorquemin, varptorquemax), UnityEngine.Random.Range(varptorquemin, varptorquemax));
		if (varpjoltspine)
		{
			for (int i = 0; i < componentInChildren.vargamnodes.vargamspine.Length; i++)
			{
				componentInChildren.vargamnodes.vargamspine[i].GetComponent<Rigidbody>().AddForce(force, varpforcemode);
				componentInChildren.vargamnodes.vargamspine[i].GetComponent<Rigidbody>().AddTorque(torque, varpforcemode);
			}
		}
		if (varpjolthead)
		{
			for (int j = 0; j < componentInChildren.vargamnodes.vargamhead.Length; j++)
			{
				componentInChildren.vargamnodes.vargamhead[j].GetComponent<Rigidbody>().AddForce(force, varpforcemode);
				componentInChildren.vargamnodes.vargamhead[j].GetComponent<Rigidbody>().AddTorque(torque, varpforcemode);
			}
		}
		if (varpjoltarmleft)
		{
			for (int k = 0; k < componentInChildren.vargamnodes.vargamarmleft.Length; k++)
			{
				componentInChildren.vargamnodes.vargamarmleft[k].GetComponent<Rigidbody>().AddForce(force, varpforcemode);
				componentInChildren.vargamnodes.vargamarmleft[k].GetComponent<Rigidbody>().AddTorque(torque, varpforcemode);
			}
		}
		if (varpjoltarmright)
		{
			for (int l = 0; l < componentInChildren.vargamnodes.vargamarmright.Length; l++)
			{
				componentInChildren.vargamnodes.vargamarmright[l].GetComponent<Rigidbody>().AddForce(force, varpforcemode);
				componentInChildren.vargamnodes.vargamarmright[l].GetComponent<Rigidbody>().AddTorque(torque, varpforcemode);
			}
		}
		if (varpjoltlegleft)
		{
			for (int m = 0; m < componentInChildren.vargamnodes.vargamlegleft.Length; m++)
			{
				componentInChildren.vargamnodes.vargamlegleft[m].GetComponent<Rigidbody>().AddForce(force, varpforcemode);
				componentInChildren.vargamnodes.vargamlegleft[m].GetComponent<Rigidbody>().AddTorque(torque, varpforcemode);
			}
		}
		if (varpjoltlegright)
		{
			for (int n = 0; n < componentInChildren.vargamnodes.vargamlegright.Length; n++)
			{
				componentInChildren.vargamnodes.vargamlegright[n].GetComponent<Rigidbody>().AddForce(force, varpforcemode);
				componentInChildren.vargamnodes.vargamlegright[n].GetComponent<Rigidbody>().AddTorque(torque, varpforcemode);
			}
		}
	}
}
