using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class mutantBabyAnimatorControl : MonoBehaviour
{
	private Animator animator;

	private CharacterController controller;

	private Transform thisTr;

	private Transform rootTr;

	private mutantAI ai;

	private mutantScriptSetup setup;

	private PlayMakerFSM pmSearch;

	public float gravity;

	private Vector3 moveDir = Vector3.zero;

	private float currYPos;

	private float velY;

	private float accelY;

	private float creepyAnger;

	public float offScreenSpeed;

	public float moveSpeed;

	private Vector3 pos;

	private RaycastHit hit;

	private int layer;

	private int layerMask;

	private bool initBool;

	private float terrainPosY;

	private FsmBool fsmInCaveBool;

	private void OnDeserialized()
	{
		this.doStart();
	}

	private void Start()
	{
		this.doStart();
	}

	private void doStart()
	{
		this.setup = base.GetComponent<mutantScriptSetup>();
		this.animator = base.GetComponent<Animator>();
		this.controller = base.transform.parent.GetComponent<CharacterController>();
		this.thisTr = base.transform;
		this.rootTr = base.transform.parent;
		this.ai = base.GetComponent<mutantAI>();
		this.fsmInCaveBool = this.setup.pmCombat.FsmVariables.GetFsmBool("inCaveBool");
		this.layer = 26;
		this.layerMask = 1 << this.layer;
		this.setup.pmCombat.FsmVariables.GetFsmInt("HashIdle").Value = Animator.StringToHash("idle");
		this.setup.pmCombat.FsmVariables.GetFsmInt("HashWalk").Value = Animator.StringToHash("walk");
		this.setup.pmCombat.FsmVariables.GetFsmInt("HashAttack").Value = Animator.StringToHash("attack");
		base.Invoke("callChangeIdle", (float)UnityEngine.Random.Range(0, 2));
	}

	private void OnEnable()
	{
		base.Invoke("initAnimator", 0.5f);
	}

	private void OnDisable()
	{
		base.StopAllCoroutines();
		base.CancelInvoke("callChangeIdle");
		base.CancelInvoke("initAnimator");
		this.initBool = false;
	}

	private void initAnimator()
	{
		this.initBool = true;
	}

	private void Update()
	{
		if (!this.animator.enabled && this.ai.doMove)
		{
			this.controller.enabled = false;
			Quaternion rotation = Quaternion.identity;
			Vector3 wantedDir = this.ai.wantedDir;
			wantedDir.y = 0f;
			if (wantedDir != Vector3.zero && wantedDir.sqrMagnitude > 0f)
			{
				rotation = Quaternion.LookRotation(wantedDir, Vector3.up);
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
				this.rootTr.Translate(this.ai.wantedDir * Time.deltaTime * this.offScreenSpeed, Space.World);
				this.rootTr.position = new Vector3(this.rootTr.position.x, this.terrainPosY, this.rootTr.position.z);
			}
		}
		else
		{
			this.controller.enabled = true;
		}
	}

	private void callChangeIdle()
	{
		base.StartCoroutine("changeIdleValue");
	}

	[DebuggerHidden]
	private IEnumerator changeIdleValue()
	{
		mutantBabyAnimatorControl.<changeIdleValue>c__Iterator6A <changeIdleValue>c__Iterator6A = new mutantBabyAnimatorControl.<changeIdleValue>c__Iterator6A();
		<changeIdleValue>c__Iterator6A.<>f__this = this;
		return <changeIdleValue>c__Iterator6A;
	}

	private void OnAnimatorMove()
	{
		if (this.animator && this.animator.enabled)
		{
			float @float = this.animator.GetFloat("Gravity");
			this.moveDir = this.animator.deltaPosition;
			this.moveDir.y = this.moveDir.y - this.gravity * Time.deltaTime * @float;
			this.controller.Move(this.moveDir);
		}
	}

	private void LateUpdate()
	{
		this.pos = this.rootTr.position;
		this.pos.y = this.pos.y + 3f;
		if (this.ai.creepy && Physics.Raycast(this.pos, Vector3.down, out this.hit, 20f, this.layerMask))
		{
			this.thisTr.rotation = Quaternion.Lerp(this.animator.rootRotation, Quaternion.LookRotation(Vector3.Cross(this.thisTr.right, this.hit.normal), this.hit.normal), Time.deltaTime * 10f);
		}
	}
}
