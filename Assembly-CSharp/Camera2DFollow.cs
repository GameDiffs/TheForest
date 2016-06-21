using System;
using UnityEngine;

public class Camera2DFollow : MonoBehaviour
{
	public Transform target;

	public float damping = 1f;

	public float lookAheadFactor = 3f;

	public float lookAheadReturnSpeed = 0.5f;

	public float lookAheadMoveThreshold = 0.1f;

	private float offsetZ;

	private Vector3 lastTargetPosition;

	private Vector3 currentVelocity;

	private Vector3 lookAheadPos;

	private void Start()
	{
		this.lastTargetPosition = this.target.position;
		this.offsetZ = (base.transform.position - this.target.position).z;
		base.transform.parent = null;
	}

	private void Update()
	{
		float x = (this.target.position - this.lastTargetPosition).x;
		bool flag = Mathf.Abs(x) > this.lookAheadMoveThreshold;
		if (flag)
		{
			this.lookAheadPos = this.lookAheadFactor * Vector3.right * Mathf.Sign(x);
		}
		else
		{
			this.lookAheadPos = Vector3.MoveTowards(this.lookAheadPos, Vector3.zero, Time.deltaTime * this.lookAheadReturnSpeed);
		}
		Vector3 vector = this.target.position + this.lookAheadPos + Vector3.forward * this.offsetZ;
		Vector3 position = Vector3.SmoothDamp(base.transform.position, vector, ref this.currentVelocity, this.damping);
		base.transform.position = position;
		this.lastTargetPosition = this.target.position;
	}
}
