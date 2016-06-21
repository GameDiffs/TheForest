using System;
using UnityEngine;

public class AstarSmoothFollow2 : MonoBehaviour
{
	public Transform target;

	public float distance = 3f;

	public float height = 3f;

	public float damping = 5f;

	public bool smoothRotation = true;

	public bool followBehind = true;

	public float rotationDamping = 10f;

	public bool staticOffset;

	private void LateUpdate()
	{
		Vector3 to;
		if (this.staticOffset)
		{
			to = this.target.position + new Vector3(0f, this.height, this.distance);
		}
		else if (this.followBehind)
		{
			to = this.target.TransformPoint(0f, this.height, -this.distance);
		}
		else
		{
			to = this.target.TransformPoint(0f, this.height, this.distance);
		}
		base.transform.position = Vector3.Lerp(base.transform.position, to, Time.deltaTime * this.damping);
		if (this.smoothRotation)
		{
			Quaternion to2 = Quaternion.LookRotation(this.target.position - base.transform.position, this.target.up);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, to2, Time.deltaTime * this.rotationDamping);
		}
		else
		{
			base.transform.LookAt(this.target, this.target.up);
		}
	}
}
