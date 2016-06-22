using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class mutantAnimatorControl : MonoBehaviour
{
	private Animator animator;

	private CharacterController controller;

	private Transform thisTr;

	private Transform rootTr;

	private mutantAI ai;

	private mutantScriptSetup setup;

	private enemyAnimEvents events;

	private mutantMaleHashId hashId;

	public AnimatorStateInfo fullBodyState;

	public AnimatorStateInfo currLayerState1;

	public AnimatorStateInfo nextLayerState1;

	public AnimatorStateInfo nextFullBodyState;

	public Transform target;

	public float hitDistance;

	public float offScreenSpeed;

	public bool inWater;

	public bool inBlockerTrigger;

	public bool inTreeTrigger;

	public Collider blockerCollider;

	public float gravity;

	private float animGravity;

	private Vector3 moveDir = Vector3.zero;

	private float currYPos;

	public float velY;

	public float accelY;

	public Vector3 wantedDir;

	public bool initBool;

	private Vector3 lookPos;

	private bool patrolling;

	private bool doResetTrigger;

	public bool forceIkBool;

	public bool playerCollideDisabled;

	public FsmFloat fsmPlayerDist;

	public FsmBool fsmDoControllerBool;

	public FsmBool fsmDeathBool;

	public FsmBool fsmInCaveBool;

	private FsmBool fsmEnableGravity;

	public FsmBool fsmTargetSeen;

	public FsmBool fsmNoMoveBool;

	private float initSpeed;

	private float terrainPosY;

	private float transitionTime;

	private float controllerRadius;

	private int jumpingHash;

	private int jumpFallHash;

	private int inTrapHash;

	private int runTrapHash;

	private RaycastHit hit;

	private Vector3 pos;

	private int layerMask;

	public float distanceTune = 2.5f;

	private bool changeBlock;

	private GameObject footPivot;

	private bool dynaCoolDown;

	private void Start()
	{
		this.animator = base.GetComponent<Animator>();
		this.controller = base.transform.parent.GetComponent<CharacterController>();
		this.events = base.GetComponent<enemyAnimEvents>();
		this.thisTr = base.transform;
		this.rootTr = base.transform.parent;
		this.ai = base.gameObject.GetComponent<mutantAI>();
		this.setup = base.GetComponent<mutantScriptSetup>();
		this.hashId = base.GetComponent<mutantMaleHashId>();
		this.target = LocalPlayer.Transform;
		this.jumpingHash = Animator.StringToHash("jumping");
		this.jumpFallHash = Animator.StringToHash("jumpFall");
		this.inTrapHash = Animator.StringToHash("inTrap");
		this.runTrapHash = Animator.StringToHash("runTrap");
		this.controllerRadius = this.controller.radius;
		if (this.setup.pmBrain)
		{
			this.fsmPlayerDist = this.setup.pmBrain.FsmVariables.GetFsmFloat("playerDist");
			this.fsmDoControllerBool = this.setup.pmBrain.FsmVariables.GetFsmBool("enableControllerBool");
			this.fsmEnableGravity = this.setup.pmBrain.FsmVariables.GetFsmBool("enableGravityBool");
			this.fsmTargetSeen = this.setup.pmBrain.FsmVariables.GetFsmBool("targetSeenBool");
		}
		if (this.setup.pmSleep)
		{
			this.fsmInCaveBool = this.setup.pmSleep.FsmVariables.GetFsmBool("inCaveBool");
			this.fsmNoMoveBool = this.setup.pmSleep.FsmVariables.GetFsmBool("noMoveBool");
		}
		if (this.setup.pmCombat)
		{
			this.fsmDeathBool = this.setup.pmCombat.FsmVariables.GetFsmBool("deathBool");
		}
		this.layerMask = 101851136;
		base.Invoke("initAnimator", 0.5f);
		if (BoltNetwork.isRunning)
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("boltIsActive").Value = true;
		}
		else
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("boltIsActive").Value = false;
		}
	}

	private void OnEnable()
	{
		base.Invoke("initAnimator", 0.5f);
		if (!this.animator)
		{
			this.animator = base.GetComponent<Animator>();
		}
		if (BoltNetwork.isRunning && this.initBool)
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("boltIsActive").Value = true;
		}
		else if (this.initBool)
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("boltIsActive").Value = false;
		}
		if (BoltNetwork.isRunning && !base.IsInvoking("forceDisableNetCollisions"))
		{
			base.InvokeRepeating("forceDisableNetCollisions", 2f, 1f);
		}
	}

	private void OnDisable()
	{
		base.CancelInvoke("initAnimator");
		this.initBool = false;
		this.doResetTrigger = false;
		this.dynaCoolDown = false;
		if (this.setup && this.setup.pmCombat)
		{
			this.setup.pmCombat.FsmVariables.GetFsmInt("dynamiteCounter").Value = 0;
		}
	}

	private void initAnimator()
	{
		this.initSpeed = this.animator.speed;
		this.initBool = true;
	}

	private void forceDisableNetCollisions()
	{
		if (!this.controller || !this.rootTr.gameObject.activeSelf)
		{
			return;
		}
		if (!this.controller.enabled)
		{
			return;
		}
		for (int i = 0; i < Scene.SceneTracker.allPlayers.Count; i++)
		{
			if (Scene.SceneTracker.allPlayers[i] && Scene.SceneTracker.allPlayers[i].CompareTag("PlayerNet"))
			{
				CapsuleCollider component = Scene.SceneTracker.allPlayers[i].GetComponent<CapsuleCollider>();
				if (component && component.enabled && Scene.SceneTracker.allPlayers[i].activeSelf)
				{
					Physics.IgnoreCollision(this.controller, component, true);
				}
			}
		}
	}

	private void checkGroundTags()
	{
	}

	private void enableIK()
	{
	}

	private void disableIK()
	{
	}

	private void resetAllActionBools()
	{
		if (this.animator.enabled)
		{
			this.animator.SetBoolReflected("crouchBOOL", false);
			this.animator.SetBoolReflected("screamBOOL", false);
			this.animator.SetBoolReflected("runAwayBOOL", false);
			this.animator.SetBoolReflected("attackBOOL", false);
			this.animator.SetBoolReflected("damageBOOL", false);
			this.animator.SetBoolReflected("attackLeftBOOL", false);
			this.animator.SetBoolReflected("attackRightBOOL", false);
			this.animator.SetBoolReflected("attackJumpBOOL", false);
			this.animator.SetBoolReflected("attackMovingBOOL", false);
			this.animator.SetBoolReflected("turnAroundBOOL", false);
			this.animator.SetBoolReflected("sideStepBOOL", false);
			this.animator.SetBoolReflected("dodgeBOOL", false);
			this.animator.SetBoolReflected("freakoutBOOL", false);
			this.animator.SetBoolReflected("idleWaryBOOL", false);
			this.animator.SetBoolReflected("fearBOOL", false);
			this.animator.SetBoolReflected("backAwayBOOL", false);
			this.animator.SetBoolReflected("jumpBOOL", false);
			this.animator.SetBoolReflected("crouchBOOL", false);
			this.animator.SetBoolReflected("crouchBOOL", false);
			this.animator.SetBoolReflected("crouchBOOL", false);
			this.animator.SetBoolReflected("onRockBOOL", false);
			this.animator.SetBoolReflected("treeBOOL", false);
			this.animator.SetBoolReflected("sideWalkBOOL", false);
			this.animator.SetBoolReflected("attackMidBOOL", false);
			this.animator.SetBoolReflected("walkBOOL", false);
			this.animator.SetBoolReflected("damageBehindBOOL", false);
			this.animator.SetBoolReflected("ritualBOOL", false);
			this.animator.SetBoolReflected("feedingBOOL", false);
			this.animator.SetBoolReflected("encounterBOOL", false);
			this.animator.SetBoolReflected("rescueBool1", false);
			this.animator.SetBoolReflected("jumpGapBool", false);
		}
	}

	private void OnAnimatorIK(int layer)
	{
		if (!this.initBool || !this.animator.enabled)
		{
			return;
		}
		if (this.setup.search.currentTarget)
		{
			Collider component = this.setup.search.currentTarget.GetComponent<Collider>();
			if (component)
			{
				this.lookPos = component.bounds.center;
			}
			else
			{
				this.lookPos = this.setup.search.currentTarget.transform.position;
			}
			this.lookPos.y = this.lookPos.y + 1.3f;
		}
		this.animator.SetLookAtPosition(this.lookPos);
		if (this.setup.pmBrain && this.animator)
		{
			if (this.fullBodyState.tagHash == this.hashId.deathTag)
			{
				this.controller.radius = 1.9f;
				if (!this.playerCollideDisabled)
				{
					this.disablePlayerCollision();
				}
			}
			if (!this.fsmTargetSeen.Value && !this.forceIkBool)
			{
				this.animator.SetLookAtWeight(0f);
			}
			else if (this.fullBodyState.tagHash == this.hashId.deathTag)
			{
				this.animator.SetLookAtWeight(0f, 0.1f, 0.6f, 1f, 0.9f);
			}
			else if (this.fullBodyState.tagHash == this.hashId.idleTag && !this.animator.IsInTransition(0))
			{
				this.animator.SetLookAtWeight(1f, 0.2f, 0.8f, 1f, 0.9f);
			}
			else if (this.fullBodyState.tagHash == this.hashId.idleTag && this.animator.IsInTransition(0))
			{
				this.animator.SetLookAtWeight(1f, 0.2f, 0.8f, 1f, 0.9f);
			}
			else if (this.fullBodyState.tagHash == this.hashId.idleTag && this.nextFullBodyState.tagHash != this.hashId.idleTag && this.animator.IsInTransition(0))
			{
				this.transitionTime = this.animator.GetAnimatorTransitionInfo(0).normalizedTime;
				this.animator.SetLookAtWeight(1f - this.transitionTime, 0.2f, 0.8f, 1f, 0.9f);
			}
			else if (this.fullBodyState.tagHash != this.hashId.idleTag && !this.animator.IsInTransition(0))
			{
				this.animator.SetLookAtWeight(0f, 0.2f, 0.8f, 1f, 0.9f);
			}
			else if (this.nextFullBodyState.tagHash == this.hashId.idleTag && this.fullBodyState.tagHash != this.hashId.idleTag && this.animator.IsInTransition(0))
			{
				this.transitionTime = this.animator.GetAnimatorTransitionInfo(0).normalizedTime;
				this.animator.SetLookAtWeight(this.transitionTime, 0.2f, 0.8f, 1f, 0.9f);
			}
			if (this.fullBodyState.tagHash != this.hashId.deathTag)
			{
				if (this.playerCollideDisabled)
				{
					this.enablePlayerCollision();
				}
				this.controller.radius = this.controllerRadius;
			}
		}
	}

	private void Update()
	{
		if (this.animator.enabled)
		{
			if (this.ai.targetAngle > -45f && this.ai.targetAngle < 45f)
			{
				if (!this.animator.GetBool("goForward"))
				{
					this.animator.SetBoolReflected("goForward", true);
				}
				if (this.animator.GetBool("goLeft"))
				{
					this.animator.SetBoolReflected("goLeft", false);
				}
				if (this.animator.GetBool("goRight"))
				{
					this.animator.SetBoolReflected("goRight", false);
				}
				if (this.animator.GetBool("goBack"))
				{
					this.animator.SetBoolReflected("goBack", false);
				}
			}
			else if (this.ai.targetAngle < -45f && this.ai.targetAngle > -110f)
			{
				if (this.animator.GetBool("goForward"))
				{
					this.animator.SetBoolReflected("goForward", false);
				}
				if (!this.animator.GetBool("goLeft"))
				{
					this.animator.SetBoolReflected("goLeft", true);
				}
				if (this.animator.GetBool("goRight"))
				{
					this.animator.SetBoolReflected("goRight", false);
				}
				if (this.animator.GetBool("goBack"))
				{
					this.animator.SetBoolReflected("goBack", false);
				}
			}
			else if (this.ai.targetAngle > 45f && this.ai.targetAngle < 110f)
			{
				if (this.animator.GetBool("goForward"))
				{
					this.animator.SetBoolReflected("goForward", false);
				}
				if (this.animator.GetBool("goLeft"))
				{
					this.animator.SetBoolReflected("goLeft", false);
				}
				if (!this.animator.GetBool("goRight"))
				{
					this.animator.SetBoolReflected("goRight", true);
				}
				if (this.animator.GetBool("goBack"))
				{
					this.animator.SetBoolReflected("goBack", false);
				}
			}
			else if (this.ai.targetAngle > 110f || this.ai.targetAngle < -110f)
			{
				if (this.animator.GetBool("goForward"))
				{
					this.animator.SetBoolReflected("goForward", false);
				}
				if (this.animator.GetBool("goLeft"))
				{
					this.animator.SetBoolReflected("goLeft", false);
				}
				if (this.animator.GetBool("goRight"))
				{
					this.animator.SetBoolReflected("goRight", false);
				}
				if (!this.animator.GetBool("goBack"))
				{
					this.animator.SetBoolReflected("goBack", true);
				}
			}
		}
		if (!this.animator.enabled && this.ai.doMove)
		{
			this.controller.enabled = false;
			Quaternion rotation = Quaternion.identity;
			this.wantedDir = this.ai.wantedDir;
			Vector3 vector = this.ai.wantedDir;
			vector.y = 0f;
			if (vector != Vector3.zero && vector.sqrMagnitude > 0f)
			{
				rotation = Quaternion.LookRotation(vector, Vector3.up);
			}
			this.thisTr.rotation = rotation;
			if (this.initBool && !this.fsmInCaveBool.Value)
			{
				if (Terrain.activeTerrain)
				{
					this.terrainPosY = Terrain.activeTerrain.SampleHeight(this.thisTr.position) + Terrain.activeTerrain.transform.position.y;
				}
				else
				{
					this.terrainPosY = this.rootTr.position.y;
				}
				this.rootTr.Translate(this.wantedDir * Time.deltaTime * this.offScreenSpeed, Space.World);
				this.rootTr.position = new Vector3(this.rootTr.position.x, this.terrainPosY, this.rootTr.position.z);
			}
		}
	}

	public void forceJumpLand()
	{
		this.events.playLandGround();
	}

	private void OnAnimatorMove()
	{
		if (!this.initBool || !this.animator.enabled)
		{
			return;
		}
		this.fullBodyState = this.animator.GetCurrentAnimatorStateInfo(0);
		this.nextFullBodyState = this.animator.GetNextAnimatorStateInfo(0);
		if (this.fullBodyState.tagHash == this.inTrapHash || this.fullBodyState.tagHash == this.runTrapHash)
		{
			this.animator.SetBoolReflected("enterTrapBool", false);
		}
		if (this.fullBodyState.tagHash == this.setup.hashs.idleTag)
		{
			this.animator.SetBoolReflected("dropFromTrap", false);
		}
		if (this.fullBodyState.tagHash == this.jumpFallHash && this.animator.enabled)
		{
			this.animator.speed = 1f;
			this.pos = this.thisTr.position;
			this.pos.y = this.pos.y + 1.4f;
			if (Physics.Raycast(this.pos, Vector3.down, out this.hit, 100f, this.layerMask))
			{
				this.hitDistance = this.hit.distance;
				if (this.hit.distance < 3f)
				{
					this.animator.SetTriggerReflected("jumpLandTrigger");
					this.events.playLandGround();
					base.Invoke("resetJumpTrigger", 2f);
				}
			}
		}
		else if (this.fullBodyState.tagHash == this.jumpingHash)
		{
			this.animator.speed = 1f;
		}
		else if (this.patrolling)
		{
			this.animator.speed = 0.8f;
		}
		else
		{
			this.animator.speed = this.ai.animSpeed;
		}
		if (this.initBool)
		{
			if (this.fsmDoControllerBool.Value)
			{
				if (this.fsmInCaveBool.Value)
				{
					this.controllerOn();
				}
				else if (this.animator && this.fsmPlayerDist.Value > 180f && !this.fsmInCaveBool.Value)
				{
					this.controllerOff();
				}
				else if (this.animator && this.fsmPlayerDist.Value < 180f)
				{
					this.controllerOn();
				}
				else
				{
					this.controllerOff();
				}
			}
			else
			{
				this.controllerOff();
			}
		}
	}

	private void controllerOn()
	{
		this.controller.enabled = true;
		if (this.fsmEnableGravity.Value)
		{
			this.animGravity = this.animator.GetFloat("Gravity");
		}
		else
		{
			this.animGravity = 0f;
		}
		this.moveDir = this.animator.deltaPosition;
		this.moveDir.y = this.moveDir.y - this.gravity * Time.deltaTime * this.animGravity;
		this.currLayerState1 = this.animator.GetCurrentAnimatorStateInfo(0);
		if (this.inBlockerTrigger && this.blockerCollider && !this.animator.GetBool("trapBool") && this.currLayerState1.tagHash != this.setup.hashs.deathTag)
		{
			Vector3 center = this.blockerCollider.bounds.center;
			center.y = this.thisTr.position.y;
			Vector3 normalized = (this.thisTr.position - center).normalized;
			CapsuleCollider component = this.blockerCollider.transform.GetComponent<CapsuleCollider>();
			float d = component.radius + this.setup.controller.radius;
			this.rootTr.position = center + normalized * d;
			this.inBlockerTrigger = false;
			if (!this.fsmNoMoveBool.Value)
			{
				this.controller.Move(this.moveDir);
				if (!this.animator.GetBool("idleWaryBOOL"))
				{
					this.thisTr.rotation = this.animator.rootRotation;
				}
			}
		}
		else if (this.inTreeTrigger && this.blockerCollider)
		{
			Vector3 center2 = this.blockerCollider.bounds.center;
			center2.y = this.thisTr.position.y;
			Vector3 vector = this.thisTr.InverseTransformPoint(center2);
			float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
			if (num < 90f && num > -90f)
			{
				Vector3 lhs = center2 - this.thisTr.position;
				if (lhs.sqrMagnitude > 0.05f)
				{
					Vector3 a = Vector3.Cross(lhs, this.thisTr.up);
					float d2 = 0f;
					if (num > 0f)
					{
						d2 = 1f;
					}
					else if (num < 0f)
					{
						d2 = -1f;
					}
					this.rootTr.position += a * d2 * (this.controller.velocity.magnitude * 0.25f) * Time.deltaTime;
					this.inTreeTrigger = false;
				}
			}
			this.inTreeTrigger = false;
		}
		if (!this.fsmNoMoveBool.Value)
		{
			this.controller.Move(this.moveDir);
			if (!this.animator.GetBool("idleWaryBOOL"))
			{
				this.thisTr.rotation = this.animator.rootRotation;
			}
		}
		Vector3 vector2 = Vector3.zero;
		if (this.fullBodyState.tagHash == this.hashId.deathTag)
		{
			Vector3 origin = new Vector3(this.thisTr.position.x, this.thisTr.position.y + 2f, this.thisTr.position.z);
			RaycastHit raycastHit;
			if (Physics.Raycast(origin, Vector3.down, out raycastHit, 10f, this.layerMask))
			{
				vector2 = raycastHit.normal;
			}
			else
			{
				vector2 = Vector3.up;
			}
		}
		else
		{
			vector2 = Vector3.up;
		}
		this.thisTr.rotation = Quaternion.Lerp(this.thisTr.rotation, Quaternion.LookRotation(Vector3.Cross(this.thisTr.right, vector2), vector2), Time.deltaTime * 8f);
	}

	private void controllerOff()
	{
		if (this.initBool)
		{
			this.controller.enabled = false;
			if (Terrain.activeTerrain)
			{
				this.terrainPosY = Terrain.activeTerrain.SampleHeight(this.thisTr.position) + Terrain.activeTerrain.transform.position.y;
			}
			else
			{
				this.terrainPosY = this.rootTr.position.y;
			}
			if (this.fsmEnableGravity.Value)
			{
				this.animGravity = this.animator.GetFloat("Gravity");
			}
			else
			{
				this.animGravity = 0f;
			}
			this.moveDir = this.animator.deltaPosition;
			if (!this.fsmNoMoveBool.Value)
			{
				this.rootTr.Translate(this.moveDir, Space.World);
				if (this.animGravity > 0.5f)
				{
					this.rootTr.position = new Vector3(this.rootTr.position.x, this.terrainPosY, this.rootTr.position.z);
				}
				if (!this.animator.GetBool("idleWaryBOOL"))
				{
					this.thisTr.rotation = this.animator.rootRotation;
				}
			}
		}
	}

	private void setDeathTrigger()
	{
		if (this.animator.enabled)
		{
			this.animator.SetTriggerReflected("deathTrigger");
		}
	}

	private void enableController()
	{
		this.fsmDoControllerBool.Value = true;
	}

	private void disableDeathBool()
	{
	}

	private void enableDeathBool()
	{
	}

	private void resetJumpTrigger()
	{
		if (this.animator.enabled)
		{
			this.animator.ResetTrigger("jumpLandTrigger");
		}
		this.doResetTrigger = false;
	}

	private void enablePatrolAnimSpeed()
	{
		this.patrolling = true;
	}

	private void disablePatrolAnimSpeed()
	{
		this.patrolling = false;
	}

	[DebuggerHidden]
	private IEnumerator smoothChangeIdle(float i)
	{
		mutantAnimatorControl.<smoothChangeIdle>c__Iterator69 <smoothChangeIdle>c__Iterator = new mutantAnimatorControl.<smoothChangeIdle>c__Iterator69();
		<smoothChangeIdle>c__Iterator.i = i;
		<smoothChangeIdle>c__Iterator.<$>i = i;
		<smoothChangeIdle>c__Iterator.<>f__this = this;
		return <smoothChangeIdle>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator fixFootPosition()
	{
		mutantAnimatorControl.<fixFootPosition>c__Iterator6A <fixFootPosition>c__Iterator6A = new mutantAnimatorControl.<fixFootPosition>c__Iterator6A();
		<fixFootPosition>c__Iterator6A.<>f__this = this;
		return <fixFootPosition>c__Iterator6A;
	}

	private void disableFixFootPosition()
	{
		base.StopCoroutine("fixFootPosition");
	}

	private void resetInTrap()
	{
		this.setup.hitReceiver.inNooseTrap = false;
	}

	private void disableBodyCollider()
	{
		this.setup.bodyCollider.SetActive(false);
		base.Invoke("enableBodyCollider", 5f);
	}

	private void enableBodyCollider()
	{
		this.setup.bodyCollider.SetActive(true);
	}

	private void resetTrapDrop()
	{
		this.animator.SetBoolReflected("dropFromTrap", false);
	}

	private void startDynamiteCoolDown()
	{
		if (!this.dynaCoolDown)
		{
			this.dynaCoolDown = true;
			base.Invoke("resetDynamiteCounter", 75f);
		}
	}

	private void resetDynamiteCounter()
	{
		this.setup.pmCombat.FsmVariables.GetFsmInt("dynamteCounter").Value = 0;
		this.dynaCoolDown = false;
	}

	private void disablePlayerCollision()
	{
		if (LocalPlayer.GameObject && this.controller.enabled)
		{
			if (LocalPlayer.AnimControl.playerCollider.enabled)
			{
				Physics.IgnoreCollision(this.controller, LocalPlayer.AnimControl.playerCollider, true);
			}
			if (LocalPlayer.AnimControl.playerHeadCollider.enabled)
			{
				Physics.IgnoreCollision(this.controller, LocalPlayer.AnimControl.playerHeadCollider, true);
			}
			if (LocalPlayer.AnimControl.enemyCollider.enabled)
			{
				Physics.IgnoreCollision(this.controller, LocalPlayer.AnimControl.enemyCollider, true);
			}
		}
		this.playerCollideDisabled = true;
	}

	private void enablePlayerCollision()
	{
		if (this.playerCollideDisabled)
		{
			if (LocalPlayer.GameObject && this.controller.enabled)
			{
				if (LocalPlayer.AnimControl.playerCollider.enabled)
				{
					Physics.IgnoreCollision(this.controller, LocalPlayer.AnimControl.playerCollider, false);
				}
				if (LocalPlayer.AnimControl.playerHeadCollider.enabled)
				{
					Physics.IgnoreCollision(this.controller, LocalPlayer.AnimControl.playerHeadCollider, false);
				}
				if (LocalPlayer.AnimControl.enemyCollider.enabled)
				{
					Physics.IgnoreCollision(this.controller, LocalPlayer.AnimControl.enemyCollider, false);
				}
			}
			this.playerCollideDisabled = false;
		}
	}

	private void runGotHitScripts()
	{
		this.setup.familyFunctions.sendCancelEvent();
		this.events.disableAllWeapons();
		this.events.bombDisable();
		this.events.enableCollision();
		this.ai.forceTreeDown();
		this.setup.familyFunctions.resetFamilyParams();
		this.setup.familyFunctions.sendAggressive();
		GameObject value = this.setup.pmCombat.FsmVariables.GetFsmGameObject("currentMemberGo").Value;
		this.setup.health.getCurrentHealth();
	}
}
