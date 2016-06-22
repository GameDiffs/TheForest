using Boo.Lang;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("Character/Character Motor"), RequireComponent(typeof(CharacterController))]
[Serializable]
public class CharacterMotor : MonoBehaviour
{
	[CompilerGenerated]
	[Serializable]
	internal sealed class $SubtractNewPlatformVelocity$8 : GenericGenerator<object>
	{
		internal CharacterMotor $self_$11;

		public $SubtractNewPlatformVelocity$8(CharacterMotor self_)
		{
			this.$self_$11 = self_;
		}

		public override IEnumerator<object> GetEnumerator()
		{
			return new CharacterMotor.$SubtractNewPlatformVelocity$8.$(this.$self_$11);
		}
	}

	public bool canControl;

	public bool useFixedUpdate;

	[NonSerialized]
	public Vector3 inputMoveDirection;

	[NonSerialized]
	public bool inputJump;

	public CharacterMotorMovement movement;

	public CharacterMotorJumping jumping;

	public CharacterMotorMovingPlatform movingPlatform;

	public CharacterMotorSliding sliding;

	[NonSerialized]
	public bool grounded;

	[NonSerialized]
	public Vector3 groundNormal;

	private Vector3 lastGroundNormal;

	private Transform tr;

	private CharacterController controller;

	public CharacterMotor()
	{
		this.canControl = true;
		this.useFixedUpdate = true;
		this.inputMoveDirection = Vector3.zero;
		this.movement = new CharacterMotorMovement();
		this.jumping = new CharacterMotorJumping();
		this.movingPlatform = new CharacterMotorMovingPlatform();
		this.sliding = new CharacterMotorSliding();
		this.grounded = true;
		this.groundNormal = Vector3.zero;
		this.lastGroundNormal = Vector3.zero;
	}

	public override void Awake()
	{
		this.controller = (CharacterController)this.GetComponent(typeof(CharacterController));
		this.tr = this.transform;
	}

	private void UpdateFunction()
	{
		Vector3 vector = this.movement.velocity;
		vector = this.ApplyInputVelocityChange(vector);
		vector = this.ApplyGravityAndJumping(vector);
		Vector3 vector2 = Vector3.zero;
		if (this.MoveWithPlatform())
		{
			Vector3 a = this.movingPlatform.activePlatform.TransformPoint(this.movingPlatform.activeLocalPoint);
			vector2 = a - this.movingPlatform.activeGlobalPoint;
			if (vector2 != Vector3.zero)
			{
				this.controller.Move(vector2);
			}
			Quaternion lhs = this.movingPlatform.activePlatform.rotation * this.movingPlatform.activeLocalRotation;
			float y = (lhs * Quaternion.Inverse(this.movingPlatform.activeGlobalRotation)).eulerAngles.y;
			if (y != (float)0)
			{
				this.tr.Rotate((float)0, y, (float)0);
			}
		}
		Vector3 position = this.tr.position;
		Vector3 vector3 = vector * Time.deltaTime;
		float d = Mathf.Max(this.controller.stepOffset, new Vector3(vector3.x, (float)0, vector3.z).magnitude);
		if (this.grounded)
		{
			vector3 -= d * Vector3.up;
		}
		this.movingPlatform.hitPlatform = null;
		this.groundNormal = Vector3.zero;
		this.movement.collisionFlags = this.controller.Move(vector3);
		this.movement.lastHitPoint = this.movement.hitPoint;
		this.lastGroundNormal = this.groundNormal;
		if (this.movingPlatform.enabled && this.movingPlatform.activePlatform != this.movingPlatform.hitPlatform && this.movingPlatform.hitPlatform != null)
		{
			this.movingPlatform.activePlatform = this.movingPlatform.hitPlatform;
			this.movingPlatform.lastMatrix = this.movingPlatform.hitPlatform.localToWorldMatrix;
			this.movingPlatform.newPlatform = true;
		}
		Vector3 vector4 = new Vector3(vector.x, (float)0, vector.z);
		this.movement.velocity = (this.tr.position - position) / Time.deltaTime;
		Vector3 lhs2 = new Vector3(this.movement.velocity.x, (float)0, this.movement.velocity.z);
		if (vector4 == Vector3.zero)
		{
			this.movement.velocity = new Vector3((float)0, this.movement.velocity.y, (float)0);
		}
		else
		{
			float value = Vector3.Dot(lhs2, vector4) / vector4.sqrMagnitude;
			this.movement.velocity = vector4 * Mathf.Clamp01(value) + this.movement.velocity.y * Vector3.up;
		}
		if (this.movement.velocity.y < vector.y - 0.001f)
		{
			if (this.movement.velocity.y < (float)0)
			{
				this.movement.velocity.y = vector.y;
			}
			else
			{
				this.jumping.holdingJumpButton = false;
			}
		}
		if (this.grounded && !this.IsGroundedTest())
		{
			this.grounded = false;
			if (this.movingPlatform.enabled && (this.movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer || this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer))
			{
				this.movement.frameVelocity = this.movingPlatform.platformVelocity;
				this.movement.velocity = this.movement.velocity + this.movingPlatform.platformVelocity;
			}
			this.SendMessage("OnFall", SendMessageOptions.DontRequireReceiver);
			this.tr.position = this.tr.position + d * Vector3.up;
		}
		else if (!this.grounded && this.IsGroundedTest())
		{
			this.grounded = true;
			this.jumping.jumping = false;
			this.StartCoroutine_Auto(this.SubtractNewPlatformVelocity());
			this.SendMessage("OnLand", SendMessageOptions.DontRequireReceiver);
		}
		if (this.MoveWithPlatform())
		{
			this.movingPlatform.activeGlobalPoint = this.tr.position + Vector3.up * (this.controller.center.y - this.controller.height * 0.5f + this.controller.radius);
			this.movingPlatform.activeLocalPoint = this.movingPlatform.activePlatform.InverseTransformPoint(this.movingPlatform.activeGlobalPoint);
			this.movingPlatform.activeGlobalRotation = this.tr.rotation;
			this.movingPlatform.activeLocalRotation = Quaternion.Inverse(this.movingPlatform.activePlatform.rotation) * this.movingPlatform.activeGlobalRotation;
		}
	}

