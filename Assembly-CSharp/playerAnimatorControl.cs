using Bolt;
using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Items;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

public class playerAnimatorControl : MonoBehaviour
{
	public bool USE_NEW_BOOK;

	public float armMultiplyer = 10f;

	public bool oculusDemo;

	private Animator animator;

	public Rigidbody controller;

	private playerHitReactions reactions;

	public playerScriptSetup setup;

	private weaponInfo mainWeaponInfo;

	private Buoyancy buoyancy;

	private PlayerInventory player;

	public animEventsManager animEvents;

	public HingeJoint sledHinge;

	public Transform sledPivot;

	public CapsuleCollider playerCollider;

	public SphereCollider playerHeadCollider;

	public CapsuleCollider enemyCollider;

	public ForceLocalPosZero forcePos;

	public GameObject cliffCollide;

	public GameObject currRaft;

	private Rigidbody rb;

	public GameObject oarHeld;

	public GameObject planeCrashGo;

	public Transform planeCrash;

	public List<Transform> starLocations = new List<Transform>();

	private AnimatorStateInfo currLayerState0;

	private AnimatorStateInfo currLayerState1;

	private AnimatorStateInfo nextLayerState1;

	private AnimatorStateInfo fullBodyState2;

	private AnimatorStateInfo fullBodyState3;

	private AnimatorStateInfo nextFullBodyState2;

	private AnimatorStateInfo nextFullBodyState3;

	private AnimatorStateInfo currLayterState7;

	private AnimatorStateInfo locoState;

	public float maxSpeed;

	public float torsoFollowSpeed;

	public float walkBlendSpeed;

	public float planeDist;

	private float colliderMove;

	public float storePrevYRotSpeed;

	private float horizontalSpeed;

	private float verticalSpeed;

	public float overallSpeed;

	private float initialAccel;

	public float tempTired;

	private Vector3 tempVelocity;

	private Quaternion storeLeftArmAngle;

	private float hVel;

	public bool tiredCheck;

	public int combo;

	private bool comboBlock;

	private bool waterBlock;

	private bool pointBlock;

	public bool onFire;

	public bool lockGravity;

	public bool swimLayerChange;

	public bool swimming;

	private bool leftArmActive;

	public bool upsideDown;

	private float startPlaneBlend;

	public bool blockCamX;

	public bool smashBool;

	public bool useRootMotion;

	public bool doSledPushMode;

	public bool onRope;

	public bool carry;

	public bool injured;

	public bool cliffClimb;

	public bool allowCliffReset;

	public GameObject placedTimmyGo;

	public GameObject heldTimmyGo;

	public GameObject placedBodyGo;

	public GameObject heldBodyGo;

	private Quaternion armAngle;

	private Vector3 prevNormal;

	private float prevMag;

	private float currMag;

	public Vector3 cliffEnterNormal;

	public Vector3 cliffEnterPos;

	private Vector3 normal1;

	private Vector3 normal2;

	private Vector3 normal3;

	private Vector3 normal4;

	private Vector3 normal5;

	private RaycastHit nHit;

	[ItemIdPicker]
	public int _rebreatherId;

	[ItemIdPicker]
	public int _torchId;

	[ItemIdPicker]
	public int _lighterId;

	[ItemIdPicker]
	public int _planeAxeId;

	[ItemIdPicker]
	public int _polaroid1Id;

	[ItemIdPicker]
	public int _polaroid2Id;

	[ItemIdPicker]
	public int _polaroid3Id;

	private int stickAttackHash;

	private int axeAttackHash;

	public int idleHash;

	public int hangingHash;

	private int checkArmsHash;

	private int heldHash;

	private int attackHash;

	private int smashHash;

	private int blockHash;

	private int deathHash;

	private int swimHash;

	private int climbingHash;

	private int climbIdleHash;

	private int enterClimbHash;

	private int explodeHash;

	private int knockBackHash;

	private FsmFloat fsmPlayerAngle;

	private FsmFloat fsmTiredFloat;

	private FsmBool fsmButtonHeldBool;

	private Transform tr;

	private Transform rootTr;

	private float camX;

	public float camY;

	public float headCamY;

	private Vector3 camForward;

	public float normCamX;

	public float absCamX;

	private float normCamY;

	private float smoothCamX;

	private float smoothCamY;

	private float camYOffset;

	private float prevCamY;

	private bool doFootReset;

	public bool onRaft;

	public bool introCutScene;

	public bool onRockThrower;

	private float mouseCurrentPosx;

	private float mouseDeltax;

	private int layerMask;

	private RaycastHit hit;

	private Vector3 pos;

	private float curVel;

	private float smoothShellBlend;

	private GameObject bird;

	public bool WaterBlock
	{
		get
		{
			return this.waterBlock;
		}
	}

	private void Awake()
	{
		this.setup = base.GetComponent<playerScriptSetup>();
		this.buoyancy = base.transform.GetComponentInParent<Buoyancy>();
		this.animEvents = base.transform.GetComponent<animEventsManager>();
		this.planeCrashGo = GameObject.FindWithTag("planeCrash");
		this.player = base.transform.GetComponentInParent<PlayerInventory>();
		this.sledHinge = base.transform.GetComponentInChildren<HingeJoint>();
		this.sledPivot = base.GetComponentInParent<Rigidbody>().transform.FindChild("sledPivot").transform;
		if (this.planeCrashGo)
		{
			this.setup.sceneInfo.planeCrash = this.planeCrashGo;
			this.planeCrash = this.planeCrashGo.transform;
		}
		else
		{
			GameObject gameObject = new GameObject();
			this.planeCrash = gameObject.transform;
			this.planeCrash.position = base.transform.position;
			if (this.setup.sceneInfo != null)
			{
				this.setup.sceneInfo.planeCrash = this.planeCrash.gameObject;
			}
		}
	}

