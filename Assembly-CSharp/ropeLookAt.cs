using System;
using UnityEngine;

public class ropeLookAt : MonoBehaviour
{
	private Transform thisTr;

	public Transform rootTr;

	public Transform endTr;

	private Transform lookTr;

	public float xOffset;

	public float yOffset;

	public float zOffset;

	private float initDist;

	private float dist;

	private void Start()
	{
		this.thisTr = base.transform;
		this.initDist = Vector3.Distance(this.rootTr.position, this.endTr.position);
	}

	private void Update()
	{
		this.thisTr.LookAt(this.endTr.position, this.rootTr.up);
		this.dist = Vector3.Distance(this.rootTr.position, this.endTr.position);
		this.thisTr.localScale = new Vector3(1f, 1f, this.dist / this.initDist);
	}
}
