using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items.Inventory;
using TheForest.UI.Multiplayer;
using TheForest.Utils;
using UnityEngine;

public class FirstPersonCharacter : MonoBehaviour
{
	public const float HARD_FALL_THRESHOLD = 28.5f;

	private const float jumpRayLength = 0.7f;

	private const float WET_GROUND_END = 1f;

	private const float IMMERSION_START = 5f;

	private const float IMMERSION_END = 10f;

	private const float IMMERSION_RANGE = 5f;

	public Vector2 InputHack;

	public LayerMask CollisionLayers;

	public float walkSpeed = 3f;

	public float runSpeed = 8f;

	public float strafeSpeed = 4f;

	public float crouchSpeed = 2f;

	public float swimmingSpeed = 3f;

	public float staminaCostPerSec = 4f;

	public bool SailingRaft;

	public bool PushingSled;

	public bool Diving;

	public bool standingOnRaft;

	public bool Locked;

	public Transform Inventory;

	public Transform SurvivalBook;

	public SphereCollider HeadBlock;

	public float gravity = 10f;

	public float maxVelocityChange = 10f;

	public float maximumVelocity = 25f;

	public bool CanJump = true;

	public float jumpHeight = 2f;

	public float stickToGroundHelperDistance = 0.05f;

	public float groundStableForce;

	public float extremeAngleGroundedLimit = 65f;

	public float maxDiveVelocity = 6.5f;

	public float maxSwimVelocity = 3f;

	public bool drinking;

	public bool run;

	public bool running;

	public bool snowFlotation;

	public RigidBodyCollisionFlags collFlags;

	public PhysicMaterial playerPhysicMaterial;

	public PhysicMaterial playerPhysicMaterial2;

	private PlayerStats Stats;

	private PlayerSfx Sfx;

	private Buoyancy buoyancy;

	private playerScriptSetup setup;

	private playerTargetFunctions targets;

	private Animator animator;

	private SphereCollider headCollider;

	public CapsuleCollider capsule;

	public Rigidbody rb;

	private bool prevGrounded = true;

	public bool allowFallDamage;

	public bool jumpCoolDown;

	public bool jumping;

	public bool allowJump;

	public bool checkGrounded;

	public bool checkSwimming;

	private float prevVelocity;

	public bool terrainContact;

	private Vector2 input;

	private Vector3 inputVelocity;

	public Vector3 velocity;

	private bool crouchBlock;

	private FsmBool fsmCrouchBool;

	public bool crouching;

	public bool standingUp;

	public bool jumpingAttack;

	private bool jumpLand;

	private bool jumpFuzzyDelay;

	private float speed;

	public bool crouch;

	public bool standUp;

	private float originalHeight;

	public float originalYPos;

	private float yChange;

	private float getCapsuleY;

	private bool inSand;

	private bool inSnow;

	private float crouchCapsuleCenter;

	private float crouchHeight = 3f;

	public bool CantRun;

	private float prevMouseXSpeed;

	private FsmBool fsmClimbBool;

	private float lastUpdateTime;

	private bool stamRechargeDelay;

	public bool allowWaterJump;

	private bool jumpInputBlock;

	public bool contactWithEnemy;

	private float storeMaxVelocity;

	public bool hitByEnemy;

	public bool setNearEnemyVelocity;

	public float jumpingTimer;

	public float clampInputVal = 1f;

	private bool doingClampInput;

	private bool fallShakeBlock;

	public float clampInputDelay = 0.2f;

	public float defaultMass;

	private bool forceStopRun;

	private bool blockWaterJump;

	public bool Grounded
	{
		get;
		private set;
	}

	public float PrevVelocity
	{
		get
		{
			return this.prevVelocity;
		}
	}

	public bool swimming
	{
		get
		{
			return this.buoyancy && this.buoyancy.InWater;
		}
	}

	public bool Sitting
	{
		get;
		set;
	}

	private void Awake()
	{
		this.buoyancy = base.GetComponent<Buoyancy>();
		this.rb = base.GetComponent<Rigidbody>();
		this.playerPhysicMaterial = base.GetComponent<CapsuleCollider>().material;
		this.playerPhysicMaterial2 = base.GetComponent<SphereCollider>().material;
		this.rb.freezeRotation = true;
		this.rb.useGravity = false;
		this.collFlags = base.transform.GetComponent<RigidBodyCollisionFlags>();
		this.setup = base.GetComponentInChildren<playerScriptSetup>();
		this.targets = base.GetComponentInChildren<playerTargetFunctions>();
		this.UnLockView();
		this.capsule = (base.GetComponent<Collider>() as CapsuleCollider);
		this.defaultMass = this.rb.mass;
		this.originalHeight = this.capsule.height;
		this.originalYPos = this.capsule.center.y;
		this.crouchCapsuleCenter = (this.crouchHeight - this.originalHeight) / 2f;
		this.Grounded = true;
		this.fsmCrouchBool = FsmVariables.GlobalVariables.GetFsmBool("playerCrouchBool");
	}

