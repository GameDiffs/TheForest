using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;
using UnitySampleAssets.Characters.FirstPerson;

namespace TheForest.Player
{
	[DoNotSerializePublic, RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
	public class FirstPersonCharacter : MonoBehaviour
	{
		[Serializable]
		public class MovementSettings
		{
			public float ForwardSpeed = 8f;

			[HideInInspector]
			public float StrafeSpeed = 4f;

			public float SprintSpeed = 10f;

			public float CarrySpeedRatio = 0.75f;

			public float SwimmingSpeed = 3f;

			public float CrouchSpeed = 2f;

			public float JumpForce = 30f;

			public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(-90f, 1f),
				new Keyframe(0f, 1f),
				new Keyframe(90f, 0f)
			});

			[HideInInspector]
			public float CurrentTargetSpeed = 8f;

			public bool Running;

			public void UpdateDesiredTargetSpeed(bool grounded, bool swimming, bool crouching, bool carryHeavy)
			{
				bool running = this.Running;
				this.Running = false;
				if (swimming && !grounded)
				{
					this.CurrentTargetSpeed = this.SwimmingSpeed;
				}
				else if (crouching && grounded)
				{
					this.CurrentTargetSpeed = this.CrouchSpeed;
				}
				else
				{
					bool button = TheForest.Utils.Input.GetButton("Run");
					if (button && !crouching && LocalPlayer.Stats.Stamina > 10f)
					{
						this.CurrentTargetSpeed = this.SprintSpeed;
						this.Running = true;
					}
					else if (!crouching)
					{
						this.CurrentTargetSpeed = this.ForwardSpeed;
					}
				}
				if (carryHeavy)
				{
					this.CurrentTargetSpeed *= this.CarrySpeedRatio;
				}
			}
		}

		[Serializable]
		public class AdvancedSettings
		{
			public float groundCheckDistance = 0.01f;

			public float stickToGroundHelperDistance = 0.5f;

			[Range(0f, 1f)]
			public float slowDownRate = 0.75f;

			public bool airControl;

			public float groundedDrag = 5f;

			public float swimmingDrag = 2f;
		}

		private const float WET_GROUND_END = 1f;

		private const float IMMERSION_START = 5f;

		private const float IMMERSION_END = 10f;

		private const float IMMERSION_RANGE = 5f;

		public LayerMask CollisionLayers;

		public FirstPersonCharacter.MovementSettings movementSettings = new FirstPersonCharacter.MovementSettings();

		public UnitySampleAssets.Characters.FirstPerson.MouseLook mouseLook = new UnitySampleAssets.Characters.FirstPerson.MouseLook();

		public FirstPersonCharacter.AdvancedSettings advancedSettings = new FirstPersonCharacter.AdvancedSettings();

		public Buoyancy buoyancy;

		public Transform Inventory;

		public Transform SurvivalBook;

		public Animator animator;

		public SphereCollider HeadBlock;

		public float sprintStaminaPerSecond = 10f;

		public float crouchHeight = 1f;

		public float gravity = 10f;

		public float fallDamageVelocityThreshold = 20f;

		public int antiStuckFramesThreshold = 10;

		private Vector2 _input;

		private FsmBool _fsmCrouchBool;

		private Rigidbody _rigidbody;

		private CapsuleCollider _capsule;

		private float _yRotation;

		private Vector3 _groundContactNormal;

		private bool _isGroundContact;

		private bool _isGrounded;

		private bool _previouslyGrounded;

		private bool _isStraffing;

		private bool _isSwiming;

		private bool _jump;

		private bool _jumping;

		private bool _crouch;

		private bool _crouching;

		private bool _standUp;

		private bool _standingUp;

		private bool _pushingSled;

		private bool _sailingRaft;

		private bool _allowFallDamage;

		private float _capsuleOriginalHeight;

		private float _crouchCapsuleCenter;

		private float _lastFrameYPosition;

		private int _antiStuckFramesCount;

		private float dot;

		public float _fallMassRatio = 0.25f;

		private float tempGrav;

		private float normVal;

		public bool Locked
		{
			get;
			set;
		}

		public Vector3 Velocity
		{
			get
			{
				return this._rigidbody.velocity;
			}
		}

