using System;
using UnityEngine;

public class crocodileAnimatorControl : MonoBehaviour
{
	private Animator animator;

	private animalAvoidance avoidance;

	private inWaterChecker waterChecker;

	private animalAI ai;

	private Transform Tr;

	private Transform rotateTr;

	private Vector3 wantedDir;

	private Quaternion finalRot;

	private AnimatorStateInfo currLayerState0;

	private AnimatorStateInfo nextLayerState0;

	private float terrainPos;

	public float gravity;

	private Vector3 moveDir = Vector3.zero;

	public bool turnClose;

	public bool blocked;

	public Collider blockCollider;

	private Vector3 pos;

	private float tx;

	private float tz;

	private Vector3 tNormal;

	private float animSpeed;

	private void Awake()
	{
		this.ai = base.transform.parent.GetComponent<animalAI>();
		this.avoidance = base.GetComponentInChildren<animalAvoidance>();
		this.animator = base.GetComponent<Animator>();
		this.waterChecker = base.GetComponentInChildren<inWaterChecker>();
		this.Tr = base.transform.parent;
		this.rotateTr = base.transform;
	}

	private void Start()
	{
		this.ai.playMaker.FsmVariables.GetFsmGameObject("animatorGO").Value = base.gameObject;
		this.ai.animatorTr = base.transform;
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
			this.rotateTr.rotation = rotation;
			this.Tr.Translate(this.wantedDir * Time.deltaTime * (this.animator.GetFloat("Speed") * 4f), Space.World);
		}
	}

	private void OnAnimatorMove()
	{
		if (this.animator.enabled)
		{
			this.moveDir = this.animator.deltaPosition;
			if (this.blocked)
			{
				Vector3 currNormal = this.avoidance.currNormal;
				currNormal.y = 0f;
				currNormal.Normalize();
				if (currNormal.sqrMagnitude > 0.03f)
				{
					Vector3 vector = this.avoidance.transform.InverseTransformPoint(this.avoidance.currPoint);
					Vector3 a = Vector3.Cross(currNormal, Vector3.up);
					Debug.DrawRay(this.avoidance.currPoint, a * 5f, Color.blue);
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
					this.animSpeed = this.animator.GetFloat("Speed") * 5f;
					this.animSpeed = Mathf.Clamp(this.animSpeed, 0.5f, 6f);
					Vector3 to = this.Tr.position + a * d;
					Debug.DrawRay(this.Tr.position, a * d * 5f, Color.red);
					this.Tr.position = Vector3.Slerp(this.Tr.position, to, this.animSpeed * Time.deltaTime);
				}
			}
			else
			{
				this.animator.applyRootMotion = true;
				this.Tr.position += this.moveDir;
			}
			float num;
			if (this.waterChecker.swimming)
			{
				num = 0.2f;
				float y = this.Tr.position.y;
				this.terrainPos = Mathf.Lerp(y, this.waterChecker.waterHeight, Time.deltaTime * num);
			}
			else
			{
				this.terrainPos = Terrain.activeTerrain.SampleHeight(this.Tr.position) + Terrain.activeTerrain.transform.position.y;
				num = 3f;
			}
			this.Tr.position = new Vector3(this.Tr.position.x, this.terrainPos, this.Tr.position.z);
			this.tx = (this.Tr.position.x - Terrain.activeTerrain.transform.position.x) / Terrain.activeTerrain.terrainData.size.x;
			this.tz = (this.Tr.position.z - Terrain.activeTerrain.transform.position.z) / Terrain.activeTerrain.terrainData.size.z;
			if (this.waterChecker.swimming)
			{
				this.tNormal = Vector3.up;
			}
			else
			{
				this.tNormal = Terrain.activeTerrain.terrainData.GetInterpolatedNormal(this.tx, this.tz);
			}
			this.finalRot = Quaternion.Lerp(this.animator.rootRotation, Quaternion.LookRotation(Vector3.Cross(this.rotateTr.right, this.tNormal), this.tNormal), Time.deltaTime * num);
		}
	}

	private void LateUpdate()
	{
		if (this.animator.enabled)
		{
			this.rotateTr.rotation = this.finalRot;
		}
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