	private void OnDeserialized()
	{
		if (!this.collFlags)
		{
			this.collFlags = base.gameObject.AddComponent<RigidBodyCollisionFlags>();
			this.collFlags.coll = this.capsule;
			this.collFlags.collType = 1;
			this.collFlags.cColl = base.transform.GetComponent<CapsuleCollider>();
		}
	}

	private void Start()
	{
		this.storeMaxVelocity = this.maximumVelocity;
		this.headCollider = base.transform.GetComponent<SphereCollider>();
		this.fsmClimbBool = LocalPlayer.ScriptSetup.pmControl.FsmVariables.GetFsmBool("climbBool");
		this.Stats = base.gameObject.GetComponent<PlayerStats>();
		this.Sfx = base.gameObject.GetComponent<PlayerSfx>();
		this.animator = this.setup.playerBase.GetComponent<Animator>();
	}

	private void Update()
	{
		if (this.collFlags.groundAngleVal > this.extremeAngleGroundedLimit && !this.doingClampInput && !this.jumping && !this.Grounded && !LocalPlayer.AnimControl.swimming)
		{
			this.clampInputVal = 0f;
		}
		else if (!this.doingClampInput && !Scene.HudGui.MpPlayerListCamGo.activeInHierarchy && !Scene.HudGui.PauseMenu.activeInHierarchy && !ChatBox.IsChatOpen)
		{
			this.clampInputVal = 1f;
		}
		if (Scene.HudGui.MpPlayerListCamGo.activeInHierarchy || Scene.HudGui.PauseMenu.activeInHierarchy || ChatBox.IsChatOpen)
		{
			this.clampInputVal = 0f;
			this.rb.velocity = new Vector3(0f, this.rb.velocity.y, 0f);
		}
		LocalPlayer.ScriptSetup.pmControl.FsmVariables.GetFsmBool("grounded").Value = this.Grounded;
		this.contactWithEnemy = false;
		if (LocalPlayer.AnimControl.swimming && !this.Diving && TheForest.Utils.Input.GetButtonDown("Jump"))
		{
			if (this.collFlags.collisionFlags == CollisionFlags.Sides || this.allowWaterJump)
			{
				this.Sfx.PlayJumpSound();
				this.rb.velocity = new Vector3(this.velocity.x, this.CalculateJumpVerticalSpeed() * 1.25f, this.velocity.z);
			}
			else if (!this.blockWaterJump)
			{
				this.Sfx.PlayJumpSound();
				this.rb.velocity = new Vector3(this.velocity.x, this.CalculateWaterJumpVerticalSpeed(), this.velocity.z);
				this.blockWaterJump = true;
				base.Invoke("resetBlockWaterJump", 1f);
			}
		}
		if (this.allowJump && !TheForest.Utils.Input.GetButton("Jump"))
		{
			if (!this.jumping)
			{
				LocalPlayer.Animator.SetBoolReflected("jumpBool", false);
			}
			if (this.collFlags.groundAngleVal > this.extremeAngleGroundedLimit)
			{
				this.playerPhysicMaterial.staticFriction = 0f;
				this.playerPhysicMaterial.dynamicFriction = 0f;
				this.playerPhysicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
				this.playerPhysicMaterial2.staticFriction = 0f;
				this.playerPhysicMaterial2.dynamicFriction = 0f;
				this.playerPhysicMaterial2.frictionCombine = PhysicMaterialCombine.Minimum;
			}
			else if (this.inputVelocity.magnitude < 0.05f)
			{
				this.playerPhysicMaterial.staticFriction = 1f;
				this.playerPhysicMaterial.dynamicFriction = 1f;
				this.playerPhysicMaterial.frictionCombine = PhysicMaterialCombine.Average;
				this.playerPhysicMaterial2.staticFriction = 1f;
				this.playerPhysicMaterial2.dynamicFriction = 1f;
				this.playerPhysicMaterial2.frictionCombine = PhysicMaterialCombine.Average;
			}
			else
			{
				this.playerPhysicMaterial.staticFriction = 0.2f;
				this.playerPhysicMaterial.dynamicFriction = 0.2f;
				this.playerPhysicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
				this.playerPhysicMaterial2.staticFriction = 0.2f;
				this.playerPhysicMaterial2.dynamicFriction = 0.2f;
				this.playerPhysicMaterial2.frictionCombine = PhysicMaterialCombine.Minimum;
			}
		}
		else if (this.allowJump && this.CanJump && this.Grounded && TheForest.Utils.Input.GetButtonDown("Jump") && !LocalPlayer.AnimControl.useRootMotion && !LocalPlayer.AnimControl.doSledPushMode && !LocalPlayer.AnimControl.onRope && !this.fsmClimbBool.Value && !this.Diving && !this.jumpInputBlock && LocalPlayer.Inventory.CurrentView != PlayerInventory.PlayerViews.Inventory)
		{
			this.jumpInputBlock = true;
			base.StartCoroutine("startJumpTimer");
			base.StartCoroutine("clampInput");
			base.Invoke("resetJumpInputBlock", 0.2f);
			this.Sfx.PlayJumpSound();
			LocalPlayer.Animator.SetBoolReflected("jumpBool", true);
			this.rb.velocity = new Vector3(this.velocity.x, this.CalculateJumpVerticalSpeed(), this.velocity.z);
			base.Invoke("allowJumpingAttack", 0.15f);
			LocalPlayer.ScriptSetup.pmControl.SendEvent("goToJump");
			this.jumping = true;
			this.allowJump = false;
		}
		bool value = this.setup.pmControl.FsmVariables.GetFsmBool("heavyAttackBool").Value;
		if (!this.crouch && !value)
		{
			this.crouch = TheForest.Utils.Input.GetButtonDown("Crouch");
		}
		else
		{
			this.standUp = ((!PlayerPreferences.UseCrouchToggle) ? (!TheForest.Utils.Input.GetButton("Crouch")) : TheForest.Utils.Input.GetButtonDown("Crouch"));
		}
		this.run = TheForest.Utils.Input.GetButton("Run");
		if (TheForest.Utils.Input.GetButtonDown("Run"))
		{
			this.CantRun = false;
		}
		if (this.run && LocalPlayer.AnimControl.overallSpeed > 0.4f)
		{
			this.running = true;
		}
		else
		{
			this.running = false;
		}
		if (this.crouch)
		{
			if (!this.crouching || this.standingUp)
			{
				this.crouching = true;
				this.standingUp = false;
				this.speed = this.crouchSpeed;
				base.StartCoroutine("EnableCrouch");
			}
			if (this.standUp || value)
			{
				this.standingUp = true;
				this.crouch = false;
				this.standUp = false;
				base.StartCoroutine("DisableCrouch");
			}
		}
	}