	private void Start()
	{
		this.animator = base.gameObject.GetComponent<Animator>();
		this.reactions = base.transform.parent.GetComponent<playerHitReactions>();
		this.controller = base.transform.GetComponentInParent<Rigidbody>();
		this.playerCollider = this.controller.GetComponent<CapsuleCollider>();
		this.playerHeadCollider = this.controller.GetComponent<SphereCollider>();
		this.enemyCollider = this.setup.enemyBlockerGo.GetComponent<CapsuleCollider>();
		this.forcePos = base.transform.GetComponent<ForceLocalPosZero>();
		this.rb = base.transform.GetComponentInParent<Rigidbody>();
		this.tr = base.transform;
		this.rootTr = base.transform.root;
		this.smoothCamX = 0f;
		this.fsmPlayerAngle = this.setup.pmRotate.FsmVariables.GetFsmFloat("playerAngle");
		this.fsmTiredFloat = this.setup.pmStamina.FsmVariables.GetFsmFloat("notTiredSpeed");
		this.fsmButtonHeldBool = this.setup.pmControl.FsmVariables.GetFsmBool("checkHeldBool");
		this.stickAttackHash = Animator.StringToHash("stickAttack");
		this.axeAttackHash = Animator.StringToHash("AxeAttack");
		this.idleHash = Animator.StringToHash("idling");
		this.hangingHash = Animator.StringToHash("hanging");
		this.checkArmsHash = Animator.StringToHash("checkArms");
		this.heldHash = Animator.StringToHash("held");
		this.attackHash = Animator.StringToHash("attacking");
		this.smashHash = Animator.StringToHash("smash");
		this.blockHash = Animator.StringToHash("block");
		this.deathHash = Animator.StringToHash("death");
		this.swimHash = Animator.StringToHash("swimming");
		this.climbingHash = Animator.StringToHash("climbing");
		this.climbIdleHash = Animator.StringToHash("climbIdle");
		this.enterClimbHash = Animator.StringToHash("enterClimb");
		this.explodeHash = Animator.StringToHash("explode");
		this.knockBackHash = Animator.StringToHash("knockBack");
		this.setup.pmControl.FsmVariables.GetFsmInt("knockBackHash").Value = Animator.StringToHash("knockBack");
		this.setup.pmControl.FsmVariables.GetFsmInt("climbHash").Value = Animator.StringToHash("climbing");
		this.setup.pmControl.FsmVariables.GetFsmInt("climbIdleHash").Value = this.climbIdleHash;
		this.setup.pmControl.FsmVariables.GetFsmInt("sledHash").Value = Animator.StringToHash("onSled");
		this.setup.pmControl.FsmVariables.GetFsmInt("stick1Hash").Value = Animator.StringToHash("stickCombo1");
		this.setup.pmControl.FsmVariables.GetFsmInt("stick2Hash").Value = Animator.StringToHash("stickCombo2");
		this.layerMask = 69345280;
		if (this.oculusDemo)
		{
			this.animator.SetBoolReflected("oculusDemoBool", true);
			this.reactions.StartCoroutine("setControllerSpeed", 0f);
			this.animator.SetLayerWeightReflected(2, 1f);
			this.animator.SetLayerWeightReflected(4, 0f);
			this.animator.SetLayerWeightReflected(5, 1f);
			base.transform.parent.GetComponent<SimpleMouseRotator>().enabled = false;
		}
		base.InvokeRepeating("checkPlaneDistance", 5f, 5f);
	}

	private void checkPlaneDistance()
	{
		if (!this.planeCrash)
		{
			this.planeCrash = GameObject.FindWithTag("planeCrash").transform;
		}
		this.planeDist = Vector3.Distance(this.tr.position, this.planeCrash.position);
	}

	private bool IsLeftHandBusy()
	{
		return !this.player.IsLeftHandEmpty();
	}

