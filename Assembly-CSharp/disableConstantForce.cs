using System;
using UnityEngine;

public class disableConstantForce : MonoBehaviour
{
	public float delay;

	private ConstantForce force;

	private bool disable;

	private void Start()
	{
		base.Invoke("disableForce", this.delay);
		this.force = base.GetComponent<ConstantForce>();
	}

	private void Update()
	{
		if (!this.disable)
		{
			base.transform.Rotate(base.transform.rotation.x, 30f * Time.deltaTime, base.transform.rotation.z, Space.Self);
		}
	}

	private void disableForce()
	{
		this.force.torque = Vector3.zero;
		this.force.force = Vector3.zero;
		this.disable = true;
	}
}