	private void FixedUpdate()
	{
		if (!this.swimming)
		{
			this.allowWaterJump = false;
		}
		this.checkGrounded = this.Grounded;
		this.checkSwimming = this.swimming;
		if (PlayerPreferences.VSync)
		{
			if (Time.realtimeSinceStartup - this.lastUpdateTime < Time.fixedDeltaTime)
			{
				return;
			}
			this.lastUpdateTime = Time.realtimeSinceStartup;
		}
		if (Clock.planecrash || LocalPlayer.AnimControl.introCutScene)
		{
			this.capsule.center = new Vector3(this.capsule.center.x, this.originalYPos, this.capsule.center.z);
			return;
		}
		int terrainSurface = LocalPlayer.AnimControl.getTerrainSurface();
		if (terrainSurface == 4 && LocalPlayer.Transform.position.y - Terrain.activeTerrain.SampleHeight(LocalPlayer.Transform.position) < 2.4f && !Clock.planecrash && !LocalPlayer.AnimControl.introCutScene)
		{
			this.inSand = true;
			if (!this.crouching)
			{
				this.yChange = this.capsule.center.y;
				this.capsule.center = new Vector3(this.capsule.center.x, Mathf.Lerp(this.yChange, this.originalYPos + 0.25f, Time.deltaTime), this.capsule.center.z);
			}
		}
		else if (LocalPlayer.Stats.IsInNorthColdArea() && !this.snowFlotation && LocalPlayer.Transform.position.y - Terrain.activeTerrain.SampleHeight(LocalPlayer.Transform.position) < 2.4f && !Clock.planecrash && !LocalPlayer.AnimControl.introCutScene)
		{
			this.inSnow = true;
			if (!this.crouching)
			{
				this.yChange = this.capsule.center.y;
				this.capsule.center = new Vector3(this.capsule.center.x, Mathf.Lerp(this.yChange, this.originalYPos + 0.25f, Time.deltaTime), this.capsule.center.z);
			}
		}
		else if (this.yChange != this.originalYPos)
		{
			this.inSand = false;
			this.inSnow = false;
			if (!this.crouching)
			{
				this.yChange = this.capsule.center.y;
				this.capsule.center = new Vector3(this.capsule.center.x, Mathf.Lerp(this.yChange, this.originalYPos, Time.deltaTime * 3f), this.capsule.center.z);
			}
		}
		if (this.SailingRaft)
		{
			return;
		}
		if (!this.Locked || (this.Grounded && !this.prevGrounded))
		{
			if (this.Grounded)
			{
				if (!this.prevGrounded)
				{
					LocalPlayer.CamFollowHead.stopAllCameraShake();
					this.fallShakeBlock = false;
					base.StopCoroutine("startJumpTimer");
					if (this.prevVelocity > 28.5f && this.allowFallDamage)
					{
						if (!this.jumpLand && !Clock.planecrash)
						{
							this.jumpCoolDown = true;
							this.jumpLand = true;
							float num = this.prevVelocity * 1.1f * (this.prevVelocity / 27.5f);
							int num2 = (int)num;
							if (this.jumpingTimer > 3.5f)
							{
								num2 *= 10;
							}
							this.Stats.Hit(num2, true, PlayerStats.DamageType.Physical);
							LocalPlayer.Animator.SetBoolReflected("jumpBool", false);
							if (this.Stats.Health > 0f)
							{
								if (!LocalPlayer.ScriptSetup.pmControl.FsmVariables.GetFsmBool("doingJumpAttack").Value)
								{
									LocalPlayer.Animator.SetIntegerReflected("jumpType", 1);
									LocalPlayer.Animator.SetBoolReflected("jumpBool", false);
									this.CanJump = false;
									LocalPlayer.HitReactions.enableControllerFreeze();
									this.prevMouseXSpeed = LocalPlayer.MainRotator.rotationSpeed;
									LocalPlayer.MainRotator.rotationSpeed = 0.55f;
									LocalPlayer.Animator.SetLayerWeightReflected(4, 0f);
									LocalPlayer.Animator.SetLayerWeightReflected(0, 1f);
									LocalPlayer.Animator.SetLayerWeightReflected(1, 0f);
									LocalPlayer.Animator.SetLayerWeightReflected(2, 0f);
									LocalPlayer.Animator.SetLayerWeightReflected(3, 0f);
									base.Invoke("resetAnimSpine", 1f);
								}
								else
								{
									this.jumpLand = false;
									this.jumpCoolDown = false;
								}
							}
							else
							{
								this.jumpCoolDown = false;
								this.jumpLand = false;
							}
						}
						this.blockJumpAttack();
					}
					this.jumping = false;
					base.CancelInvoke("setAnimatorJump");
					if (!this.jumpCoolDown)
					{
						LocalPlayer.Animator.SetIntegerReflected("jumpType", 0);
						LocalPlayer.Animator.SetBoolReflected("jumpBool", false);
						LocalPlayer.ScriptSetup.pmControl.SendEvent("toWait");
						this.blockJumpAttack();
					}
					base.CancelInvoke("fallDamageTimer");
					this.allowFallDamage = false;
				}
				this.allowFallDamage = false;
				this.prevGrounded = true;
				this.jumping = false;
			}
			else
			{
				if (this.prevGrounded)
				{
					this.jumping = true;
					base.Invoke("disableAllowJump", 0.25f);
					base.Invoke("fallDamageTimer", 0.35f);
					if (!LocalPlayer.AnimControl.useRootMotion && !LocalPlayer.AnimControl.doSledPushMode && !LocalPlayer.AnimControl.onRope && !this.fsmClimbBool.Value && !this.Diving && !this.crouching)
					{
						base.Invoke("setAnimatorJump", 0.3f);
					}
				}
				this.prevGrounded = false;
			}
			if (LocalPlayer.AnimControl.swimming)
			{
				this.animator.SetBoolReflected("jumpBool", false);
			}
			Vector3 vector = new Vector3(TheForest.Utils.Input.GetAxis("Horizontal") + this.InputHack.x, 0f, TheForest.Utils.Input.GetAxis("Vertical") + this.InputHack.y);
			vector = Vector3.ClampMagnitude(vector, 1.1f);
			this.inputVelocity = vector;
			if (this.PushingSled)
			{
				vector.x = 0f;
			}
			vector = base.transform.TransformDirection(vector);
			vector *= this.speed;
			this.velocity = this.rb.velocity;
			Vector3 vector2 = vector - this.velocity;
			vector2.x = Mathf.Clamp(vector2.x, -this.maxVelocityChange, this.maxVelocityChange) * this.clampInputVal;
			vector2.z = Mathf.Clamp(vector2.z, -this.maxVelocityChange, this.maxVelocityChange) * this.clampInputVal;
			vector2.y = 0f;
			if (this.Grounded || this.swimming)
			{
				if (this.swimming && !this.Grounded)
				{
					LocalPlayer.CamFollowHead.stopAllCameraShake();
					this.fallShakeBlock = false;
					Vector3 vector3 = Vector3.zero;
					vector3 = Vector3.Slerp(vector3, vector2, Time.deltaTime * 8f);
					this.rb.AddForce(vector3, ForceMode.VelocityChange);
					if (LocalPlayer.AnimControl.swimming && !this.Diving && !LocalPlayer.WaterViz.InWater && this.collFlags.collisionFlags != CollisionFlags.Sides && !this.allowWaterJump)
					{
						this.maxDiveVelocity = this.maxSwimVelocity;
						if (this.rb.velocity.magnitude > this.maxDiveVelocity)
						{
							Vector3 a = this.rb.velocity.normalized;
							a *= this.maxDiveVelocity;
							this.rb.velocity = a;
						}
					}
					else
					{
						this.maxDiveVelocity = 7f;
					}
				}
				else
				{
					this.rb.AddForce(vector2, ForceMode.VelocityChange);
				}
				if (this.hitByEnemy)
				{
					this.maximumVelocity = 3f;
				}
				else if (this.setNearEnemyVelocity)
				{
					this.maximumVelocity = 5.5f;
				}
				else
				{
					this.maximumVelocity = this.storeMaxVelocity;
				}
				if (this.rb.velocity.magnitude > this.maximumVelocity)
				{
					Vector3 a2 = this.rb.velocity.normalized;
					a2 *= this.maximumVelocity;
					this.rb.velocity = a2;
				}
				if (this.inputVelocity.magnitude > 0.05f && !this.swimming && this.Grounded && this.allowJump)
				{
					float num3 = this.rb.velocity.magnitude * this.rb.mass * this.groundStableForce;
					this.rb.AddForce(new Vector3(0f, -num3, 0f));
				}
			}
			if (!this.swimming && !this.Diving && this.jumping)
			{
				Vector3 vector4 = Vector3.zero;
				if (!this.hitByEnemy)
				{
					this.maximumVelocity = this.storeMaxVelocity;
				}
				vector4 = Vector3.Slerp(vector4, vector2 * 0.9f, Time.deltaTime * 10f);
				vector4.y = vector2.y;
				this.rb.AddForce(vector4, ForceMode.VelocityChange);
				float magnitude = this.rb.velocity.magnitude;
				if (magnitude > this.maximumVelocity)
				{
					Vector3 a3 = this.rb.velocity.normalized;
					a3 *= this.maximumVelocity;
					this.rb.velocity = a3;
				}
				if (magnitude > 26f && !this.fallShakeBlock)
				{
					LocalPlayer.CamFollowHead.StartCoroutine("startFallingShake");
					LocalPlayer.AnimControl.disconnectFromObject();
					this.fallShakeBlock = true;
				}
			}
			if (this.run && !this.crouch && !this.CantRun && this.Stats.Stamina > 0f && LocalPlayer.AnimControl.overallSpeed > 0.4f)
			{
				if (this.Stats.Stamina < 10.2f && !this.forceStopRun)
				{
					base.StartCoroutine("doForceStopRun");
				}
				this.Stats.Stamina -= this.staminaCostPerSec * Time.fixedDeltaTime * LocalPlayer.Stats.Skills.RunStaminaRatio;
				LocalPlayer.Stats.Skills.TotalRunDuration += Time.fixedDeltaTime;
				if (this.swimming && !this.Grounded)
				{
					this.speed = this.swimmingSpeed * 2.2f;
				}
				else if (this.Diving)
				{
					this.speed = this.swimmingSpeed;
				}
				else if (this.inSand || this.inSnow)
				{
					this.speed = this.runSpeed;
				}
				else
				{
					this.speed = this.runSpeed;
				}
			}
			else if (!this.crouch)
			{
				if (this.swimming && !this.Grounded)
				{
					this.speed = this.swimmingSpeed;
				}
				else if (this.Diving)
				{
					this.speed = this.swimmingSpeed * 0.8f;
				}
				else if (this.inSand)
				{
					this.speed = this.walkSpeed * 0.85f;
				}
				else if (this.inSnow)
				{
					this.speed = this.walkSpeed * 0.85f;
				}
				else
				{
					this.speed = this.walkSpeed;
				}
			}
			if (this.Diving)
			{
				float num4 = LocalPlayer.AnimControl.absCamX;
				num4 = Mathf.Clamp(num4, -1f, 1f);
				float num5 = num4;
				if (num5 > 0f)
				{
					num4 *= 8f;
				}
				else
				{
					num4 *= 25f;
				}
				if (num5 < 0f)
				{
					num5 *= -1f;
				}
				num5 = 1f - num5;
				this.inputVelocity = this.inputVelocity.normalized;
				if ((double)this.rb.velocity.magnitude > 0.05)
				{
					this.rb.AddForce(new Vector3(0f, -num4 * this.rb.mass, 0f));
					if (this.rb.velocity.magnitude > this.maxDiveVelocity)
					{
						Vector3 a4 = this.rb.velocity.normalized;
						a4 *= this.maxDiveVelocity;
						this.rb.velocity = a4;
					}
				}
				if (this.inputVelocity.magnitude < 0.1f)
				{
					this.rb.AddForce(new Vector3(0f, -this.rb.velocity.y * 50f, 0f));
				}
				else
				{
					this.rb.AddForce(new Vector3(0f, -this.rb.velocity.y * 50f * num5, 0f));
				}
			}
			else
			{
				this.rb.AddForce(new Vector3(0f, -this.gravity * this.rb.mass, 0f));
				this.Grounded = false;
				this.standingOnRaft = false;
				this.terrainContact = false;
			}
		}
	}