		public bool Grounded
		{
			get
			{
				return this._isGrounded;
			}
		}

		public bool CanJump
		{
			get;
			set;
		}

		public bool Running
		{
			get
			{
				return this.movementSettings.Running;
			}
		}

		public bool Jumping
		{
			get
			{
				return this._jumping;
			}
		}

		public bool Crouching
		{
			get
			{
				return this._crouching;
			}
		}

		public bool Sitting
		{
			get;
			set;
		}

		public bool Climbing
		{
			get;
			set;
		}

		public bool Swimming
		{
			get
			{
				return this.buoyancy.InWater;
			}
		}

		public bool Diving
		{
			get;
			set;
		}

		public bool PushingSled
		{
			get
			{
				return this._pushingSled;
			}
		}

		public bool SailingRaft
		{
			get
			{
				return this._sailingRaft;
			}
		}

		public bool CarryHeavy
		{
			get
			{
				return LocalPlayer.AnimControl.carry || LocalPlayer.Inventory.Logs.HasLogs;
			}
		}

		public float walkSpeed
		{
			get
			{
				return this.movementSettings.ForwardSpeed;
			}
			set
			{
				this.movementSettings.ForwardSpeed = value;
			}
		}

		public float runSpeed
		{
			get
			{
				return this.movementSettings.SprintSpeed;
			}
			set
			{
				this.movementSettings.SprintSpeed = value;
			}
		}

		public float strafeSpeed
		{
			get
			{
				return this.movementSettings.StrafeSpeed;
			}
			set
			{
				this.movementSettings.StrafeSpeed = value;
			}
		}

		private void Awake()
		{
			this.CanJump = true;
			this._rigidbody = base.GetComponent<Rigidbody>();
			this._rigidbody.freezeRotation = true;
			this._rigidbody.useGravity = false;
			this.mouseLook.Init(base.transform, LocalPlayer.MainCamTr);
			this.UnLockView();
			this._capsule = base.GetComponent<CapsuleCollider>();
			this._capsuleOriginalHeight = this._capsule.height;
			this._crouchCapsuleCenter = (this.crouchHeight - this._capsuleOriginalHeight) / 2f;
			this._fsmCrouchBool = FsmVariables.GlobalVariables.GetFsmBool("playerCrouchBool");
			this._isGrounded = true;
		}

		private void Start()
		{
			this.UnLockView();
		}

		private void OnEnable()
		{
			this.ResetRotations();
		}

		private void OnApplicationFocus(bool focused)
		{
			if (focused && !this.Locked)
			{
				this.UnLockView();
			}
		}

		private void Update()
		{
			if (!this.Locked)
			{
				if (!this._pushingSled && !this.Sitting)
				{
					this.RotateView();
				}
				this._isSwiming = this.Swimming;
				if (!this._sailingRaft && !this._pushingSled && (!this.Swimming || this._isGrounded))
				{
					if (!this._crouch)
					{
						this._crouch = TheForest.Utils.Input.GetButtonDown("Crouch");
					}
					else
					{
						this._standUp = !TheForest.Utils.Input.GetButton("Crouch");
					}
					if (TheForest.Utils.Input.GetButtonDown("Jump") && !this._jump && this.CanJump)
					{
						this._jump = true;
						this._standUp = true;
						LocalPlayer.Sfx.PlayJumpSound();
					}
				}
				if (LocalPlayer.CamRotator.enabled != (this._pushingSled || this.Sitting || this.Climbing))
				{
					if (LocalPlayer.CamRotator.enabled)
					{
						this.ResetRotations();
					}
					LocalPlayer.CamRotator.enabled = !LocalPlayer.CamRotator.enabled;
				}
				LocalPlayer.MainRotator.enabled = false;
			}
		}

