using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Transform_Demo : MonoBehaviour
{
	public Vector3 rotate = new Vector3(0f, 3f, 0f);

	private Transform xform;

	private bool moveForward = true;

	private float speed = 5f;

	private float duration = 0.6f;

	private float delay = 1.5f;

	private Vector3 bigScale = new Vector3(2f, 2f, 2f);

	private Vector3 smallScale = new Vector3(1f, 1f, 1f);

	private void Awake()
	{
		this.xform = base.transform;
	}

	private void Start()
	{
		base.StartCoroutine(this.MoveTarget());
		base.StartCoroutine(this.RotateTarget());
	}

	[DebuggerHidden]
	private IEnumerator RotateTarget()
	{
		Transform_Demo.<RotateTarget>c__Iterator20D <RotateTarget>c__Iterator20D = new Transform_Demo.<RotateTarget>c__Iterator20D();
		<RotateTarget>c__Iterator20D.<>f__this = this;
		return <RotateTarget>c__Iterator20D;
	}

	[DebuggerHidden]
	private IEnumerator MoveTarget()
	{
		Transform_Demo.<MoveTarget>c__Iterator20E <MoveTarget>c__Iterator20E = new Transform_Demo.<MoveTarget>c__Iterator20E();
		<MoveTarget>c__Iterator20E.<>f__this = this;
		return <MoveTarget>c__Iterator20E;
	}
}