	private void disableAllowJump()
	{
		this.allowJump = false;
	}

	private void OnCollisionEnter(Collision coll)
	{
		this.prevVelocity = coll.relativeVelocity.y;
		if (coll.contacts.Length == 0)
		{
			return;
		}
		if (coll.contacts[0].point.y > base.transform.position.y && coll.contacts[0].thisCollider == this.capsule)
		{
			LocalPlayer.Stats.CheckCollisionFromAbove(coll);
		}
	}

	private void OnCollisionStay(Collision col)
	{
		ContactPoint[] contacts = col.contacts;
		for (int i = 0; i < contacts.Length; i++)
		{
			ContactPoint contactPoint = contacts[i];
			if (contactPoint.otherCollider.GetType() == typeof(TerrainCollider))
			{
				this.terrainContact = true;
			}
			else
			{
				this.terrainContact = false;
			}
			if (contactPoint.otherCollider.GetType() == typeof(CapsuleCollider))
			{
				if (!this.prevGrounded)
				{
					if (57.29578f * Mathf.Asin(contactPoint.normal.y) > 45f && this.velocity.y <= 3f && this.collFlags.groundAngleVal < this.extremeAngleGroundedLimit)
					{
						base.CancelInvoke("disableAllowJump");
						this.Grounded = true;
						this.allowJump = true;
					}
				}
				else if (57.29578f * Mathf.Asin(contactPoint.normal.y) > 45f && this.collFlags.groundAngleVal < this.extremeAngleGroundedLimit)
				{
					base.CancelInvoke("disableAllowJump");
					this.Grounded = true;
					this.allowJump = true;
				}
			}
			if (contactPoint.otherCollider.GetType() == typeof(MeshCollider) && LocalPlayer.AnimControl.swimming)
			{
				float num = 57.29578f * Mathf.Asin(contactPoint.normal.y);
				float num2 = contactPoint.point.y - base.transform.position.y;
				if (num > 0f && num2 < 0.5f)
				{
					this.allowWaterJump = true;
				}
				else
				{
					this.allowWaterJump = false;
				}
			}
			else
			{
				this.allowWaterJump = false;
			}
		}
		if (!col.collider)
		{
			return;
		}
		if (this.collFlags.collisionFlags == CollisionFlags.Below)
		{
			if (!this.prevGrounded)
			{
				if ((this.velocity.y <= 3f || this.terrainContact) && !this.Diving && this.collFlags.groundAngleVal < this.extremeAngleGroundedLimit)
				{
					if (col.gameObject.CompareTag("Raft"))
					{
						this.standingOnRaft = true;
					}
					base.CancelInvoke("disableAllowJump");
					this.allowJump = true;
					this.Grounded = true;
				}
			}
			else if (!this.Diving && this.collFlags.groundAngleVal < this.extremeAngleGroundedLimit)
			{
				if (col.gameObject.CompareTag("Raft"))
				{
					this.standingOnRaft = true;
				}
				base.CancelInvoke("disableAllowJump");
				this.allowJump = true;
				this.Grounded = true;
			}
		}
	}