		private void FixedUpdate()
		{
			if (!this.Locked && !this._sailingRaft)
			{
				this.GroundCheck();
				this.AntiStuckCheck();
				this.UpdateInput();
				if (this._jumping && this._rigidbody.velocity.y <= 0f)
				{
					this._jumping = false;
				}
				if (Mathf.Abs(this._input.x) > 1.401298E-45f || Mathf.Abs(this._input.y) > 1.401298E-45f)
				{
					Vector3 vector;
					if (LocalPlayer.MainCamTr.localRotation.eulerAngles.x > 0f && !this.Diving)
					{
						vector = LocalPlayer.Transform.forward * this._input.y;
					}
					else
					{
						vector = LocalPlayer.MainCamTr.transform.forward * this._input.y;
					}
					if (!this._pushingSled)
					{
						vector += LocalPlayer.Transform.right * this._input.x * 1.2f;
					}
					this.dot = Vector3.Dot(vector, this._groundContactNormal);
					bool flag;
					if (this.dot > -0.95f && this.dot < 0.95f)
					{
						vector = Vector3.ProjectOnPlane(vector, this._groundContactNormal);
						flag = true;
					}
					else
					{
						flag = false;
					}
					vector.Normalize();
					vector.x *= this.movementSettings.CurrentTargetSpeed;
					vector.z *= this.movementSettings.CurrentTargetSpeed;
					vector.y = Mathf.Abs(this._rigidbody.velocity.y) + vector.y * this.movementSettings.CurrentTargetSpeed;
					float y = this._rigidbody.velocity.y;
					if (!this._jumping)
					{
						this._rigidbody.velocity *= 0.5f;
					}
					else
					{
						this._rigidbody.velocity *= 0.95f;
					}
					if (this._rigidbody.velocity.sqrMagnitude < this.movementSettings.CurrentTargetSpeed * this.movementSettings.CurrentTargetSpeed)
					{
						if (flag)
						{
							vector *= this.SlopeMultiplier();
						}
						this._rigidbody.AddForce(this._rigidbody.velocity - vector, ForceMode.VelocityChange);
					}
					if (!this.Jumping && !this._isGrounded)
					{
						this._rigidbody.AddForce(new Vector3(0f, this._fallMassRatio + y, 0f), ForceMode.Impulse);
					}
					if (this.movementSettings.Running)
					{
						LocalPlayer.Stats.Stamina -= this.sprintStaminaPerSecond * Time.deltaTime;
					}
				}
				else if (this._isGrounded && !this._jumping && !this._rigidbody.IsSleeping() && Mathf.Abs(this._input.x) < 1.401298E-45f && Mathf.Abs(this._input.y) < 1.401298E-45f)
				{
					this._rigidbody.velocity *= this.advancedSettings.slowDownRate;
					if (this._rigidbody.velocity.sqrMagnitude < 1f)
					{
						this._rigidbody.Sleep();
					}
				}
				if (this._isGrounded)
				{
					this._rigidbody.drag = this.advancedSettings.groundedDrag;
					if (this._jump)
					{
						this._rigidbody.drag = 0f;
						this._rigidbody.velocity = new Vector3(this._rigidbody.velocity.x, 0f, this._rigidbody.velocity.z);
						this._rigidbody.AddForce(0f, this.movementSettings.JumpForce, 0f, ForceMode.Impulse);
						this._jumping = true;
					}
					else if (this._pushingSled)
					{
						float num = this._input.x * -50f;
						LocalPlayer.Transform.RotateAround(LocalPlayer.AnimControl.sledPivot.position, Vector3.up, num * Time.deltaTime);
					}
				}
				else
				{
					this._rigidbody.drag = ((!this._isSwiming) ? 0f : this.advancedSettings.swimmingDrag);
					if (this._previouslyGrounded && !this._jumping)
					{
						this.StickToGroundHelper();
					}
				}
				if (this._crouch)
				{
					if (!this._crouching || this._standingUp)
					{
						this._crouching = true;
						this._standingUp = false;
						base.StartCoroutine("EnableCrouch");
					}
					if (this._standUp)
					{
						this._standingUp = true;
						this._crouch = false;
						this._standUp = false;
						base.StartCoroutine("DisableCrouch");
					}
				}
				this._jump = false;
				if (this.Swimming || !this._isGrounded)
				{
					if (this.Diving)
					{
						this.tempGrav = LocalPlayer.AnimControl.absCamX;
						this.tempGrav = Mathf.Clamp(this.tempGrav, -1f, 1f);
						this.normVal = this.tempGrav;
						this.tempGrav *= 6f;
						if (this.normVal < 0f)
						{
							this.normVal *= -1f;
						}
						this.normVal = 1f - this.normVal;
						if ((double)this._rigidbody.velocity.magnitude > 0.05)
						{
							this._rigidbody.AddForce(new Vector3(0f, -this.tempGrav * this._rigidbody.mass, 0f));
						}
						if (this._input.sqrMagnitude < 1f)
						{
							this._rigidbody.AddForce(new Vector3(0f, -this._rigidbody.velocity.y * 40f, 0f));
						}
						else
						{
							this._rigidbody.AddForce(new Vector3(0f, this._rigidbody.velocity.magnitude * 2f * this.normVal, 0f), ForceMode.Impulse);
						}
					}
					else
					{
						this._rigidbody.AddForce(new Vector3(0f, -this.gravity * this._rigidbody.mass, 0f));
					}
				}
			}
		}

