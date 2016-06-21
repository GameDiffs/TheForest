using HutongGames.PlayMaker;
using Pathfinding;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class mutantCollisionDetect : MonoBehaviour
{
	private Transform rootTr;

	public Transform rayCastPos;

	private mutantScriptSetup setup;

	private Animator animator;

	private Collider hitCollider;

	public float terrainDist;

	private FsmBool fsmEnemyCloseBool;

	private FsmBool fsmFearOverrideBool;

	private FsmBool fsmDeathBool;

	public GameObject currentTrigger;

	public Collider currentJumpCollider;

	private Vector3 localTarget;

	public float targetAngle;

	private float targetDist;

	private bool lighterCooldown;

	private bool detected;

	public bool inEffigy;

	private bool inPlayerFire;

	public bool jumpBlock;

	private bool triggerCoolDown;

	private Vector3 currentPos;

	private Vector3 nextPos;

	private Vector3 pos;

	private Vector3 origColliderPos;

	private RaycastHit hit;

	private int layer;

	private int layerMask;

	public bool doCollision;

	private GraphNode node;

	private void Awake()
	{
		this.setup = base.transform.root.GetComponentInChildren<mutantScriptSetup>();
		this.rootTr = base.transform.root.GetChild(0).transform;
		this.animator = base.transform.root.GetComponentInChildren<Animator>();
		this.hitCollider = base.GetComponent<Collider>();
	}

	private void Start()
	{
		this.fsmDeathBool = this.setup.pmCombat.FsmVariables.GetFsmBool("deathBool");
		this.fsmEnemyCloseBool = this.setup.pmCombat.FsmVariables.GetFsmBool("enemyCloseBOOL");
		this.fsmFearOverrideBool = this.setup.pmBrain.FsmVariables.GetFsmBool("fearOverrideBool");
		this.layer = 26;
		this.layerMask = 1 << this.layer;
		this.origColliderPos = base.transform.localPosition;
		base.Invoke("enableCollisions", 2f);
		this.hitCollider.enabled = true;
	}

	private void enableCollisions()
	{
		this.doCollision = true;
	}

	private void OnEnable()
	{
		if (this.animator && this.animator.enabled)
		{
			this.animator.SetBoolReflected("goLeftBOOL", false);
			this.animator.SetBoolReflected("goRightBOOL", false);
		}
		base.StartCoroutine("pollCollider");
		base.StartCoroutine("terrainHeightCheck");
		base.Invoke("enableCollisions", 2f);
	}

	private void OnDisable()
	{
		if (this.animator && this.animator.enabled)
		{
			this.animator.SetBoolReflected("goLeftBOOL", false);
			this.animator.SetBoolReflected("goRightBOOL", false);
		}
		base.StopCoroutine("resetJumpCollision");
		base.StopCoroutine("pollCollider");
		base.StopCoroutine("terrainHeightCheck");
	}

	private void Update()
	{
		if ((this.inEffigy || this.inPlayerFire) && (!this.currentTrigger || !this.currentTrigger.activeSelf))
		{
			this.forceExit();
		}
	}

	[DebuggerHidden]
	private IEnumerator terrainHeightCheck()
	{
		mutantCollisionDetect.<terrainHeightCheck>c__Iterator6B <terrainHeightCheck>c__Iterator6B = new mutantCollisionDetect.<terrainHeightCheck>c__Iterator6B();
		<terrainHeightCheck>c__Iterator6B.<>f__this = this;
		return <terrainHeightCheck>c__Iterator6B;
	}

	[DebuggerHidden]
	private IEnumerator pollCollider()
	{
		mutantCollisionDetect.<pollCollider>c__Iterator6C <pollCollider>c__Iterator6C = new mutantCollisionDetect.<pollCollider>c__Iterator6C();
		<pollCollider>c__Iterator6C.<>f__this = this;
		return <pollCollider>c__Iterator6C;
	}

	private void forceExit()
	{
		this.setup.pmCombat.FsmVariables.GetFsmBool("fearBOOL").Value = false;
		this.setup.pmBrain.SendEvent("toActivateFSM");
		base.Invoke("resetFlee", 15f);
		this.inEffigy = false;
		this.inPlayerFire = false;
	}

	private void doCoolDown()
	{
		if (!this.lighterCooldown)
		{
			this.lighterCooldown = true;
			base.Invoke("resetCoolDown", 20f);
		}
	}

	private void resetCoolDown()
	{
		this.lighterCooldown = false;
		UnityEngine.Debug.Log("COOLDOWN OFF.");
	}

	private void sidestepReset()
	{
		this.animator.SetBoolReflected("goLeftBOOL", false);
		this.animator.SetBoolReflected("goRightBOOL", false);
	}

	private void jumpCoolDown()
	{
		this.jumpBlock = false;
		this.animator.SetBoolReflected("jumpBOOL", false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!this.doCollision)
		{
			return;
		}
		if (other.gameObject.CompareTag("effigy") && !this.setup.ai.fireman && !this.animator.GetBool("trapBool") && !this.animator.GetBool("deathBOOL") && !this.fsmDeathBool.Value && !this.setup.health.onFire)
		{
			if (this.fsmFearOverrideBool.Value)
			{
				this.forceExit();
			}
			else
			{
				if (!this.inEffigy)
				{
					this.currentTrigger = other.gameObject;
				}
				this.setup.pmBrain.SendEvent("toSetFearful");
				this.setup.aiManager.flee = true;
				this.setup.pmBrain.FsmVariables.GetFsmGameObject("fearTargetGo").Value = other.gameObject;
				this.inEffigy = true;
			}
		}
		if (other.gameObject.CompareTag("explodeReact") && this.animator.GetInteger("hurtLevelInt") < 3 && !this.animator.GetBool("trapBool") && !this.animator.GetBool("deathBOOL") && !this.fsmDeathBool.Value && !this.setup.health.onFire && Vector3.Distance(other.transform.position, base.transform.position) > 15f)
		{
			this.currentTrigger = other.gameObject;
			this.setup.pmBrain.SendEvent("toSetRunAway");
			this.setup.pmBrain.FsmVariables.GetFsmGameObject("fearTargetGo").Value = other.gameObject;
		}
		if (other.gameObject.CompareTag("DeadTree") || other.gameObject.CompareTag("jumpObject") || other.gameObject.CompareTag("Multisled"))
		{
			if (other.GetType() == typeof(MeshCollider))
			{
				this.animator.SetIntegerReflected("randInt2", UnityEngine.Random.Range(0, 2));
				this.animator.SetBoolReflected("jumpBOOL", true);
				this.jumpBlock = true;
				this.currentJumpCollider = other;
				base.Invoke("jumpCoolDown", 0.37f);
			}
			else
			{
				this.localTarget = this.rootTr.InverseTransformPoint(other.transform.position);
			}
			this.targetAngle = Mathf.Atan2(this.localTarget.x, this.localTarget.z) * 57.29578f;
			if (this.targetAngle > -45f && this.targetAngle < 45f && !this.jumpBlock)
			{
				this.animator.SetIntegerReflected("randInt2", UnityEngine.Random.Range(0, 2));
				this.animator.SetBoolReflected("jumpBOOL", true);
				this.jumpBlock = true;
				this.currentJumpCollider = other;
				base.Invoke("jumpCoolDown", 0.37f);
			}
		}
		if (other.gameObject.CompareTag("Tree"))
		{
			this.targetDist = Vector3.Distance(this.rootTr.position, other.transform.position);
			if (this.targetDist < 5f)
			{
				this.localTarget = this.rootTr.InverseTransformPoint(other.transform.position);
				this.targetAngle = Mathf.Atan2(this.localTarget.x, this.localTarget.z) * 57.29578f;
				if (this.targetAngle < 0f && this.targetAngle > -70f)
				{
					this.animator.SetBoolReflected("goRightBOOL", true);
					base.Invoke("sidestepReset", 0.6f);
				}
				if (this.targetAngle > 0f && this.targetAngle < 70f)
				{
					this.animator.SetBoolReflected("goLeftBOOL", true);
					base.Invoke("sidestepReset", 0.6f);
				}
			}
		}
		if (other.gameObject.CompareTag("enemy"))
		{
			this.fsmEnemyCloseBool.Value = true;
		}
	}

	[DebuggerHidden]
	private IEnumerator resetJumpCollision(Collider other)
	{
		mutantCollisionDetect.<resetJumpCollision>c__Iterator6D <resetJumpCollision>c__Iterator6D = new mutantCollisionDetect.<resetJumpCollision>c__Iterator6D();
		<resetJumpCollision>c__Iterator6D.other = other;
		<resetJumpCollision>c__Iterator6D.<$>other = other;
		<resetJumpCollision>c__Iterator6D.<>f__this = this;
		return <resetJumpCollision>c__Iterator6D;
	}

	private void resetTriggerCoolDown()
	{
		this.triggerCoolDown = false;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("effigy") || other.gameObject.CompareTag("playerFire"))
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("fearBOOL").Value = false;
			if (!this.animator.GetBool("trapBool") && !this.animator.GetBool("deathBOOL") && !this.fsmDeathBool.Value && !this.setup.health.onFire)
			{
				this.setup.pmBrain.SendEvent("toActivateFSM");
			}
			this.inEffigy = false;
			this.inPlayerFire = false;
			base.Invoke("resetFlee", 15f);
		}
		if (other.gameObject.CompareTag("Obstruction") || other.gameObject.CompareTag("DeadTree") || other.gameObject.CompareTag("Tree") || other.gameObject.CompareTag("jumpObject") || other.gameObject.CompareTag("Multisled"))
		{
			this.animator.SetBoolReflected("jumpBOOL", false);
			this.animator.SetBoolReflected("goLeftBOOL", false);
			this.animator.SetBoolReflected("goRightBOOL", false);
		}
		if (other.gameObject.CompareTag("Obstruction"))
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("closeRockLeftBOOL").Value = false;
		}
		if (other.gameObject.CompareTag("Obstruction"))
		{
			this.setup.pmCombat.FsmVariables.GetFsmBool("closeRockRightBOOL").Value = false;
		}
		if (other.gameObject.CompareTag("enemy"))
		{
			this.fsmEnemyCloseBool.Value = false;
		}
	}

	private void resetFlee()
	{
		this.setup.aiManager.flee = false;
	}

	[DebuggerHidden]
	private IEnumerator returnObjectAngle(GameObject go)
	{
		mutantCollisionDetect.<returnObjectAngle>c__Iterator6E <returnObjectAngle>c__Iterator6E = new mutantCollisionDetect.<returnObjectAngle>c__Iterator6E();
		<returnObjectAngle>c__Iterator6E.go = go;
		<returnObjectAngle>c__Iterator6E.<$>go = go;
		<returnObjectAngle>c__Iterator6E.<>f__this = this;
		return <returnObjectAngle>c__Iterator6E;
	}
}
