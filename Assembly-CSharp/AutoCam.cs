using System;
using UnityEngine;

public class AutoCam : AbstractTargetFollower
{
	[SerializeField]
	private float moveSpeed = 3f;

	[SerializeField]
	private float turnSpeed = 1f;

	[SerializeField]
	private float rollSpeed = 0.2f;

	[SerializeField]
	private bool followVelocity;

	[SerializeField]
	private bool followTilt = true;

	[SerializeField]
	private float spinTurnLimit = 90f;

	[SerializeField]
	private float targetVelocityLowerLimit = 4f;

	[SerializeField]
	private float smoothTurnTime = 0.2f;

	private float lastFlatAngle;

	private float currentTurnAmount;

	private float turnSpeedVelocityChange;

	private Vector3 rollUp = Vector3.up;

	protected override void Start()
	{
		base.Start();
	}

	protected override void FollowTarget(float deltaTime)
	{
		if (deltaTime <= 0f || this.target == null)
		{
			return;
		}
		Vector3 forward = this.target.forward;
		Vector3 up = this.target.up;
		if (this.followVelocity)
		{
			if (this.target.GetComponent<Rigidbody>().velocity.magnitude > this.targetVelocityLowerLimit)
			{
				forward = this.target.GetComponent<Rigidbody>().velocity.normalized;
				up = Vector3.up;
			}
			else
			{
				up = Vector3.up;
			}
			this.currentTurnAmount = Mathf.SmoothDamp(this.currentTurnAmount, 1f, ref this.turnSpeedVelocityChange, this.smoothTurnTime);
		}
		else
		{
			float target = Mathf.Atan2(forward.x, forward.z) * 57.29578f;
			if (this.spinTurnLimit > 0f)
			{
				float value = Mathf.Abs(Mathf.DeltaAngle(this.lastFlatAngle, target)) / deltaTime;
				float num = Mathf.InverseLerp(this.spinTurnLimit, this.spinTurnLimit * 0.75f, value);
				float smoothTime = (this.currentTurnAmount <= num) ? 1f : 0.1f;
				this.currentTurnAmount = Mathf.SmoothDamp(this.currentTurnAmount, num, ref this.turnSpeedVelocityChange, smoothTime);
			}
			else
			{
				this.currentTurnAmount = 1f;
			}
			this.lastFlatAngle = target;
		}
		base.transform.position = Vector3.Lerp(base.transform.position, this.target.position, deltaTime * this.moveSpeed);
		if (!this.followTilt)
		{
			forward.y = 0f;
			if (forward.sqrMagnitude < 1.401298E-45f)
			{
				forward = base.transform.forward;
			}
		}
		Quaternion to = Quaternion.LookRotation(forward, this.rollUp);
		this.rollUp = ((this.rollSpeed <= 0f) ? Vector3.up : Vector3.Slerp(this.rollUp, up, this.rollSpeed * Time.deltaTime));
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, to, this.turnSpeed * this.currentTurnAmount * Time.smoothDeltaTime);
	}
}