		private void OnCollisionEnter(Collision coll)
		{
			if (this._allowFallDamage)
			{
				if (coll.relativeVelocity.y > this.fallDamageVelocityThreshold)
				{
					float num = coll.relativeVelocity.y * 1f;
					LocalPlayer.Stats.Hit((int)num, true, PlayerStats.DamageType.Physical);
					base.CancelInvoke("FallDamageTimer");
				}
				this._allowFallDamage = !this._isGrounded;
			}
		}

		public void LockView(bool rigidbodyLock = true)
		{
			if (!BoltNetwork.isRunning && rigidbodyLock)
			{
				this._rigidbody.Sleep();
				this._rigidbody.isKinematic = true;
				this._rigidbody.useGravity = false;
			}
			this.Locked = true;
			this.Inventory.transform.parent = null;
			this.SurvivalBook.transform.parent = null;
			TheForest.Utils.Input.UnLockMouse();
		}

		public void UnLockView()
		{
			this.Inventory.transform.parent = base.transform;
			this.SurvivalBook.transform.parent = base.transform;
			this.Locked = false;
			TheForest.Utils.Input.LockMouse();
			this._rigidbody.isKinematic = false;
			this._rigidbody.useGravity = true;
			this._rigidbody.WakeUp();
		}

		public void OnRaft()
		{
			this._sailingRaft = true;
			this._isGrounded = false;
			this._crouch = false;
			this.animator.SetBoolReflected("paddleIdleBool", true);
			this.animator.SetLayerWeightReflected(2, 1f);
		}

		public void OffRaft()
		{
			this._sailingRaft = false;
			this._isGrounded = true;
			this.animator.SetBoolReflected("paddleIdleBool", false);
		}

		public void enablePaddleOnRaft(bool set)
		{
			this.animator.SetBoolReflected("paddleBool", set);
		}

		public void OnSled()
		{
			this._pushingSled = true;
			this._crouch = false;
		}

		public void OffSled()
		{
			this._pushingSled = false;
		}

		public void ResetRotations()
		{
			this.mouseLook.Init(base.transform, LocalPlayer.MainCamTr);
		}

		private void FallDamageTimer()
		{
			this._allowFallDamage = true;
		}

		private float SlopeMultiplier()
		{
			float time = Vector3.Angle(this._groundContactNormal, Vector3.up);
			return this.movementSettings.SlopeCurveModifier.Evaluate(time);
		}

		private void StickToGroundHelper()
		{
			RaycastHit raycastHit;
			if (Physics.SphereCast(base.transform.position, this._capsule.radius, Vector3.down, out raycastHit, this._capsule.height / 2f - this._capsule.radius + this.advancedSettings.stickToGroundHelperDistance) && Mathf.Abs(Vector3.Angle(raycastHit.normal, Vector3.up)) < 85f)
			{
				this._rigidbody.velocity -= Vector3.Project(this._rigidbody.velocity, raycastHit.normal);
			}
		}

		private void UpdateInput()
		{
			this.movementSettings.UpdateDesiredTargetSpeed(this._isGrounded, this.Swimming, this._crouching, this.CarryHeavy);
			this._input.x = -TheForest.Utils.Input.GetAxis("Horizontal");
			this._input.y = -TheForest.Utils.Input.GetAxis("Vertical");
		}