	private void Update()
	{
		if (this.blockCamX)
		{
			this.startPlaneBlend = 0f;
		}
		else
		{
			this.startPlaneBlend = Mathf.Lerp(this.startPlaneBlend, 1f, Time.deltaTime);
		}
		if (this.doSledPushMode)
		{
			this.hVel = Mathf.SmoothDamp(this.hVel, TheForest.Utils.Input.GetAxis("Horizontal") * 50f, ref this.curVel, 0.1f);
			if (Mathf.Abs(this.hVel) > 0.0001f)
			{
				this.animEvents.IsSledTurning = true;
				this.rootTr.RotateAround(this.sledPivot.position, Vector3.up, this.hVel * Time.deltaTime);
			}
			else
			{
				this.curVel = 0f;
				this.animEvents.IsSledTurning = false;
			}
		}
		float @float = this.animator.GetFloat("normCamX");
		float float2 = this.animator.GetFloat("pedIdleBlend");
		float float3 = this.animator.GetFloat("normCamY");
		float float4 = this.animator.GetFloat("vSpeed");
		float float5 = this.animator.GetFloat("hSpeed");
		float float6 = this.animator.GetFloat("overallSpeed");
		this.currLayerState0 = this.animator.GetCurrentAnimatorStateInfo(0);
		this.currLayerState1 = this.animator.GetCurrentAnimatorStateInfo(1);
		this.nextLayerState1 = this.animator.GetNextAnimatorStateInfo(1);
		this.fullBodyState2 = this.animator.GetCurrentAnimatorStateInfo(2);
		this.fullBodyState3 = this.animator.GetCurrentAnimatorStateInfo(3);
		this.nextFullBodyState2 = this.animator.GetNextAnimatorStateInfo(2);
		this.nextFullBodyState3 = this.animator.GetNextAnimatorStateInfo(3);
		this.animator.SetBoolReflected("movingBool", false);
		if (this.IsLeftHandBusy())
		{
			if (!this.tiredCheck)
			{
				this.tempTired = this.fsmTiredFloat.Value;
				this.fsmTiredFloat.Value = this.fsmTiredFloat.Value / 1f;
				this.tiredCheck = true;
			}
		}
		else if (this.tiredCheck)
		{
			this.fsmTiredFloat.Value = this.tempTired;
			this.tiredCheck = false;
		}
		if (!LocalPlayer.Inventory.IsRightHandEmpty() || this.onRockThrower || LocalPlayer.Animator.GetBool("lookAtPhoto"))
		{
			this.animator.SetBoolReflected("blockColdBool", true);
		}
		else
		{
			this.animator.SetBoolReflected("blockColdBool", false);
		}
		if (LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.Book)
		{
			LocalPlayer.Ridigbody.velocity = new Vector3(0f, LocalPlayer.Ridigbody.velocity.y, 0f);
			LocalPlayer.FpCharacter.playerPhysicMaterial.dynamicFriction = 10f;
			LocalPlayer.FpCharacter.playerPhysicMaterial.staticFriction = 10f;
			LocalPlayer.FpCharacter.playerPhysicMaterial.frictionCombine = PhysicMaterialCombine.Average;
		}
		if (this.nextLayerState1.tagHash == this.heldHash && this.currLayerState1.tagHash == this.attackHash && this.animator.GetBool("bowHeld") && this.animator.IsInTransition(1))
		{
			this.animEvents.StopCoroutine("smoothEnableSpine");
			this.animEvents.StartCoroutine("smoothEnableSpine");
		}
		if (this.currLayerState1.IsName("bowIdle") && !this.animator.IsInTransition(1) && this.currLayerState0.tagHash != this.explodeHash)
		{
			this.animator.SetLayerWeightReflected(4, 1f);
		}
		if (!LocalPlayer.FpCharacter.jumpCoolDown)
		{
			if ((this.currLayerState1.tagHash == this.idleHash && !this.animator.IsInTransition(1)) || this.useRootMotion)
			{
				if (!this.upsideDown && !this.introCutScene)
				{
					this.animator.SetLayerWeightReflected(1, 0f);
				}
			}
			else if (this.nextLayerState1.tagHash == this.heldHash && this.currLayerState1.tagHash != this.attackHash && this.currLayerState1.tagHash != this.heldHash && this.currLayerState1.tagHash != this.smashHash && this.currLayerState1.tagHash != this.blockHash && this.currLayerState1.tagHash != this.knockBackHash)
			{
				float normalizedTime = this.animator.GetAnimatorTransitionInfo(1).normalizedTime;
				this.animator.SetLayerWeightReflected(1, normalizedTime);
			}
			else if (this.nextLayerState1.tagHash == this.idleHash)
			{
				float normalizedTime2 = this.animator.GetAnimatorTransitionInfo(1).normalizedTime;
				this.animator.SetLayerWeightReflected(1, 1f - normalizedTime2);
			}
			else if (!this.animator.IsInTransition(1) && this.currLayerState1.tagHash == this.heldHash)
			{
				this.animator.SetLayerWeightReflected(1, 1f);
			}
			if (this.fullBodyState2.tagHash == this.deathHash)
			{
				this.animator.SetLayerWeightReflected(2, 1f);
				this.animator.SetLayerWeightReflected(1, 0f);
				this.animator.SetLayerWeightReflected(3, 0f);
				this.animator.SetLayerWeightReflected(4, 0f);
				LocalPlayer.Ridigbody.velocity = new Vector3(0f, LocalPlayer.Ridigbody.velocity.y, 0f);
				this.pos = new Vector3(this.tr.position.x, this.tr.position.y + 5f, this.tr.position.z);
				if (Physics.Raycast(this.pos, Vector3.down, out this.hit, 20f, this.layerMask))
				{
					this.tr.rotation = Quaternion.Lerp(this.tr.rotation, Quaternion.LookRotation(Vector3.Cross(this.tr.right, this.hit.normal), this.hit.normal), Time.deltaTime * 6f);
				}
			}
			else if (this.fullBodyState2.tagHash != this.swimHash)
			{
				if (this.fullBodyState2.tagHash == this.explodeHash)
				{
					this.animator.SetLayerWeightReflected(2, 1f);
					LocalPlayer.Ridigbody.velocity = new Vector3(0f, LocalPlayer.Ridigbody.velocity.y, 0f);
				}
				else if (this.nextFullBodyState2.tagHash == this.idleHash && this.animator.IsInTransition(2) && !this.animator.GetBool(this.axeAttackHash) && !this.animator.GetBool(this.stickAttackHash) && this.fullBodyState2.tagHash != this.idleHash && this.animator.GetFloat("crouch") < 0.5f)
				{
					float normalizedTime3 = this.animator.GetAnimatorTransitionInfo(2).normalizedTime;
					this.animator.SetLayerWeightReflected(2, 0.95f - normalizedTime3);
				}
			}
			if (this.nextFullBodyState3.tagHash == this.checkArmsHash && this.fullBodyState3.tagHash == this.idleHash && this.animator.IsInTransition(3))
			{
				float normalizedTime4 = this.animator.GetAnimatorTransitionInfo(3).normalizedTime;
				this.animator.SetLayerWeightReflected(3, normalizedTime4);
				this.leftArmActive = true;
			}
			else if (!this.animator.IsInTransition(3) && this.fullBodyState3.tagHash == this.checkArmsHash)
			{
				this.leftArmActive = true;
				this.animator.SetLayerWeightReflected(3, 1f);
			}
			else if (this.nextFullBodyState3.tagHash == this.checkArmsHash && this.fullBodyState3.tagHash == this.checkArmsHash && this.animator.IsInTransition(3))
			{
				this.animator.SetLayerWeightReflected(3, 1f);
			}
			else if (this.nextFullBodyState3.tagHash == this.idleHash)
			{
				float normalizedTime5 = this.animator.GetAnimatorTransitionInfo(3).normalizedTime;
				this.animator.SetLayerWeightReflected(3, 1f - normalizedTime5);
				this.leftArmActive = false;
			}
			else if (this.fullBodyState3.tagHash == this.idleHash && !this.animator.IsInTransition(3))
			{
				this.animator.SetLayerWeightReflected(3, 0f);
			}
		}
		Vector3 vector = this.tr.InverseTransformDirection(this.controller.velocity);
		if (this.onRope)
		{
			float num = 50f;
			if (TheForest.Utils.Input.GetButton("Run"))
			{
				num = 100f;
			}
			vector.x = TheForest.Utils.Input.GetAxis("Horizontal") * num;
			vector.z = TheForest.Utils.Input.GetAxis("Vertical") * num;
		}
		this.verticalSpeed = Mathf.SmoothStep(this.verticalSpeed, vector.z / this.maxSpeed, Time.deltaTime * this.walkBlendSpeed);
		this.horizontalSpeed = Mathf.SmoothStep(this.horizontalSpeed, vector.x / this.maxSpeed, Time.deltaTime * this.walkBlendSpeed);
		this.tempVelocity = this.controller.velocity;
		this.tempVelocity.y = 0f;
		this.overallSpeed = this.tempVelocity.magnitude / this.maxSpeed;
		if (Mathf.Abs(float5 - this.horizontalSpeed) > 0.001f)
		{
			this.animator.SetFloat("hSpeed", this.horizontalSpeed);
		}
		if (Mathf.Abs(float4 - this.verticalSpeed) > 0.001f)
		{
			this.animator.SetFloat("vSpeed", this.verticalSpeed);
		}
		if (Mathf.Abs(float6 - this.overallSpeed) > 0.001f)
		{
			this.animator.SetFloat("overallSpeed", this.overallSpeed);
		}
		if (this.oculusDemo)
		{
			this.camX = this.setup.playerCam.eulerAngles.x;
			this.camForward = this.setup.playerCam.forward;
		}
		else
		{
			this.camX = this.setup.playerCam.eulerAngles.x;
			this.camForward = this.setup.playerCam.forward;
		}
		Vector3 forward = this.tr.forward;
		forward.y = 0f;
		this.camForward.y = 0f;
		Vector3 lhs = Vector3.Cross(forward, this.camForward);
		float num2 = Vector3.Dot(lhs, this.tr.up);
		if (this.camX > 180f)
		{
			this.normCamX = this.camX - 360f;
			this.absCamX = this.normCamX / 45f;
		}
		else
		{
			this.normCamX = this.camX;
			this.absCamX = this.normCamX / 45f;
		}
		if (!this.swimming)
		{
			if (LocalPlayer.FpCharacter.crouch)
			{
				this.animator.SetFloatReflected("spineBlendX", 6f, 0.5f, Time.deltaTime * 5f);
			}
			else if (LocalPlayer.FpCharacter.crouching && !LocalPlayer.FpCharacter.crouch)
			{
				this.animator.SetFloatReflected("spineBlendX", 10f, 0.5f, Time.deltaTime * 5f);
			}
			else
			{
				this.animator.SetFloatReflected("spineBlendX", 10f, 0.5f, Time.deltaTime * 2f);
			}
		}
		if (this.swimming)
		{
			this.normCamX /= 82f;
			this.normCamX = Mathf.Clamp(this.normCamX, -1f, 0.75f) - 0.1f;
			if (LocalPlayer.FpCharacter.Diving)
			{
				this.animator.SetFloatReflected("spineBlendX", 10f, 0.5f, Time.deltaTime * 2f);
			}
			else
			{
				this.animator.SetFloatReflected("spineBlendX", 0f, 0.5f, Time.deltaTime * 2f);
			}
		}
		else if (this.animator.GetBool("bowHeld"))
		{
			this.normCamX /= 85f;
			this.normCamX = Mathf.Clamp(this.normCamX, -1f, 1f);
		}
		else if (this.animator.GetBool("spearHeld"))
		{
			this.normCamX /= 70f;
			this.normCamX = Mathf.Clamp(this.normCamX, -1f, 1f);
		}
		else
		{
			this.normCamX /= 82f;
			this.normCamX = Mathf.Clamp(this.normCamX, -1f, 0.75f) - 0.1f;
		}
		this.camY = LocalPlayer.MainRotator.followAngles.y;
		this.headCamY = LocalPlayer.CamRotator.followAngles.y;
		if (this.camY < -100f && this.prevCamY > 100f)
		{
			this.camYOffset -= 360f;
		}
		else if (this.camY > 100f && this.prevCamY < -100f)
		{
			this.camYOffset += 360f;
		}
		this.fsmPlayerAngle.Value = this.camY;
		this.camY -= this.camYOffset;
		this.normCamY = this.camY / 60f;
		this.normCamY = Mathf.Clamp(this.normCamY, -1f, 1f) * 10f;
		this.smoothCamX = Mathf.Lerp(this.smoothCamX, this.normCamX, Time.deltaTime * this.torsoFollowSpeed);
		if (this.normCamY == 10f)
		{
			this.animator.SetIntegerReflected("turnLegInt", 1);
			base.Invoke("resetLegInt", 0.35f);
		}
		else if (this.normCamY == -10f)
		{
			this.animator.SetIntegerReflected("turnLegInt", -1);
			base.Invoke("resetLegInt", 0.35f);
		}
		this.smoothCamY = this.normCamY;
		this.prevCamY = LocalPlayer.MainRotator.followAngles.y;
		if (LocalPlayer.FpCharacter.SailingRaft || this.onRockThrower)
		{
			this.animator.SetFloat("normCamY", this.headCamY);
		}
		else
		{
			this.animator.SetFloat("normCamY", this.smoothCamY);
		}
		if (Mathf.Abs(float2 - this.smoothCamX) > 0.001f)
		{
			this.animator.SetFloat("pedIdleBlend", this.smoothCamX);
		}
		if (Mathf.Abs(@float - this.normCamX) > 0.001f)
		{
			if (this.upsideDown)
			{
				this.animator.SetFloat("normCamX", -this.normCamX);
			}
			else
			{
				this.animator.SetFloat("normCamX", this.normCamX * this.startPlaneBlend);
			}
		}
		if (this.buoyancy.InWater)
		{
			if (this.rootTr.position.y + 0.5f < this.buoyancy.WaterLevel)
			{
				this.animator.SetFloatReflected("inWaterBlend", 1f, 1f, Time.deltaTime);
				if (!this.swimLayerChange && !this.waterBlock)
				{
					base.StartCoroutine("smoothEnableLayerNew", 2);
					this.animator.SetBoolReflected("swimmingBool", true);
					this.swimming = true;
					LocalPlayer.Inventory.MemorizeItem(Item.EquipmentSlot.LeftHand);
					if (!LocalPlayer.Inventory.IsRightHandEmpty())
					{
						if (!LocalPlayer.Inventory.RightHand.IsHeldOnly)
						{
							LocalPlayer.Inventory.MemorizeItem(Item.EquipmentSlot.RightHand);
						}
						LocalPlayer.Inventory.StashEquipedWeapon(false);
					}
					else if (LocalPlayer.Inventory.Logs.HasLogs)
					{
						LocalPlayer.Inventory.Logs.PutDown(false, true, false);
						if (LocalPlayer.Inventory.Logs.HasLogs)
						{
							LocalPlayer.Inventory.Logs.PutDown(false, true, false);
						}
					}
					this.resetPushSled();
					base.CancelInvoke("disableSwimming");
					this.waterBlock = true;
				}
				if (this.rootTr.position.y + 1f < this.buoyancy.WaterLevel)
				{
					LocalPlayer.Inventory.StashLeftHand();
				}
				if ((this.absCamX > 0.65f && TheForest.Utils.Input.GetAxis("Vertical") > 0f) || this.rootTr.position.y + 3f < this.buoyancy.WaterLevel)
				{
					this.rb.useGravity = false;
					this.animator.SetBoolReflected("swimDiveBool", true);
					LocalPlayer.FpCharacter.Diving = true;
					LocalPlayer.FpCharacter.CanJump = false;
				}
				else
				{
					this.rb.useGravity = true;
					this.animator.SetBoolReflected("swimDiveBool", false);
					LocalPlayer.FpCharacter.Diving = false;
					LocalPlayer.FpCharacter.CanJump = true;
				}
			}
			else if (this.rootTr.position.y + 0.25f > this.buoyancy.WaterLevel && this.waterBlock)
			{
				if (!LocalPlayer.FpCharacter.Grounded && !LocalPlayer.FpCharacter.jumping)
				{
					base.Invoke("disableSwimming", 1.2f);
				}
				else
				{
					this.disableSwimming();
				}
				this.waterBlock = false;
			}
		}
		else if (this.waterBlock)
		{
			if (!LocalPlayer.FpCharacter.Grounded && !LocalPlayer.FpCharacter.jumping)
			{
				base.Invoke("disableSwimming", 1.2f);
			}
			else
			{
				this.disableSwimming();
			}
			this.waterBlock = false;
		}
		if (this.carry && TheForest.Utils.Input.GetButtonDown("Drop"))
		{
			this.DropBody();
		}
		if (this.waterBlock)
		{
			this.animator.SetFloatReflected("inWaterBlend", 1f, 1f, Time.deltaTime);
		}
		else
		{
			this.animator.SetFloatReflected("inWaterBlend", 0f, 1f, Time.deltaTime);
		}
		this.armAngle = this.setup.leftArm.rotation;
	}

