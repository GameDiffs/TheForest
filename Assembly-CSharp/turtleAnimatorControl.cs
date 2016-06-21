using System;
using UnityEngine;

public class turtleAnimatorControl : MonoBehaviour
{
	private turtleAvoidance avoidance;

	private Animator animator;

	private animalAI ai;

	private Transform Tr;

	private Vector3 wantedDir;

	private AnimatorStateInfo currLayerState0;

	private AnimatorStateInfo nextLayerState0;

	public float gravity;

	private Vector3 moveDir = Vector3.zero;

	private float terrainPosY;

	private Quaternion rot;

	private Transform ocean;

	private float oceanHeight;

	public float oceanDepth;

	public float swimSpeed;

	public float depthSpeed;

	private bool doMove;

	public bool blocked;

	public Collider blockedCollider;

	public float animSpeed = 0.5f;

	private int layer;

	private int layerMask;

	private RaycastHit hit;

	private Vector3 pos;

	private Vector3 tNormal;

	private float tx;

	private float tz;

	private void Start()
	{
		this.avoidance = base.transform.GetComponentInChildren<turtleAvoidance>();
		GameObject gameObject = GameObject.FindWithTag("OceanHeight");
		if (gameObject)
		{
			this.ocean = gameObject.transform;
		}
		this.ai = base.GetComponent<animalAI>();
		this.animator = base.GetComponent<Animator>();
		this.Tr = base.transform;
		base.InvokeRepeating("randomDepth", 1f, UnityEngine.Random.Range(8f, 15f));
		this.layer = 26;
		this.layerMask = 1 << this.layer;
	}

	private void Update()
	{
		if (!this.animator.enabled && this.ai.doMove)
		{
			this.terrainPosY = Terrain.activeTerrain.SampleHeight(this.Tr.position) + Terrain.activeTerrain.transform.position.y;
			this.rot = Quaternion.identity;
			this.wantedDir = this.ai.wantedDir;
			if (this.ai.wantedDir != Vector3.zero)
			{
				Vector3 forward = this.ai.wantedDir;
				forward.y = 0f;
				this.rot = Quaternion.LookRotation(forward, Vector3.up);
			}
			this.Tr.rotation = this.rot;
			this.Tr.Translate(this.wantedDir * Time.deltaTime, Space.World);
		}
		if (this.doMove)
		{
			this.Tr.Translate(Vector3.forward * this.swimSpeed * 0.1f);
		}
		if (!this.doMove)
		{
			this.tx = (this.Tr.position.x - Terrain.activeTerrain.transform.position.x) / Terrain.activeTerrain.terrainData.size.x;
			this.tz = (this.Tr.position.z - Terrain.activeTerrain.transform.position.z) / Terrain.activeTerrain.terrainData.size.z;
			this.tNormal = Terrain.activeTerrain.terrainData.GetInterpolatedNormal(this.tx, this.tz);
			this.Tr.rotation = Quaternion.Lerp(this.animator.rootRotation, Quaternion.LookRotation(Vector3.Cross(this.Tr.right, this.tNormal), this.tNormal), Time.deltaTime * 10f);
		}
	}

	private void enableSwimMove()
	{
		this.doMove = true;
	}

	private void randomDepth()
	{
	}

	private void OnAnimatorMove()
	{
		if (this.animator)
		{
			this.moveDir = this.animator.deltaPosition;
			if (this.blocked && this.blockedCollider)
			{
				Vector3 vector = this.avoidance.transform.InverseTransformPoint(this.blockedCollider.transform.position);
				Vector3 normalized = (this.Tr.position - this.blockedCollider.transform.position).normalized;
				Vector3 vector2 = Vector3.Cross(normalized, Vector3.up);
				this.animator.applyRootMotion = false;
				if (vector.x >= 0f)
				{
					if (vector.x > 0f)
					{
					}
				}
				this.Tr.position += normalized * this.animSpeed * Time.deltaTime;
			}
			else
			{
				this.Tr.position += this.moveDir;
				this.animator.applyRootMotion = true;
			}
			if (!this.ocean)
			{
				return;
			}
			this.oceanHeight = this.Tr.position.y - this.ocean.position.y;
			this.terrainPosY = Terrain.activeTerrain.SampleHeight(this.Tr.position) + Terrain.activeTerrain.transform.position.y;
			if (this.oceanHeight > -0.9f)
			{
				if (this.terrainPosY != this.Tr.position.y)
				{
					this.terrainPosY = Mathf.Lerp(this.Tr.position.y, this.terrainPosY, Time.deltaTime);
					this.Tr.position = new Vector3(this.Tr.position.x, this.terrainPosY, this.Tr.position.z);
				}
				this.Tr.position = new Vector3(this.Tr.position.x, this.terrainPosY, this.Tr.position.z);
				this.animator.SetBoolReflected("swimming", false);
				this.doMove = false;
			}
			else
			{
				this.oceanDepth = this.ocean.position.y - this.terrainPosY;
				if (this.oceanDepth > 0.5f)
				{
					this.terrainPosY = Mathf.Lerp(this.Tr.position.y, this.terrainPosY + this.oceanDepth / 2f - 0.3f, Time.deltaTime * this.depthSpeed);
					this.Tr.position = new Vector3(this.Tr.position.x, this.terrainPosY, this.Tr.position.z);
				}
				this.animator.SetBoolReflected("swimming", true);
				this.ai.fsmRotateSpeed.Value = 0.6f;
				base.Invoke("enableSwimMove", 1f);
			}
		}
	}
}
