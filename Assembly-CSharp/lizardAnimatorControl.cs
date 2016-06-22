using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class lizardAnimatorControl : MonoBehaviour
{
	private Animator animator;

	private animalAI ai;

	private animalAvoidance avoidance;

	private Transform Tr;

	private Vector3 wantedDir;

	private AnimatorStateInfo currLayerState0;

	private AnimatorStateInfo nextLayerState0;

	public float gravity;

	private Vector3 moveDir = Vector3.zero;

	private float terrainPos;

	private float tx;

	private float tz;

	private Vector3 tNormal;

	public bool blocked;

	public Collider blockCollider;

	private float animSpeed;

	private FsmBool fsmTreeBool;

	private RaycastHit hit;

	private Vector3 pos;

	private void Awake()
	{
		this.ai = base.GetComponent<animalAI>();
		this.animator = base.GetComponent<Animator>();
		this.avoidance = base.transform.GetComponentInChildren<animalAvoidance>();
		this.Tr = base.transform;
	}

	private void Start()
	{
		this.fsmTreeBool = this.ai.playMaker.FsmVariables.GetFsmBool("treeBool");
	}

	private void checkVis()
	{
		if (!this.animator.enabled)
		{
			this.ai.playMaker.SendEvent("notVisible");
		}
		else
		{
			this.ai.playMaker.SendEvent("visible");
		}
	}

	private void Update()
	{
		if (!this.animator.enabled && this.ai.doMove)
		{
			Quaternion rotation = Quaternion.identity;
			this.wantedDir = this.ai.wantedDir;
			if (this.ai.wantedDir != Vector3.zero)
			{
				Vector3 forward = this.ai.wantedDir;
				forward.y = 0f;
				rotation = Quaternion.LookRotation(forward, Vector3.up);
			}
			this.Tr.rotation = rotation;
			this.Tr.Translate(this.wantedDir * Time.deltaTime * (this.animator.GetFloat("Speed") * 4f), Space.World);
		}
	}

	private void OnAnimatorMove()
	{
		if (this.animator.enabled)
		{
			this.moveDir = this.animator.deltaPosition;
			if (this.blocked && !this.fsmTreeBool.Value)
			{
				Vector3 currNormal = this.avoidance.currNormal;
				currNormal.y = 0f;
				currNormal.Normalize();
				if (currNormal.sqrMagnitude > 0.03f)
				{
					Vector3 vector = this.avoidance.transform.InverseTransformPoint(this.avoidance.currPoint);
					Vector3 a = Vector3.Cross(currNormal, Vector3.up);
					UnityEngine.Debug.DrawRay(this.avoidance.currPoint, a * 5f, Color.blue);
					this.animator.applyRootMotion = false;
					float d = 0f;
					if (vector.x < 0f)
					{
						d = 1f;
					}
					else if (vector.x > 0f)
					{
						d = -1f;
					}
					this.animSpeed = this.animator.GetFloat("Speed") * 3f;
					this.animSpeed = Mathf.Clamp(this.animSpeed, 0.2f, 6f);
					Vector3 to = this.Tr.position + a * d;
					UnityEngine.Debug.DrawRay(this.Tr.position, a * d * 5f, Color.red);
					this.Tr.position = Vector3.Slerp(this.Tr.position, to, this.animSpeed * Time.deltaTime);
				}
			}
			else
			{
				this.animator.applyRootMotion = true;
				this.Tr.position += this.moveDir;
			}
			if (!this.animator.GetBool("Tree"))
			{
				this.terrainPos = Terrain.activeTerrain.SampleHeight(this.Tr.position) + Terrain.activeTerrain.transform.position.y;
				this.Tr.position = new Vector3(this.Tr.position.x, this.terrainPos, this.Tr.position.z);
				this.tx = (this.Tr.position.x - Terrain.activeTerrain.transform.position.x) / Terrain.activeTerrain.terrainData.size.x;
				this.tz = (this.Tr.position.z - Terrain.activeTerrain.transform.position.z) / Terrain.activeTerrain.terrainData.size.z;
				this.tNormal = Terrain.activeTerrain.terrainData.GetInterpolatedNormal(this.tx, this.tz);
				this.Tr.rotation = Quaternion.Lerp(this.animator.rootRotation, Quaternion.LookRotation(Vector3.Cross(base.transform.right, this.tNormal), this.tNormal), Time.deltaTime * 10f);
			}
			else
			{
				this.Tr.rotation = Quaternion.Lerp(this.Tr.rotation, Quaternion.LookRotation(Vector3.Cross(base.transform.right, Vector3.up), Vector3.up), Time.deltaTime * 4f);
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator moveToTreeTarget(Vector3 pos)
	{
		lizardAnimatorControl.<moveToTreeTarget>c__Iterator62 <moveToTreeTarget>c__Iterator = new lizardAnimatorControl.<moveToTreeTarget>c__Iterator62();
		<moveToTreeTarget>c__Iterator.pos = pos;
		<moveToTreeTarget>c__Iterator.<$>pos = pos;
		<moveToTreeTarget>c__Iterator.<>f__this = this;
		return <moveToTreeTarget>c__Iterator;
	}

	private void enableBlocked()
	{
		this.blocked = true;
	}

	private void disableBlocked()
	{
		this.blocked = false;
	}
}
