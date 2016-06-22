using System;
using TheForest.Utils;
using UnityEngine;

public class alignMolotovFire : MonoBehaviour
{
	public Transform target;

	public Transform followTarget;

	public Transform dummyTarget;

	public float xOffset;

	private Transform origParent;

	private float currRot;

	private float prevRot;

	private float startPos;

	private Vector3 pos;

	private Vector3 velRef;

	public float smoothTime = 15f;

	public float followDistance = 0.25f;

	public bool net;

	private void Awake()
	{
		this.startPos = base.transform.localPosition.y;
		this.origParent = base.transform.parent;
	}

	private void Start()
	{
		this.pos = base.transform.position;
	}

	private void LateUpdate()
	{
		if (LocalPlayer.MainCamTr && !this.target)
		{
			this.target = LocalPlayer.MainCam.transform.FindChild("followMe").transform;
		}
		if (this.net)
		{
			if (this.target)
			{
				this.origParent.rotation = this.target.rotation;
			}
			return;
		}
		this.origParent.rotation = this.target.rotation;
		this.pos = Vector3.Slerp(this.pos, this.dummyTarget.position, Time.deltaTime * 18f);
		this.followTarget.position = this.pos;
		if (Vector3.Distance(this.followTarget.position, this.dummyTarget.position) > this.followDistance)
		{
			Vector3 vector = this.followTarget.position - this.dummyTarget.position;
			vector = Vector3.ClampMagnitude(vector, this.followDistance);
			this.followTarget.position = this.dummyTarget.position + vector;
		}
		base.transform.localPosition = Vector3.Slerp(base.transform.localPosition, this.followTarget.localPosition, Time.deltaTime * this.smoothTime);
	}
}