	private void resetJumpInputBlock()
	{
		this.jumpInputBlock = false;
	}

	private void StickToGroundHelper()
	{
		RaycastHit raycastHit;
		if (Physics.SphereCast(base.transform.position, this.capsule.radius, Vector3.down, out raycastHit, this.capsule.height / 2f - this.capsule.radius + this.stickToGroundHelperDistance) && Mathf.Abs(Vector3.Angle(raycastHit.normal, Vector3.up)) < 85f)
		{
			this.rb.velocity -= Vector3.Project(this.rb.velocity, raycastHit.normal);
		}
	}

	public void ResetRotations()
	{
	}

	private void fallDamageTimer()
	{
		this.allowFallDamage = true;
	}

	private float CalculateJumpVerticalSpeed()
	{
		return Mathf.Sqrt(2f * this.jumpHeight * this.gravity);
	}

	private float CalculateWaterJumpVerticalSpeed()
	{
		return Mathf.Sqrt(2f * this.jumpHeight * this.gravity);
	}

	public void CoveredInMud()
	{
		this.targets.coveredInMud = true;
	}

	public void NotCoveredInMud()
	{
		this.targets.coveredInMud = false;
	}

	private void ScaleCapsuleForCrouching(float alpha)
	{
		float to = this.crouchCapsuleCenter;
		this.getCapsuleY = 0f;
		if (this.inSand || this.inSnow)
		{
			to = this.crouchCapsuleCenter + 0.25f;
			this.getCapsuleY = 0.25f;
		}
		this.capsule.height = Mathf.Lerp(this.originalHeight, this.crouchHeight, alpha);
		this.capsule.center = new Vector3(0f, Mathf.Lerp(this.getCapsuleY, to, alpha), 0f);
		this.HeadBlock.center = new Vector3(0f, Mathf.Lerp(1.76f, -0.65f, alpha), 1f);
	}

