using System;
using UnityEngine;

public class lookAtTerrain : MonoBehaviour
{
	public Transform endTr;

	private Transform tr;

	private float terrainPos;

	private Vector3 lookAtPos;

	private Vector3 samplePos;

	public float damping;

	public BoxCollider sledCollider;

	private int layerMask;

	private RaycastHit hit;

	private void Start()
	{
		this.tr = base.transform;
		this.layerMask = 102768640;
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	public void setSledCollider(BoxCollider col)
	{
		this.sledCollider = col;
	}

	public void resetSledCollider()
	{
		this.sledCollider = null;
	}

	private void FixedUpdate()
	{
		this.samplePos = this.endTr.position;
		this.samplePos.y = this.samplePos.y + 6f;
		if (Physics.Raycast(this.samplePos, Vector3.down, out this.hit, 30f, this.layerMask))
		{
			this.terrainPos = this.hit.point.y + 0.2f;
			this.lookAtPos = new Vector3(this.endTr.position.x, this.terrainPos, this.endTr.position.z);
			Quaternion quaternion = Quaternion.LookRotation(this.lookAtPos - this.tr.position);
			this.tr.rotation = quaternion;
			this.tr.rotation = Quaternion.Lerp(this.tr.rotation, quaternion, Time.deltaTime * this.damping);
		}
	}
}
