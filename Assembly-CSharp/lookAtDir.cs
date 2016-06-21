using System;
using UnityEngine;

public class lookAtDir : MonoBehaviour
{
	private Transform thisTr;

	public Transform rootTr;

	public Transform endTr;

	private Transform lookTr;

	public float xOffset;

	public float yOffset;

	public float zOffset;

	private void Start()
	{
		this.thisTr = base.transform;
		GameObject gameObject = new GameObject();
		this.lookTr = gameObject.transform;
		this.lookTr.position = this.endTr.position;
		this.lookTr.parent = this.rootTr;
	}

	private void FixedUpdate()
	{
		this.thisTr.LookAt(this.lookTr.position, this.rootTr.up);
	}
}
