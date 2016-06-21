using System;
using UnityEngine;

public class boarAnimatorControl : MonoBehaviour
{
	private Animator animator;

	private animalAI ai;

	private animalAvoidance avoidance;

	private Transform Tr;

	private Transform rotateTr;

	private Vector3 wantedDir;

	private AnimatorStateInfo currLayerState0;

	private AnimatorStateInfo nextLayerState0;

	private float terrainPos;

	public float gravity;

	private Vector3 moveDir = Vector3.zero;

	public bool turnClose;

	public bool blocked;

	public Collider blockCollider;

	private RaycastHit hit;

	private RaycastHit hit2;

	private Vector3 pos;

	private float tx;

	private float tz;

	private Vector3 tNormal;

	private float animSpeed;

	private void Awake()
	{
		this.ai = base.transform.parent.GetComponent<animalAI>();
		this.animator = base.GetComponent<Animator>();
		this.avoidance = base.GetComponentInChildren<animalAvoidance>();
		this.Tr = base.transform.parent;
		this.rotateTr = base.transform;
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
					if (vector.z < 0f)
					{
						d = 1f;
					}
					else if (vector.z > 0f)
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
			this.terrainPos = Terrain.activeTerrain.SampleHeight(this.Tr.position) + Terrain.activeTerrain.transform.position.y;
			this.Tr.position = new Vector3(this.Tr.position.x, this.terrainPos, this.Tr.position.z);
			this.tx = (this.Tr.position.x - Terrain.activeTerrain.transform.position.x) / Terrain.activeTerrain.terrainData.size.x;
			this.tz = (this.Tr.position.z - Terrain.activeTerrain.transform.position.z) / Terrain.activeTerrain.terrainData.size.z;
			this.tNormal = Terrain.activeTerrain.terrainData.GetInterpolatedNormal(this.tx, this.tz);
			this.rotateTr.rotation = Quaternion.Lerp(this.animator.rootRotation, Quaternion.LookRotation(Vector3.Cross(this.rotateTr.right, this.tNormal), this.tNormal), Time.deltaTime * 10f);
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
