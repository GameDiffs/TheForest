using PathologicalGames;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ScriptedExample : MonoBehaviour
{
	public float moveSpeed = 1f;

	public float turnSpeed = 1f;

	public float newDirectionInterval = 3f;

	private SmoothLookAtConstraint lookCns;

	private TransformConstraint xformCns;

	private Transform xform;

	private void Awake()
	{
		this.xform = base.transform;
		this.xformCns = base.gameObject.AddComponent<TransformConstraint>();
		this.xformCns.noTargetMode = UnityConstraints.NO_TARGET_OPTIONS.SetByScript;
		this.xformCns.constrainRotation = false;
		this.lookCns = base.gameObject.AddComponent<SmoothLookAtConstraint>();
		this.lookCns.noTargetMode = UnityConstraints.NO_TARGET_OPTIONS.SetByScript;
		this.lookCns.pointAxis = Vector3.up;
		this.lookCns.upAxis = Vector3.forward;
		this.lookCns.speed = this.turnSpeed;
		base.StartCoroutine(this.LookAtRandom());
		base.StartCoroutine(this.MoveRandom());
	}

	[DebuggerHidden]
	private IEnumerator MoveRandom()
	{
		ScriptedExample.<MoveRandom>c__Iterator202 <MoveRandom>c__Iterator = new ScriptedExample.<MoveRandom>c__Iterator202();
		<MoveRandom>c__Iterator.<>f__this = this;
		return <MoveRandom>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator LookAtRandom()
	{
		ScriptedExample.<LookAtRandom>c__Iterator203 <LookAtRandom>c__Iterator = new ScriptedExample.<LookAtRandom>c__Iterator203();
		<LookAtRandom>c__Iterator.<>f__this = this;
		return <LookAtRandom>c__Iterator;
	}
}
