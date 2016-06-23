using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class MoveTargetDemo : MonoBehaviour
{
	private Transform xform;

	private bool moveForward = true;

	private float speed = 20f;

	private float duration = 0.6f;

	private float delay = 3f;

	private void Start()
	{
		this.xform = base.transform;
		base.StartCoroutine(this.MoveTarget());
	}

	[DebuggerHidden]
	private IEnumerator MoveTarget()
	{
		MoveTargetDemo.<MoveTarget>c__Iterator20A <MoveTarget>c__Iterator20A = new MoveTargetDemo.<MoveTarget>c__Iterator20A();
		<MoveTarget>c__Iterator20A.<>f__this = this;
		return <MoveTarget>c__Iterator20A;
	}
}