	private void ResetCapsule()
	{
		this.ScaleCapsuleForCrouching(0f);
	}

	[DebuggerHidden]
	private IEnumerator EnableCrouch()
	{
		FirstPersonCharacter.<EnableCrouch>c__Iterator114 <EnableCrouch>c__Iterator = new FirstPersonCharacter.<EnableCrouch>c__Iterator114();
		<EnableCrouch>c__Iterator.<>f__this = this;
		return <EnableCrouch>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator DisableCrouch()
	{
		FirstPersonCharacter.<DisableCrouch>c__Iterator115 <DisableCrouch>c__Iterator = new FirstPersonCharacter.<DisableCrouch>c__Iterator115();
		<DisableCrouch>c__Iterator.<>f__this = this;
		return <DisableCrouch>c__Iterator;
	}

	private void NormalSpeed()
	{
	}

	private void Slow()
	{
	}

	public void LockView(bool rigidbodyLock = true)
	{
		if (!BoltNetwork.isRunning && rigidbodyLock && this.Grounded)
		{
			this.rb.Sleep();
			this.rb.isKinematic = true;
			this.rb.useGravity = false;
		}
		this.Locked = true;
		this.Inventory.transform.parent = null;
		this.CanJump = false;
		TheForest.Utils.Input.UnLockMouse();
	}

	public void UnLockView()
	{
		this.Inventory.transform.parent = base.transform;
		this.Locked = false;
		this.CanJump = true;
		TheForest.Utils.Input.LockMouse();
		this.rb.isKinematic = false;
		this.rb.useGravity = true;
		this.rb.WakeUp();
	}

	public void OnRaft()
	{
		this.SailingRaft = true;
		this.Grounded = false;
		this.animator.SetBoolReflected("paddleIdleBool", true);
		this.animator.SetLayerWeightReflected(2, 1f);
		this.animator.SetLayerWeightReflected(1, 0f);
		this.animator.SetLayerWeightReflected(4, 0f);
	}

	public void OffRaft()
	{
		this.SailingRaft = false;
		this.Grounded = true;
		this.animator.SetBoolReflected("paddleIdleBool", false);
		this.animator.SetBoolReflected("paddleBool", false);
		LocalPlayer.Transform.localEulerAngles = new Vector3(0f, LocalPlayer.Transform.localEulerAngles.y, 0f);
	}

	public void enablePaddleOnRaft(bool set)
	{
		this.animator.SetBoolReflected("paddleBool", set);
	}

	public void OnSled()
	{
		this.PushingSled = true;
	}

	public void OffSled()
	{
		this.PushingSled = false;
	}

	public bool IsAboveWaistDeep()
	{
		return this.CalculateWaterDepth() >= 7.5f;
	}

	public bool IsInWater()
	{
		return this.buoyancy.WaterLevel > this.capsule.bounds.min.y;
	}

	public float CalculateWaterDepth()
	{
		float num = this.buoyancy.WaterLevel - this.capsule.bounds.min.y;
		if (num >= 0f)
		{
			return Mathf.Clamp01(num / this.capsule.height) * 5f + 5f;
		}
		return WaterOnTerrain.Instance.RainIntensity * 1f;
	}

	public float CalculateSpeedParameter(float flatVelocity)
	{
		if (flatVelocity > this.walkSpeed)
		{
			return Mathf.Clamp01((flatVelocity - this.walkSpeed) / (this.runSpeed - this.walkSpeed));
		}
		float num = this.walkSpeed - this.crouchSpeed;
		return Mathf.Clamp01((flatVelocity - this.crouchSpeed) / num) - 1f;
	}

	[DebuggerHidden]
	private IEnumerator resetFallDamage()
	{
		FirstPersonCharacter.<resetFallDamage>c__Iterator116 <resetFallDamage>c__Iterator = new FirstPersonCharacter.<resetFallDamage>c__Iterator116();
		<resetFallDamage>c__Iterator.<>f__this = this;
		return <resetFallDamage>c__Iterator;
	}

	private void setAnimatorJump()
	{
		LocalPlayer.Animator.SetBoolReflected("jumpBool", true);
	}

	private void resetAnimSpine()
	{
		this.jumpLand = false;
		base.StartCoroutine("smoothEnableSpine");
	}

	private void allowJumpingAttack()
	{
		LocalPlayer.AnimControl.setup.pmControl.FsmVariables.GetFsmBool("allowJumpAttack").Value = true;
	}

	private void blockJumpAttack()
	{
		LocalPlayer.AnimControl.setup.pmControl.FsmVariables.GetFsmBool("allowJumpAttack").Value = false;
	}

	[DebuggerHidden]
	private IEnumerator smoothEnableSpine()
	{
		FirstPersonCharacter.<smoothEnableSpine>c__Iterator117 <smoothEnableSpine>c__Iterator = new FirstPersonCharacter.<smoothEnableSpine>c__Iterator117();
		<smoothEnableSpine>c__Iterator.<>f__this = this;
		return <smoothEnableSpine>c__Iterator;
	}

	public void resetPhysicMaterial()
	{
		this.playerPhysicMaterial.dynamicFriction = 1f;
		this.playerPhysicMaterial.staticFriction = 1f;
		this.playerPhysicMaterial.frictionCombine = PhysicMaterialCombine.Average;
		this.playerPhysicMaterial2.dynamicFriction = 1f;
		this.playerPhysicMaterial2.staticFriction = 1f;
		this.playerPhysicMaterial2.frictionCombine = PhysicMaterialCombine.Average;
	}

	[DebuggerHidden]
	private IEnumerator enableStamRechargeDelay()
	{
		FirstPersonCharacter.<enableStamRechargeDelay>c__Iterator118 <enableStamRechargeDelay>c__Iterator = new FirstPersonCharacter.<enableStamRechargeDelay>c__Iterator118();
		<enableStamRechargeDelay>c__Iterator.<>f__this = this;
		return <enableStamRechargeDelay>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator doForceStopRun()
	{
		FirstPersonCharacter.<doForceStopRun>c__Iterator119 <doForceStopRun>c__Iterator = new FirstPersonCharacter.<doForceStopRun>c__Iterator119();
		<doForceStopRun>c__Iterator.<>f__this = this;
		return <doForceStopRun>c__Iterator;
	}

	public void clampVelocity()
	{
	}

	[DebuggerHidden]
	private IEnumerator doClampVelocity()
	{
		FirstPersonCharacter.<doClampVelocity>c__Iterator11A <doClampVelocity>c__Iterator11A = new FirstPersonCharacter.<doClampVelocity>c__Iterator11A();
		<doClampVelocity>c__Iterator11A.<>f__this = this;
		return <doClampVelocity>c__Iterator11A;
	}

	public void setEnemyContact(bool set)
	{
		if (set)
		{
			this.contactWithEnemy = true;
		}
		else
		{
			this.contactWithEnemy = false;
		}
	}

	[DebuggerHidden]
	private IEnumerator startJumpTimer()
	{
		FirstPersonCharacter.<startJumpTimer>c__Iterator11B <startJumpTimer>c__Iterator11B = new FirstPersonCharacter.<startJumpTimer>c__Iterator11B();
		<startJumpTimer>c__Iterator11B.<>f__this = this;
		return <startJumpTimer>c__Iterator11B;
	}

	[DebuggerHidden]
	private IEnumerator clampInput()
	{
		FirstPersonCharacter.<clampInput>c__Iterator11C <clampInput>c__Iterator11C = new FirstPersonCharacter.<clampInput>c__Iterator11C();
		<clampInput>c__Iterator11C.<>f__this = this;
		return <clampInput>c__Iterator11C;
	}

	private void resetBlockWaterJump()
	{
		this.blockWaterJump = false;
	}
}
