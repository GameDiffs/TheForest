using System;
using System.Collections;
using UnityEngine;

public class ThirdPersonCharacter : MonoBehaviour
{
	[Serializable]
	public class AdvancedSettings
	{
		public float stationaryTurnSpeed = 180f;

		public float movingTurnSpeed = 360f;

		public float headLookResponseSpeed = 2f;

		public float crouchHeightFactor = 0.6f;

		public float crouchChangeSpeed = 4f;

		public float autoTurnThresholdAngle = 100f;

		public float autoTurnSpeed = 2f;

		public PhysicMaterial zeroFrictionMaterial;

		public PhysicMaterial highFrictionMaterial;

		public float jumpRepeatDelayTime = 0.25f;

		public float runCycleLegOffset = 0.2f;

		public float groundStickyEffect = 5f;
	}

	private class RayHitComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			return ((RaycastHit)x).distance.CompareTo(((RaycastHit)y).distance);
		}
	}

	private const float half = 0.5f;

	[SerializeField]
	private float jumpPower = 12f;

	[SerializeField]
	private float airSpeed = 6f;

	[SerializeField]
	private float airControl = 2f;

	[Range(1f, 4f), SerializeField]
	public float gravityMultiplier = 2f;

	[Range(0.1f, 3f), SerializeField]
	private float moveSpeedMultiplier = 1f;

	[Range(0.1f, 3f), SerializeField]
	private float animSpeedMultiplier = 1f;

	[SerializeField]
	private ThirdPersonCharacter.AdvancedSettings advancedSettings;

	private bool onGround;

	private Vector3 lookPos;

	private float originalHeight;

	private Animator animator;

	private float lastAirTime;

	private CapsuleCollider capsule;

	private Vector3 moveInput;

	private bool crouchInput;

	private bool jumpInput;

	private float turnAmount;

	private float forwardAmount;

	private Vector3 velocity;

	public Transform lookTarget
	{
		get;
		set;
	}

	private void Start()
	{
		this.animator = base.GetComponentInChildren<Animator>();
		this.capsule = (base.GetComponent<Collider>() as CapsuleCollider);
		if (this.capsule != null)
		{
			this.originalHeight = this.capsule.height;
			this.capsule.center = Vector3.up * this.originalHeight * 0.5f;
		}
		else
		{
			Debug.LogError(" collider cannot be cast to CapsuleCollider");
		}
		this.SetUpAnimator();
	}

	public void Move(Vector3 move, bool crouch, bool jump, Vector3 lookPos)
	{
		this.moveInput = move;
		this.crouchInput = crouch;
		this.jumpInput = jump;
		this.lookPos = lookPos;
		this.velocity = base.GetComponent<Rigidbody>().velocity;
		this.ConvertMoveInput();
		this.TurnTowardsCameraForward();
		this.PreventStandingInLowHeadroom();
		this.ScaleCapsuleForCrouching();
		this.ApplyExtraTurnRotation();
		this.GroundCheck();
		this.SetFriction();
		if (this.onGround)
		{
			this.HandleGroundedVelocities();
		}
		else
		{
			this.HandleAirborneVelocities();
		}
		this.UpdateAnimator();
		base.GetComponent<Rigidbody>().velocity = this.velocity;
	}

	private void ConvertMoveInput()
	{
		Vector3 vector = base.transform.InverseTransformDirection(this.moveInput);
		this.turnAmount = Mathf.Atan2(vector.x, vector.z);
		this.forwardAmount = vector.z;
	}

	private void TurnTowardsCameraForward()
	{
		if (Mathf.Abs(this.forwardAmount) < 0.01f)
		{
			Vector3 vector = base.transform.InverseTransformDirection(this.lookPos - base.transform.position);
			float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
			if (Mathf.Abs(num) > this.advancedSettings.autoTurnThresholdAngle)
			{
				this.turnAmount += num * this.advancedSettings.autoTurnSpeed * 0.001f;
			}
		}
	}

	private void PreventStandingInLowHeadroom()
	{
		if (!this.crouchInput)
		{
			Ray ray = new Ray(base.GetComponent<Rigidbody>().position + Vector3.up * this.capsule.radius * 0.5f, Vector3.up);
			float maxDistance = this.originalHeight - this.capsule.radius * 0.5f;
			if (Physics.SphereCast(ray, this.capsule.radius * 0.5f, maxDistance))
			{
				this.crouchInput = true;
			}
		}
	}

	private void ScaleCapsuleForCrouching()
	{
		if (this.onGround && this.crouchInput && this.capsule.height != this.originalHeight * this.advancedSettings.crouchHeightFactor)
		{
			this.capsule.height = Mathf.MoveTowards(this.capsule.height, this.originalHeight * this.advancedSettings.crouchHeightFactor, Time.deltaTime * 4f);
			this.capsule.center = Vector3.MoveTowards(this.capsule.center, Vector3.up * this.originalHeight * this.advancedSettings.crouchHeightFactor * 0.5f, Time.deltaTime * 2f);
		}
		else if (this.capsule.height != this.originalHeight && this.capsule.center != Vector3.up * this.originalHeight * 0.5f)
		{
			this.capsule.height = Mathf.MoveTowards(this.capsule.height, this.originalHeight, Time.deltaTime * 4f);
			this.capsule.center = Vector3.MoveTowards(this.capsule.center, Vector3.up * this.originalHeight * 0.5f, Time.deltaTime * 2f);
		}
	}

	private void ApplyExtraTurnRotation()
	{
		float num = Mathf.Lerp(this.advancedSettings.stationaryTurnSpeed, this.advancedSettings.movingTurnSpeed, this.forwardAmount);
		base.transform.Rotate(0f, this.turnAmount * num * Time.deltaTime, 0f);
	}

	private void GroundCheck()
	{
		Ray ray = new Ray(base.transform.position + Vector3.up * 0.1f, -Vector3.up);
		RaycastHit[] array = Physics.RaycastAll(ray, 0.5f);
		Array.Sort(array, new ThirdPersonCharacter.RayHitComparer());
		if (this.velocity.y < this.jumpPower * 0.5f)
		{
			this.onGround = false;
			base.GetComponent<Rigidbody>().useGravity = true;
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				if (!raycastHit.collider.isTrigger)
				{
					if (this.velocity.y <= 0f)
					{
						base.GetComponent<Rigidbody>().position = Vector3.MoveTowards(base.GetComponent<Rigidbody>().position, raycastHit.point, Time.deltaTime * this.advancedSettings.groundStickyEffect);
					}
					this.onGround = true;
					base.GetComponent<Rigidbody>().useGravity = false;
					break;
				}
			}
		}
		if (!this.onGround)
		{
			this.lastAirTime = Time.time;
		}
	}

	private void SetFriction()
	{
		if (this.onGround)
		{
			if (this.moveInput.magnitude == 0f)
			{
				base.GetComponent<Collider>().material = this.advancedSettings.highFrictionMaterial;
			}
			else
			{
				base.GetComponent<Collider>().material = this.advancedSettings.zeroFrictionMaterial;
			}
		}
		else
		{
			base.GetComponent<Collider>().material = this.advancedSettings.zeroFrictionMaterial;
		}
	}

	private void HandleGroundedVelocities()
	{
		this.velocity.y = 0f;
		if (this.moveInput.magnitude == 0f)
		{
			this.velocity.x = 0f;
			this.velocity.z = 0f;
		}
		bool flag = this.animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded");
		bool flag2 = Time.time > this.lastAirTime + this.advancedSettings.jumpRepeatDelayTime;
		if (this.jumpInput && !this.crouchInput && flag2 && flag)
		{
			this.onGround = false;
			this.velocity = this.moveInput * this.airSpeed;
			this.velocity.y = this.jumpPower;
		}
	}

	private void HandleAirborneVelocities()
	{
		Vector3 to = new Vector3(this.moveInput.x * this.airSpeed, this.velocity.y, this.moveInput.z * this.airSpeed);
		this.velocity = Vector3.Lerp(this.velocity, to, Time.deltaTime * this.airControl);
		base.GetComponent<Rigidbody>().useGravity = true;
		Vector3 force = Physics.gravity * this.gravityMultiplier - Physics.gravity;
		base.GetComponent<Rigidbody>().AddForce(force);
	}

	private void UpdateAnimator()
	{
		this.animator.applyRootMotion = this.onGround;
		this.lookPos = Vector3.Lerp(this.lookPos, this.lookPos, Time.deltaTime * this.advancedSettings.headLookResponseSpeed);
		this.animator.SetFloatReflected("Forward", this.forwardAmount, 0.1f, Time.deltaTime);
		this.animator.SetFloatReflected("Turn", this.turnAmount, 0.1f, Time.deltaTime);
		this.animator.SetBoolReflected("Crouch", this.crouchInput);
		this.animator.SetBoolReflected("OnGround", this.onGround);
		if (!this.onGround)
		{
			this.animator.SetFloatReflected("Jump", this.velocity.y);
		}
		float num = Mathf.Repeat(this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime + this.advancedSettings.runCycleLegOffset, 1f);
		float value = (float)((num >= 0.5f) ? -1 : 1) * this.forwardAmount;
		if (this.onGround)
		{
			this.animator.SetFloatReflected("JumpLeg", value);
		}
		if (this.onGround && this.moveInput.magnitude > 0f)
		{
			this.animator.speed = this.animSpeedMultiplier;
		}
		else
		{
			this.animator.speed = 1f;
		}
	}

	private void OnAnimatorIK(int layerIndex)
	{
		this.animator.SetLookAtWeight(1f, 0.2f, 2.5f);
		if (this.lookTarget != null)
		{
			this.lookPos = this.lookTarget.position;
		}
		this.animator.SetLookAtPosition(this.lookPos);
	}

	private void SetUpAnimator()
	{
		this.animator = base.GetComponent<Animator>();
		Animator[] componentsInChildren = base.GetComponentsInChildren<Animator>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Animator animator = componentsInChildren[i];
			if (animator != this.animator)
			{
				this.animator.avatar = animator.avatar;
				UnityEngine.Object.Destroy(animator);
				break;
			}
		}
	}

	public void OnAnimatorMove()
	{
		base.GetComponent<Rigidbody>().rotation = this.animator.rootRotation;
		if (this.onGround && Time.deltaTime > 0f)
		{
			Vector3 vector = this.animator.deltaPosition * this.moveSpeedMultiplier / Time.deltaTime;
			vector.y = base.GetComponent<Rigidbody>().velocity.y;
			base.GetComponent<Rigidbody>().velocity = vector;
		}
	}
}
