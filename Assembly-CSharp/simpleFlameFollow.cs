using System;
using UnityEngine;

public class simpleFlameFollow : MonoBehaviour
{
	public Transform followTarget;

	public Transform dummyTarget;

	private Vector3 origPos;

	private float startPos;

	private Vector3 pos;

	private Vector3 velRef;

	public float finalSmoothTime = 6f;

	public float followDistance = 0.03f;

	public bool lockXTranslate;

	public bool lockYTranslate;

	public bool lockZTranslate;

	private void Awake()
	{
		this.startPos = base.transform.localPosition.y;
		this.origPos = base.transform.localPosition;
	}

	private void Start()
	{
		this.pos = base.transform.position;
	}

	private void LateUpdate()
	{
		this.pos = Vector3.Lerp(this.pos, this.dummyTarget.position, Time.deltaTime * 25f);
		this.followTarget.position = this.pos;
		if (Vector3.Distance(this.followTarget.position, this.dummyTarget.position) > this.followDistance)
		{
			Vector3 vector = this.followTarget.position - this.dummyTarget.position;
			vector = Vector3.ClampMagnitude(vector, this.followDistance);
			this.followTarget.position = this.dummyTarget.position + vector;
		}
		base.transform.localPosition = Vector3.Slerp(base.transform.localPosition, this.followTarget.localPosition, Time.deltaTime * this.finalSmoothTime);
		float x = base.transform.localPosition.x;
		float y = base.transform.localPosition.y;
		float z = base.transform.localPosition.z;
		if (this.lockXTranslate)
		{
			x = this.origPos.x;
		}
		if (this.lockYTranslate)
		{
			y = this.origPos.y;
		}
		if (this.lockZTranslate)
		{
			z = this.origPos.z;
		}
		base.transform.localPosition = new Vector3(x, y, z);
	}
}
