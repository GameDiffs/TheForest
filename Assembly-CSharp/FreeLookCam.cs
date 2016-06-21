using System;
using UnityEngine;

public class FreeLookCam : AbstractTargetFollower
{
	private const float LookDistance = 100f;

	[SerializeField]
	private float moveSpeed = 1f;

	[Range(0f, 10f), SerializeField]
	private float turnSpeed = 1.5f;

	[SerializeField]
	private float turnSmoothing = 0.1f;

	[SerializeField]
	private float tiltMax = 75f;

	[SerializeField]
	private float tiltMin = 45f;

	[SerializeField]
	private bool lockCursor;

	private float lookAngle;

	private float tiltAngle;

	private Transform pivot;

	private ThirdPersonCharacter character;

	private float smoothX;

	private float smoothY;

	private float smoothXvelocity;

	private float smoothYvelocity;

	private void Awake()
	{
		Cursor.lockState = ((!this.lockCursor) ? CursorLockMode.None : CursorLockMode.Locked);
		Cursor.visible = !this.lockCursor;
		this.pivot = base.transform.GetChild(0);
		Debug.Log("Cam Test");
	}

	private void Update()
	{
		this.HandleRotationMovement();
	}

	protected override void FollowTarget(float deltaTime)
	{
		base.transform.position = Vector3.Lerp(base.transform.position, this.target.position, deltaTime * this.moveSpeed);
	}

	private void HandleRotationMovement()
	{
		float axis = CrossPlatformInput.GetAxis("Mouse X");
		float axis2 = CrossPlatformInput.GetAxis("Mouse Y");
		if (this.turnSmoothing > 0f)
		{
			this.smoothX = Mathf.SmoothDamp(this.smoothX, axis, ref this.smoothXvelocity, this.turnSmoothing);
			this.smoothY = Mathf.SmoothDamp(this.smoothY, axis2, ref this.smoothYvelocity, this.turnSmoothing);
		}
		else
		{
			this.smoothX = axis;
			this.smoothY = axis2;
		}
		this.lookAngle += this.smoothX * this.turnSpeed;
		base.transform.rotation = Quaternion.Euler(0f, this.lookAngle, 0f);
		this.tiltAngle -= this.smoothY * this.turnSpeed;
		this.tiltAngle = Mathf.Clamp(this.tiltAngle, -this.tiltMin, this.tiltMax);
		this.pivot.localRotation = Quaternion.Euler(this.tiltAngle, 0f, 0f);
	}
}
