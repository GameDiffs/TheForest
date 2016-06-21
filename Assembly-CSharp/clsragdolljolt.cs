using System;
using UnityEngine;

public class clsragdolljolt : MonoBehaviour
{
	public float vargamforcemin = -1f;

	public float vargamforcemax = 0.5f;

	public float vargamtorquemin = -200f;

	public float vargamtorquemax = 100f;

	public ForceMode vargamforcemode = ForceMode.VelocityChange;

	private void Start()
	{
		this.metjolt(this.vargamforcemin, this.vargamforcemax, this.vargamtorquemin, this.vargamtorquemax, this.vargamforcemode);
	}

	private void metjolt(float varpforcemin, float varpforcemax, float varptorquemin, float varptorquemax, ForceMode varpforcemode)
	{
		Rigidbody[] componentsInChildren = base.gameObject.GetComponentsInChildren<Rigidbody>();
		if (componentsInChildren.Length < 1)
		{
			return;
		}
		Vector3 force = new Vector3(UnityEngine.Random.Range(varpforcemin, varpforcemax), UnityEngine.Random.Range(varpforcemin, varpforcemax), UnityEngine.Random.Range(varpforcemin, varpforcemax));
		Vector3 torque = new Vector3(UnityEngine.Random.Range(varptorquemin, varptorquemax), UnityEngine.Random.Range(varptorquemin, varptorquemax), UnityEngine.Random.Range(varptorquemin, varptorquemax));
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].AddForce(force, varpforcemode);
			componentsInChildren[i].AddTorque(torque, varpforcemode);
		}
	}
}