		private void RotateView()
		{
			if (Mathf.Abs(Time.timeScale) < 1.401298E-45f)
			{
				return;
			}
			float y = base.transform.eulerAngles.y;
			this.mouseLook.LookRotation(base.transform, LocalPlayer.MainCamTr);
			if (this._isGrounded || this.advancedSettings.airControl)
			{
			}
		}

		private void AntiStuckCheck()
		{
			if (Mathf.Approximately(this._lastFrameYPosition, base.transform.position.y))
			{
				if (++this._antiStuckFramesCount > this.antiStuckFramesThreshold)
				{
					this._isGrounded = true;
					this._antiStuckFramesCount = 0;
				}
			}
			else
			{
				this._lastFrameYPosition = base.transform.position.y;
				this._antiStuckFramesCount = 0;
			}
		}

		private void GroundCheck()
		{
			this._previouslyGrounded = this._isGrounded;
			RaycastHit raycastHit;
			if (Physics.SphereCast(base.transform.position, this._capsule.radius, Vector3.down, out raycastHit, this._capsule.height / 2f - this._capsule.radius - this._capsule.center.y + this.advancedSettings.groundCheckDistance, this.CollisionLayers.value))
			{
				this._isGrounded = true;
				this._groundContactNormal = raycastHit.normal;
				this._isGroundContact = (Mathf.Abs(raycastHit.point.y - base.transform.position.y) > this._capsule.height / 2f - this._capsule.radius * 2f);
			}
			else
			{
				this._isGrounded = false;
				this._isGroundContact = true;
				this._groundContactNormal = Vector3.up;
			}
			if (!this._previouslyGrounded && this._isGrounded && this._jumping)
			{
				this._jumping = false;
			}
			if (this._allowFallDamage && this._isGrounded && this._previouslyGrounded)
			{
				this._allowFallDamage = false;
				base.CancelInvoke("FallDamageTimer");
			}
			else if (!this._isGrounded && this._previouslyGrounded)
			{
				base.Invoke("FallDamageTimer", 0.35f);
			}
		}

		public bool IsAboveWaistDeep()
		{
			return this.CalculateWaterDepth() >= 7.5f;
		}

		public float CalculateWaterDepth()
		{
			float num = this.buoyancy.WaterLevel - this._capsule.bounds.min.y;
			if (num >= 0f)
			{
				return Mathf.Clamp01(num / this._capsule.height) * 5f + 5f;
			}
			return TerrainWetness.instance.WaterLevel * 1f;
		}

		public float CalculateSpeedParameter(float flatVelocity)
		{
			if (flatVelocity > this.movementSettings.ForwardSpeed)
			{
				return Mathf.Clamp01((flatVelocity - this.movementSettings.ForwardSpeed) / (this.movementSettings.SprintSpeed - this.movementSettings.ForwardSpeed));
			}
			float num = this.movementSettings.ForwardSpeed - this.movementSettings.CrouchSpeed;
			return Mathf.Clamp01((flatVelocity - this.movementSettings.CrouchSpeed) / num) - 1f;
		}

		private void ScaleCapsuleForCrouching(float alpha)
		{
			this._capsule.height = Mathf.Lerp(this._capsuleOriginalHeight, this.crouchHeight, alpha);
			this._capsule.center = new Vector3(0f, Mathf.Lerp(0f, this._crouchCapsuleCenter, alpha), 0f);
			this.HeadBlock.center = new Vector3(0f, Mathf.Lerp(1.76f, -0.65f, alpha), 1f);
		}

		private void ResetCapsule()
		{
			this.ScaleCapsuleForCrouching(0f);
		}

		[DebuggerHidden]
		private IEnumerator EnableCrouch()
		{
			FirstPersonCharacter.<EnableCrouch>c__Iterator184 <EnableCrouch>c__Iterator = new FirstPersonCharacter.<EnableCrouch>c__Iterator184();
			<EnableCrouch>c__Iterator.<>f__this = this;
			return <EnableCrouch>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator DisableCrouch()
		{
			FirstPersonCharacter.<DisableCrouch>c__Iterator185 <DisableCrouch>c__Iterator = new FirstPersonCharacter.<DisableCrouch>c__Iterator185();
			<DisableCrouch>c__Iterator.<>f__this = this;
			return <DisableCrouch>c__Iterator;
		}
	}
}
