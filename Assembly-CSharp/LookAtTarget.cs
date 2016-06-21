using System;
using UnityEngine;

public class LookatTarget : AbstractTargetFollower
{
	public Vector2 rotationRange;

	public float followSpeed = 1f;

	private Vector3 followAngles;

	private Vector3 followVelocity;

	private Quaternion originalRotation;

	protected override void Start()
	{
		base.Start();
		this.originalRotation = base.transform.localRotation;
	}

	protected override void FollowTarget(float deltaTime)
	{
		base.transform.localRotation = this.originalRotation;
		Vector3 vector = base.transform.InverseTransformPoint(this.target.position);
		float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
		num = Mathf.Clamp(num, -this.rotationRange.y * 0.5f, this.rotationRange.y * 0.5f);
		base.transform.localRotation = this.originalRotation * Quaternion.Euler(0f, num, 0f);
		vector = base.transform.InverseTransformPoint(this.target.position);
		float num2 = Mathf.Atan2(vector.y, vector.z) * 57.29578f;
		num2 = Mathf.Clamp(num2, -this.rotationRange.x * 0.5f, this.rotationRange.x * 0.5f);
		Vector3 target = new Vector3(this.followAngles.x + Mathf.DeltaAngle(this.followAngles.x, num2), this.followAngles.y + Mathf.DeltaAngle(this.followAngles.y, num));
		this.followAngles = Vector3.SmoothDamp(this.followAngles, target, ref this.followVelocity, this.followSpeed);
		base.transform.localRotation = this.originalRotation * Quaternion.Euler(-this.followAngles.x, this.followAngles.y, 0f);
	}
}