	public override void FixedUpdate()
	{
		if (this.movingPlatform.enabled)
		{
			if (this.movingPlatform.activePlatform != null)
			{
				if (!this.movingPlatform.newPlatform)
				{
					Vector3 platformVelocity = this.movingPlatform.platformVelocity;
					this.movingPlatform.platformVelocity = (this.movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(this.movingPlatform.activeLocalPoint) - this.movingPlatform.lastMatrix.MultiplyPoint3x4(this.movingPlatform.activeLocalPoint)) / Time.deltaTime;
				}
				this.movingPlatform.lastMatrix = this.movingPlatform.activePlatform.localToWorldMatrix;
				this.movingPlatform.newPlatform = false;
			}
			else
			{
				this.movingPlatform.platformVelocity = Vector3.zero;
			}
		}
		if (this.useFixedUpdate)
		{
			this.UpdateFunction();
		}
	}

	public override void Update()
	{
		if (!this.useFixedUpdate)
		{
			this.UpdateFunction();
		}
	}

	private Vector3 ApplyInputVelocityChange(Vector3 velocity)
	{
		if (!this.canControl)
		{
			this.inputMoveDirection = Vector3.zero;
		}
		Vector3 vector = default(Vector3);
		if (this.grounded && this.TooSteep())
		{
			vector = new Vector3(this.groundNormal.x, (float)0, this.groundNormal.z).normalized;
			Vector3 vector2 = Vector3.Project(this.inputMoveDirection, vector);
			vector = vector + vector2 * this.sliding.speedControl + (this.inputMoveDirection - vector2) * this.sliding.sidewaysControl;
			vector *= this.sliding.slidingSpeed;
		}
		else
		{
			vector = this.GetDesiredHorizontalVelocity();
		}
		if (this.movingPlatform.enabled && this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
		{
			vector += this.movement.frameVelocity;
			vector.y = (float)0;
		}
		if (this.grounded)
		{
			vector = this.AdjustGroundVelocityToNormal(vector, this.groundNormal);
		}
		else
		{
			velocity.y = (float)0;
		}
		float num = this.GetMaxAcceleration(this.grounded) * Time.deltaTime;
		Vector3 b = vector - velocity;
		if (b.sqrMagnitude > num * num)
		{
			b = b.normalized * num;
		}
		if (this.grounded || this.canControl)
		{
			velocity += b;
		}
		if (this.grounded)
		{
			velocity.y = Mathf.Min(velocity.y, (float)0);
		}
		return velocity;
	}

	private Vector3 ApplyGravityAndJumping(Vector3 velocity)
	{
		if (!this.inputJump || !this.canControl)
		{
			this.jumping.holdingJumpButton = false;
			this.jumping.lastButtonDownTime = (float)-100;
		}
		if (this.inputJump && this.jumping.lastButtonDownTime < (float)0 && this.canControl)
		{
			this.jumping.lastButtonDownTime = Time.time;
		}
		if (this.grounded)
		{
			velocity.y = Mathf.Min((float)0, velocity.y) - this.movement.gravity * Time.deltaTime;
		}
		else
		{
			velocity.y = this.movement.velocity.y - this.movement.gravity * Time.deltaTime;
			if (this.jumping.jumping && this.jumping.holdingJumpButton && Time.time < this.jumping.lastStartTime + this.jumping.extraHeight / this.CalculateJumpVerticalSpeed(this.jumping.baseHeight))
			{
				velocity += this.jumping.jumpDir * this.movement.gravity * Time.deltaTime;
			}
			velocity.y = Mathf.Max(velocity.y, -this.movement.maxFallSpeed);
		}
		if (this.grounded)
		{
			if (this.jumping.enabled && this.canControl && Time.time - this.jumping.lastButtonDownTime < 0.2f)
			{
				this.grounded = false;
				this.jumping.jumping = true;
				this.jumping.lastStartTime = Time.time;
				this.jumping.lastButtonDownTime = (float)-100;
				this.jumping.holdingJumpButton = true;
				if (this.TooSteep())
				{
					this.jumping.jumpDir = Vector3.Slerp(Vector3.up, this.groundNormal, this.jumping.steepPerpAmount);
				}
				else
				{
					this.jumping.jumpDir = Vector3.Slerp(Vector3.up, this.groundNormal, this.jumping.perpAmount);
				}
				velocity.y = (float)0;
				velocity += this.jumping.jumpDir * this.CalculateJumpVerticalSpeed(this.jumping.baseHeight);
				if (this.movingPlatform.enabled && (this.movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer || this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer))
				{
					this.movement.frameVelocity = this.movingPlatform.platformVelocity;
					velocity += this.movingPlatform.platformVelocity;
				}
				this.SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				this.jumping.holdingJumpButton = false;
			}
		}
		return velocity;
	}

	public override void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.normal.y > (float)0 && hit.normal.y > this.groundNormal.y && hit.moveDirection.y < (float)0)
		{
			if ((hit.point - this.movement.lastHitPoint).sqrMagnitude > 0.001f || this.lastGroundNormal == Vector3.zero)
			{
				this.groundNormal = hit.normal;
			}
			else
			{
				this.groundNormal = this.lastGroundNormal;
			}
			this.movingPlatform.hitPlatform = hit.collider.transform;
			this.movement.hitPoint = hit.point;
			this.movement.frameVelocity = Vector3.zero;
		}
	}

	private IEnumerator SubtractNewPlatformVelocity()
	{
		return new CharacterMotor.$SubtractNewPlatformVelocity$8(this).GetEnumerator();
	}

	private bool MoveWithPlatform()
	{
		bool arg_2D_0;
		if (arg_2D_0 = this.movingPlatform.enabled)
		{
			arg_2D_0 = (this.grounded ?? (this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaLocked));
		}
		bool arg_45_0;
		if (arg_45_0 = arg_2D_0)
		{
			arg_45_0 = (this.movingPlatform.activePlatform != null);
		}
		return arg_45_0;
	}

	private Vector3 GetDesiredHorizontalVelocity()
	{
		Vector3 vector = this.tr.InverseTransformDirection(this.inputMoveDirection);
		float num = this.MaxSpeedInDirection(vector);
		if (this.grounded)
		{
			float time = Mathf.Asin(this.movement.velocity.normalized.y) * 57.29578f;
			num *= this.movement.slopeSpeedMultiplier.Evaluate(time);
		}
		return this.tr.TransformDirection(vector * num);
	}

	private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
	{
		Vector3 lhs = Vector3.Cross(Vector3.up, hVelocity);
		return Vector3.Cross(lhs, groundNormal).normalized * hVelocity.magnitude;
	}

	private bool IsGroundedTest()
	{
		return this.groundNormal.y > 0.01f;
	}

	public override float GetMaxAcceleration(bool grounded)
	{
		return (!grounded) ? this.movement.maxAirAcceleration : this.movement.maxGroundAcceleration;
	}

	public override float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		return Mathf.Sqrt((float)2 * targetJumpHeight * this.movement.gravity);
	}

	public override bool IsJumping()
	{
		return this.jumping.jumping;
	}

	public override bool IsSliding()
	{
		bool arg_18_0;
		if (arg_18_0 = this.grounded)
		{
			arg_18_0 = this.sliding.enabled;
		}
		bool arg_25_0;
		if (arg_25_0 = arg_18_0)
		{
			arg_25_0 = this.TooSteep();
		}
		return arg_25_0;
	}

	public override bool IsTouchingCeiling()
	{
		return (this.movement.collisionFlags & CollisionFlags.Above) != CollisionFlags.None;
	}

	public override bool IsGrounded()
	{
		return this.grounded;
	}

	public override bool TooSteep()
	{
		return this.groundNormal.y <= Mathf.Cos(this.controller.slopeLimit * 0.0174532924f);
	}

	public override Vector3 GetDirection()
	{
		return this.inputMoveDirection;
	}

	public override void SetControllable(bool controllable)
	{
		this.canControl = controllable;
	}

	public override float MaxSpeedInDirection(Vector3 desiredMovementDirection)
	{
		float arg_AC_0;
		if (desiredMovementDirection == Vector3.zero)
		{
			arg_AC_0 = (float)0;
		}
		else
		{
			float num = ((desiredMovementDirection.z <= (float)0) ? this.movement.maxBackwardsSpeed : this.movement.maxForwardSpeed) / this.movement.maxSidewaysSpeed;
			Vector3 normalized = new Vector3(desiredMovementDirection.x, (float)0, desiredMovementDirection.z / num).normalized;
			float num2 = new Vector3(normalized.x, (float)0, normalized.z * num).magnitude * this.movement.maxSidewaysSpeed;
			arg_AC_0 = num2;
		}
		return arg_AC_0;
	}

	public override void SetVelocity(Vector3 velocity)
	{
		this.grounded = false;
		this.movement.velocity = velocity;
		this.movement.frameVelocity = Vector3.zero;
		this.SendMessage("OnExternalVelocity");
	}

	public override void Main()
	{
	}
}
