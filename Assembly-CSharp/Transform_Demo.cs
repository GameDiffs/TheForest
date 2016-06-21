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
		Transform_Demo.<RotateTarget>c__Iterator204 <RotateTarget>c__Iterator = new Transform_Demo.<RotateTarget>c__Iterator204();
		<RotateTarget>c__Iterator.<>f__this = this;
		return <RotateTarget>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator MoveTarget()
	{
		Transform_Demo.<MoveTarget>c__Iterator205 <MoveTarget>c__Iterator = new Transform_Demo.<MoveTarget>c__Iterator205();
		<MoveTarget>c__Iterator.<>f__this = this;
		return <MoveTarget>c__Iterator;
	}
}