	private void disableSwimming()
	{
		base.StartCoroutine("smoothDisableLayerNew", 2);
		this.swimming = false;
		this.animator.SetBoolReflected("swimmingBool", false);
		this.animator.SetBoolReflected("swimDiveBool", false);
		LocalPlayer.GameObject.GetComponent<Rigidbody>().useGravity = true;
		LocalPlayer.FpCharacter.Diving = false;
		LocalPlayer.FpCharacter.CanJump = true;
		if (!this.onRope)
		{
			LocalPlayer.Inventory.EquipPreviousUtility();
			if (LocalPlayer.Inventory.IsRightHandEmpty() && !LocalPlayer.Inventory.Logs.HasLogs)
			{
				LocalPlayer.Inventory.EquipPreviousWeapon();
			}
		}
		this.waterBlock = false;
	}

	public void resetLegInt()
	{
		this.camYOffset = LocalPlayer.MainRotator.followAngles.y;
		this.animator.SetIntegerReflected("turnLegInt", 0);
	}

	private void LateUpdate()
	{
		Vector3 vector = this.tr.InverseTransformDirection(LocalPlayer.MainCamTr.forward);
		Quaternion quaternion = this.setup.leftArm.rotation;
		if (LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.LeftHand, this._torchId) && this.leftArmActive)
		{
			if (this.onRope)
			{
				quaternion = Quaternion.AngleAxis(this.normCamX * this.armMultiplyer * 2f, LocalPlayer.Transform.right) * quaternion;
				quaternion *= Quaternion.AngleAxis(-vector.x * this.armMultiplyer * 2f, LocalPlayer.Transform.up);
			}
			else
			{
				quaternion = Quaternion.AngleAxis(this.normCamX * this.armMultiplyer * 0.8f * this.animator.GetLayerWeight(4), LocalPlayer.Transform.right) * quaternion;
			}
			this.setup.leftArm.rotation = quaternion;
		}
		if (this.animator.GetBool("shellHeld"))
		{
			float to;
			if (this.animator.GetBool("lighterHeld") || this.animator.GetBool("flashLightHeld"))
			{
				to = 1f;
			}
			else
			{
				to = 0f;
			}
			this.smoothShellBlend = Mathf.SmoothStep(this.smoothShellBlend, to, Time.deltaTime * 10f);
			this.animator.SetFloatReflected("shellBlend", this.smoothShellBlend);
		}
		Quaternion quaternion2 = this.setup.neckJnt.rotation;
		vector = this.tr.InverseTransformDirection(LocalPlayer.MainCamTr.forward);
		float num = 5f;
		if (this.cliffClimb)
		{
			num = 45f;
		}
		if (this.normCamX > 0f)
		{
			quaternion2 = Quaternion.AngleAxis(this.normCamX * num, LocalPlayer.Transform.right) * quaternion2;
		}
		if (this.onRaft)
		{
			quaternion2 = Quaternion.AngleAxis(this.normCamY * num * -10f, LocalPlayer.Transform.forward) * quaternion2;
		}
		this.setup.neckJnt.rotation = quaternion2;
	}

	private void FixedUpdate()
	{
		if (this.cliffClimb)
		{
			int num = 8192;
			LocalPlayer.FpCharacter.playerPhysicMaterial.dynamicFriction = 0f;
			LocalPlayer.FpCharacter.playerPhysicMaterial.staticFriction = 0f;
			LocalPlayer.FpCharacter.playerPhysicMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
			this.playerHeadCollider.enabled = false;
			this.rootTr.localEulerAngles = new Vector3(this.rootTr.localEulerAngles.x, this.rootTr.localEulerAngles.y, 0f);
			if (!this.allowCliffReset)
			{
				base.Invoke("enableCliffReset", 2f);
			}
			RaycastHit raycastHit;
			if (Physics.Raycast(this.rootTr.position - this.rootTr.forward, this.rootTr.forward, out raycastHit, 12f, num))
			{
				if (!raycastHit.collider.CompareTag("climbWall") && Clock.InCave)
				{
					this.player.SpecialActions.SendMessage("exitClimbCliffGround");
					this.allowCliffReset = false;
					base.CancelInvoke("enableCliffReset");
				}
				if (this.rb.velocity.magnitude < 0.1f)
				{
					this.rootTr.position = this.rootTr.position;
				}
				else
				{
					this.rootTr.position = Vector3.Lerp(this.rootTr.position, raycastHit.point - this.rootTr.forward * 1.9f, Time.deltaTime * 10f);
				}
				Vector3 vector = this.rootTr.position + this.rootTr.right * 2f + this.rootTr.up * 2f;
				if (Physics.Raycast(vector, this.rootTr.forward, out this.nHit, 20f, num))
				{
					UnityEngine.Debug.DrawLine(vector, this.nHit.point, Color.red);
					if (Clock.InCave && this.nHit.collider.gameObject.CompareTag("climbWall"))
					{
						this.normal1 = this.nHit.normal;
					}
					else if (!Clock.InCave)
					{
						this.normal1 = this.nHit.normal;
					}
				}
				vector = this.rootTr.position + this.rootTr.right * -2f + this.rootTr.up * 2f;
				if (Physics.Raycast(vector, this.rootTr.forward, out this.nHit, 20f, num))
				{
					UnityEngine.Debug.DrawLine(vector, this.nHit.point, Color.red);
					if (Clock.InCave && this.nHit.collider.gameObject.CompareTag("climbWall"))
					{
						this.normal2 = this.nHit.normal;
					}
					else if (!Clock.InCave)
					{
						this.normal2 = this.nHit.normal;
					}
				}
				vector = this.rootTr.position + this.rootTr.right * -2f + this.rootTr.up * -2f;
				if (Physics.Raycast(vector, this.rootTr.forward, out this.nHit, 20f, num))
				{
					UnityEngine.Debug.DrawLine(vector, this.nHit.point, Color.red);
					if (Clock.InCave && this.nHit.collider.gameObject.CompareTag("climbWall"))
					{
						this.normal3 = this.nHit.normal;
					}
					else if (!Clock.InCave)
					{
						this.normal3 = this.nHit.normal;
					}
				}
				vector = this.rootTr.position + this.rootTr.right * 2f + this.rootTr.up * -2f;
				if (Physics.Raycast(vector, this.rootTr.forward, out this.nHit, 20f, num))
				{
					UnityEngine.Debug.DrawLine(vector, this.nHit.point, Color.red);
					if (Clock.InCave && this.nHit.collider.gameObject.CompareTag("climbWall"))
					{
						this.normal4 = this.nHit.normal;
					}
					else if (!Clock.InCave)
					{
						this.normal4 = this.nHit.normal;
					}
				}
				vector = this.rootTr.position;
				if (Physics.Raycast(vector, this.rootTr.forward, out this.nHit, 20f, num))
				{
					UnityEngine.Debug.DrawLine(vector, this.nHit.point, Color.red);
					if (Clock.InCave && this.nHit.collider.gameObject.CompareTag("climbWall"))
					{
						this.normal5 = this.nHit.normal;
					}
					else if (!Clock.InCave)
					{
						this.normal5 = this.nHit.normal;
					}
				}
				Vector3 vector2 = (this.normal1 + this.normal2 + this.normal3 + this.normal4 + this.normal5) / 5f;
				float num2 = (vector2 - this.prevNormal).magnitude * 10f;
				if (this.rb.velocity.magnitude < 0.1f && num2 < 0.1f)
				{
					this.rootTr.rotation = this.rootTr.rotation;
				}
				else
				{
					this.rootTr.rotation = Quaternion.Lerp(this.rootTr.rotation, Quaternion.LookRotation(vector2 * -1f, this.rootTr.up), Time.deltaTime * 4f);
				}
				this.prevNormal = vector2;
				if (Vector3.Angle(Vector3.up, vector2) < 30f && this.allowCliffReset)
				{
					this.player.SpecialActions.SendMessage("exitClimbCliffGround");
					this.allowCliffReset = false;
					base.CancelInvoke("enableCliffReset");
				}
			}
			else if (this.allowCliffReset)
			{
				this.player.SpecialActions.SendMessage("exitClimbCliffGround");
				this.allowCliffReset = false;
				base.CancelInvoke("enableCliffReset");
			}
			Vector3 a = this.animator.deltaPosition * 75f;
			Vector3 velocity = this.rb.velocity;
			Vector3 force = a - velocity;
			this.rb.AddForce(force, ForceMode.VelocityChange);
		}
	}

	private void enableCliffReset()
	{
		this.allowCliffReset = true;
	}

	private void enablePointAtMap()
	{
		this.animator.SetBoolReflected("pointAtMap", true);
	}

	private void lookAtCamera()
	{
		this.tr.LookAt(this.setup.lookAtTr);
	}

	private void OnAnimatorMove()
	{
		this.storeLeftArmAngle = this.setup.leftArm.rotation;
		if (this.useRootMotion)
		{
			this.player._inventoryGO.SetActive(false);
			if (this.lockGravity)
			{
				this.rb.useGravity = false;
			}
			else
			{
				this.rb.useGravity = true;
				this.rb.isKinematic = false;
			}
			if (this.cliffClimb)
			{
				this.rb.isKinematic = false;
			}
			else if (this.lockGravity)
			{
				this.rb.isKinematic = true;
			}
			if (!this.cliffClimb)
			{
				this.rootTr.position += this.animator.deltaPosition;
			}
			this.pos = this.rootTr.position;
			this.pos.y = this.pos.y - 1.2f;
			if (this.onRope || this.injured)
			{
				LocalPlayer.Animator.SetLayerWeightReflected(4, 0f);
			}
			if (this.onRope)
			{
				if (this.cliffClimb)
				{
					LocalPlayer.CamRotator.rotationRange = new Vector2(104f, 90f);
				}
				else if (this.normCamX > 0f && this.currLayerState0.tagHash == this.climbIdleHash)
				{
					LocalPlayer.CamRotator.rotationRange = new Vector2(103f, 100f);
				}
				else if (this.normCamX < 0f && this.currLayerState0.tagHash == this.climbIdleHash)
				{
					LocalPlayer.CamRotator.rotationRange = new Vector2(150f, 105f);
				}
				else if (this.normCamX > 0f && this.currLayerState0.tagHash != this.climbIdleHash)
				{
					LocalPlayer.CamRotator.rotationRange = new Vector2(73f, 105f);
				}
				else
				{
					LocalPlayer.CamRotator.rotationRange = new Vector2(150f, 105f);
				}
			}
			if (this.injured)
			{
				this.tr.localEulerAngles = new Vector3(0f, 0f, 0f);
				LocalPlayer.Animator.SetLayerWeightReflected(4, 0f);
				LocalPlayer.Animator.SetLayerWeightReflected(0, 0f);
				LocalPlayer.Animator.SetLayerWeightReflected(1, 0f);
				LocalPlayer.Animator.SetLayerWeightReflected(2, 1f);
				LocalPlayer.Animator.SetLayerWeightReflected(3, 0f);
				return;
			}
			if (Physics.Raycast(this.pos, Vector3.down, out this.hit, 20f, this.layerMask) && this.hit.distance < 3f && TheForest.Utils.Input.GetAxis("Vertical") < -0.1f && this.animator.GetBool("setClimbBool") && (this.currLayerState0.tagHash == this.climbingHash || this.currLayerState0.tagHash == this.climbIdleHash))
			{
				UnityEngine.Debug.DrawLine(this.pos, this.hit.point, Color.red, 5f);
				int integer = this.animator.GetInteger("climbTypeInt");
				if (integer == 0)
				{
					this.player.SpecialActions.SendMessage("exitClimbRopeGround");
				}
				if (integer == 1)
				{
					this.player.SpecialActions.SendMessage("exitClimbWallGround");
				}
			}
		}
	}

	public void hitCombo()
	{
		if (!this.comboBlock)
		{
			this.combo++;
			this.comboBlock = true;
			base.Invoke("resetComboBlock", 0.27f);
			if (this.combo >= 1)
			{
				if (this.combo > 3)
				{
					this.combo = 0;
				}
				this.animator.SetBoolReflected("comboBool", true);
			}
		}
	}

	private void resetCombo()
	{
		this.combo = 0;
	}

	private void setComboOne()
	{
		this.combo = 1;
	}

	private void resetComboBlock()
	{
		this.comboBlock = false;
	}

	private void setStickAttack()
	{
		this.animator.SetBoolReflected("stickAttack", true);
	}

	public void resetAnimator()
	{
		this.animator.SetTriggerReflected("resetTrigger");
	}

	public void enableUseRootMotion()
	{
		this.useRootMotion = true;
	}

	public void enterClimbMode()
	{
		this.useRootMotion = true;
		this.onRope = true;
	}

	public void enterPushMode()
	{
		this.doSledPushMode = true;
	}

	public void exitPushMode()
	{
		this.doSledPushMode = false;
	}

	public void exitClimbMode()
	{
		if (this.useRootMotion)
		{
			this.playerHeadCollider.enabled = true;
			LocalPlayer.GameObject.GetComponent<Rigidbody>().freezeRotation = true;
			this.rootTr.localEulerAngles = new Vector3(0f, this.rootTr.localEulerAngles.y, 0f);
			this.controller.useGravity = true;
			this.controller.isKinematic = false;
			this.animator.SetLayerWeightReflected(4, 1f);
			this.player.PM.FsmVariables.GetFsmBool("climbBool").Value = false;
			this.player.PM.FsmVariables.GetFsmBool("climbTopBool").Value = false;
			LocalPlayer.CamRotator.rotationRange = new Vector2(170f, 0f);
			LocalPlayer.MainRotator.enabled = true;
			if (this.onRope || this.cliffClimb)
			{
				LocalPlayer.MainRotator.enabled = true;
				LocalPlayer.MainRotator.resetOriginalRotation = true;
			}
			this.setup.pmControl.SendEvent("toExitClimb");
			int integer = this.animator.GetInteger("climbTypeInt");
			if (integer == 0)
			{
				if (this.onRope)
				{
					this.player.SpecialActions.SendMessage("resetClimbRope");
				}
			}
			else if (integer == 1)
			{
				this.player.SpecialActions.SendMessage("resetClimbWall");
				this.player.SpecialActions.SendMessage("resetClimbCliff");
			}
			this.animator.SetBoolReflected("exitClimbTopBool", false);
			this.onRope = false;
			this.useRootMotion = false;
			base.CancelInvoke("enableCliffReset");
			this.allowCliffReset = false;
			this.cliffClimb = false;
		}
	}

	public void resetPushSled()
	{
		this.player.SpecialActions.SendMessage("forceExitSled");
	}

	public void sendEnableSledTrigger()
	{
		this.player.SpecialActions.SendMessage("enableSledTrigger");
	}

	public void resetCliffClimb()
	{
		this.player.SpecialActions.SendMessage("exitClimbCliffGround");
	}

	public void enableAnimLayer2()
	{
		if (this.animator.GetFloat("crouch") < 0.5f)
		{
			this.animator.SetLayerWeightReflected(2, 1f);
		}
		else
		{
			this.animator.SetLayerWeightReflected(2, 0f);
		}
	}

	public void setDeathState()
	{
	}

	public void setTimmyPickup(GameObject go)
	{
		if (!this.carry)
		{
			this.placedBodyGo = go;
			this.placedBodyGo.SetActive(false);
			this.heldBodyGo.SetActive(true);
			base.Invoke("setCarryBool", 0.5f);
		}
	}

	public void setMutantPickUp(GameObject go)
	{
		if (!this.heldBodyGo.activeSelf)
		{
			Scene.HudGui.DropButton.SetActive(true);
			this.player.MemorizeItem(Item.EquipmentSlot.RightHand);
			this.player.StashEquipedWeapon(false);
			this.placedBodyGo = go;
			if (BoltNetwork.isRunning && go.GetComponentInChildren<BoltEntity>())
			{
				SetCorpsePosition setCorpsePosition = SetCorpsePosition.Create(GlobalTargets.OnlyServer);
				setCorpsePosition.Corpse = go.GetComponentInChildren<BoltEntity>();
				setCorpsePosition.Corpse.Freeze(false);
				setCorpsePosition.Pickup = true;
				setCorpsePosition.Send();
			}
			else
			{
				this.placedBodyGo.SetActive(false);
			}
			this.heldBodyGo.SetActive(true);
			base.Invoke("setCarryBool", 0.5f);
			this.animator.SetBoolReflected("bodyHeld", true);
		}
	}

	public GameObject DropBody()
	{
		return this.DropBody(false);
	}

	public GameObject DropBody(bool destroy)
	{
		this.animator.SetBoolReflected("bodyHeld", false);
		this.carry = false;
		this.player.ShowRightHand();
		Scene.HudGui.DropButton.SetActive(false);
		this.heldBodyGo.SetActive(false);
		Vector3 vector = this.tr.position + this.tr.forward * 2f;
		if (BoltNetwork.isRunning)
		{
			vector = this.tr.position + this.tr.forward * 3.5f;
		}
		Quaternion rotation = this.rootTr.rotation;
		if (!BoltNetwork.isRunning)
		{
			this.placedBodyGo.transform.position = this.tr.position + this.tr.forward * 2f;
		}
		this.placedBodyGo.transform.rotation = this.rootTr.rotation;
		this.placedBodyGo.transform.localEulerAngles = new Vector3(this.placedBodyGo.transform.localEulerAngles.x, this.placedBodyGo.transform.localEulerAngles.y + 35f, this.placedBodyGo.transform.localEulerAngles.z);
		Vector3 vector2 = new Vector3(this.placedBodyGo.transform.position.x, this.placedBodyGo.transform.position.y + 8f, this.placedBodyGo.transform.position.z);
		int num = 100802560;
		RaycastHit raycastHit;
		if (Physics.Raycast(vector, Vector3.down, out raycastHit, 30f, num) && !BoltNetwork.isRunning)
		{
			this.placedBodyGo.transform.position = raycastHit.point;
			this.placedBodyGo.transform.rotation = Quaternion.LookRotation(Vector3.Cross(this.placedBodyGo.transform.right, raycastHit.normal), raycastHit.normal);
		}
		if (BoltNetwork.isRunning && this.placedBodyGo.GetComponent<BoltEntity>() && this.placedBodyGo.GetComponent<BoltEntity>().IsAttached())
		{
			DroppedBody droppedBody = DroppedBody.Create(GlobalTargets.Everyone);
			droppedBody.Target = this.placedBodyGo.GetComponent<BoltEntity>();
			droppedBody.rot = this.placedBodyGo.transform.rotation;
			droppedBody.Send();
			SetCorpsePosition setCorpsePosition = SetCorpsePosition.Create(GlobalTargets.OnlyServer);
			setCorpsePosition.Corpse = this.placedBodyGo.GetComponent<BoltEntity>();
			setCorpsePosition.Corpse.Freeze(false);
			setCorpsePosition.Position = vector;
			setCorpsePosition.Rotation = this.placedBodyGo.transform.rotation;
			setCorpsePosition.Pickup = false;
			setCorpsePosition.Destroy = destroy;
			setCorpsePosition.Send();
			this.placedBodyGo.transform.position = new Vector3(4096f, 4096f, 4096f);
		}
		else
		{
			this.placedBodyGo.SetActive(true);
			this.placedBodyGo.SendMessage("dropFromCarry", SendMessageOptions.DontRequireReceiver);
		}
		return this.placedBodyGo;
	}

	private void setCarryBool()
	{
		this.carry = true;
	}

	[DebuggerHidden]
	private IEnumerator checkChargeConditions()
	{
		playerAnimatorControl.<checkChargeConditions>c__IteratorD3 <checkChargeConditions>c__IteratorD = new playerAnimatorControl.<checkChargeConditions>c__IteratorD3();
		<checkChargeConditions>c__IteratorD.<>f__this = this;
		return <checkChargeConditions>c__IteratorD;
	}

	private void heavySwingDebug()
	{
	}

	public int getTerrainSurface()
	{
		return TerrainHelper.GetProminantTextureIndex(base.transform.position);
	}

	private void disableLockGravity()
	{
		this.lockGravity = false;
	}

	private void setBlockParams()
	{
		if (this.fullBodyState2.tagHash != this.explodeHash)
		{
			this.animator.SetLayerWeightReflected(2, 0f);
		}
	}

	public void sendCoopBlock()
	{
		if (BoltNetwork.isRunning)
		{
			playerBlock playerBlock = playerBlock.Create(GlobalTargets.Everyone);
			playerBlock.target = base.transform.parent.GetComponent<BoltEntity>();
			playerBlock.Send();
		}
	}

	public void forceAnimSpineReset()
	{
		this.animator.SetLayerWeightReflected(4, 1f);
	}

	public void resetCamera()
	{
		LocalPlayer.CamFollowHead.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
	}

	public void doSmoothEnableLayer2()
	{
		this.smoothEnableLayer(2, 0.5f);
	}

	public void doSmoothDisableLayer2()
	{
		this.smoothDisableLayer(2, 0.5f);
	}

	[DebuggerHidden]
	public IEnumerator smoothEnableLayer(int l, float s)
	{
		playerAnimatorControl.<smoothEnableLayer>c__IteratorD4 <smoothEnableLayer>c__IteratorD = new playerAnimatorControl.<smoothEnableLayer>c__IteratorD4();
		<smoothEnableLayer>c__IteratorD.l = l;
		<smoothEnableLayer>c__IteratorD.s = s;
		<smoothEnableLayer>c__IteratorD.<$>l = l;
		<smoothEnableLayer>c__IteratorD.<$>s = s;
		<smoothEnableLayer>c__IteratorD.<>f__this = this;
		return <smoothEnableLayer>c__IteratorD;
	}

	[DebuggerHidden]
	public IEnumerator smoothDisableLayer(int l, float s)
	{
		playerAnimatorControl.<smoothDisableLayer>c__IteratorD5 <smoothDisableLayer>c__IteratorD = new playerAnimatorControl.<smoothDisableLayer>c__IteratorD5();
		<smoothDisableLayer>c__IteratorD.l = l;
		<smoothDisableLayer>c__IteratorD.s = s;
		<smoothDisableLayer>c__IteratorD.<$>l = l;
		<smoothDisableLayer>c__IteratorD.<$>s = s;
		<smoothDisableLayer>c__IteratorD.<>f__this = this;
		return <smoothDisableLayer>c__IteratorD;
	}

	[DebuggerHidden]
	public IEnumerator smoothEnableLayerNew(int l)
	{
		playerAnimatorControl.<smoothEnableLayerNew>c__IteratorD6 <smoothEnableLayerNew>c__IteratorD = new playerAnimatorControl.<smoothEnableLayerNew>c__IteratorD6();
		<smoothEnableLayerNew>c__IteratorD.l = l;
		<smoothEnableLayerNew>c__IteratorD.<$>l = l;
		<smoothEnableLayerNew>c__IteratorD.<>f__this = this;
		return <smoothEnableLayerNew>c__IteratorD;
	}

	[DebuggerHidden]
	public IEnumerator smoothDisableLayerNew(int l)
	{
		playerAnimatorControl.<smoothDisableLayerNew>c__IteratorD7 <smoothDisableLayerNew>c__IteratorD = new playerAnimatorControl.<smoothDisableLayerNew>c__IteratorD7();
		<smoothDisableLayerNew>c__IteratorD.l = l;
		<smoothDisableLayerNew>c__IteratorD.<$>l = l;
		<smoothDisableLayerNew>c__IteratorD.<>f__this = this;
		return <smoothDisableLayerNew>c__IteratorD;
	}

	public void disconnectFromObject()
	{
		if (this.currRaft)
		{
			this.currRaft.SendMessage("offRaft", SendMessageOptions.DontRequireReceiver);
		}
		LocalPlayer.SpecialActions.SendMessage("forceDisableBench", SendMessageOptions.DontRequireReceiver);
		LocalPlayer.SpecialActions.SendMessage("forceDisableSled", SendMessageOptions.DontRequireReceiver);
		if (LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.Book)
		{
			LocalPlayer.Create.CloseTheBook();
		}
	}

	private void enableBirdOnHand()
	{
		this.setup.smallBirdGo.SetActive(true);
		this.bird = (GameObject)UnityEngine.Object.Instantiate((GameObject)Resources.Load("CutScene/smallBird_ANIM_landOnFinger_prefab"), this.setup.smallBirdGo.transform.position, this.setup.smallBirdGo.transform.rotation);
		this.bird.transform.parent = this.setup.smallBirdGo.transform;
		this.bird.transform.localPosition = Vector3.zero;
		this.bird.transform.localRotation = Quaternion.identity;
		Animator component = this.bird.GetComponent<Animator>();
		component.SetBool("toHand", true);
		base.Invoke("disableBirdOnHand", 10f);
	}

	public void disableBirdOnHand()
	{
		if (this.bird)
		{
			UnityEngine.Object.Destroy(this.bird);
		}
	}

	public void cancelAnimatorActions()
	{
		this.animator.SetBoolReflected("lookAtItemRight", false);
		this.animator.SetBoolReflected("lookAtPhoto", false);
		this.animator.SetBoolReflected("lookAtItem", false);
	}

	public void runGotHitScripts()
	{
		this.disconnectFromObject();
		LocalPlayer.HitReactions.enableControllerFreeze();
		this.animEvents.disableWeapon();
		this.animEvents.disableSmashWeapon();
		this.animEvents.disableWeapon2();
		LocalPlayer.HitReactions.enableHitState();
		this.disableBirdOnHand();
		this.animEvents.resetDrinkParams();
	}

	public void runReset2Scripts()
	{
		this.exitClimbMode();
		this.resetCombo();
		LocalPlayer.HitReactions.disableControllerFreeze();
		LocalPlayer.HitReactions.disableBlockState();
	}

	public void runWaitForInputScripts()
	{
		this.setBlockParams();
		this.animEvents.disableWeapon();
	}
}
