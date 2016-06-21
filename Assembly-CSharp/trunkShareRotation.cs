using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class trunkShareRotation : MonoBehaviour
{
	public Transform sourceJoint;

	public Transform joint1;

	public Transform joint2;

	public float joint1Blend;

	public float joint2Blend;

	public float followDelay;

	public List<Quaternion> storeQuat = new List<Quaternion>();

	private void Start()
	{
		base.Invoke("startFollowJ1", this.followDelay);
		base.Invoke("startFollowJ2", this.followDelay + this.followDelay);
	}

	private void FixedUpdate()
	{
		this.storeQuat.Add(this.sourceJoint.localRotation);
	}

	private void startFollowJ1()
	{
		base.StartCoroutine("doFollowJ1");
	}

	private void startFollowJ2()
	{
		base.StartCoroutine("doFollowJ2");
	}

	[DebuggerHidden]
	private IEnumerator doFollowJ1()
	{
		trunkShareRotation.<doFollowJ1>c__Iterator100 <doFollowJ1>c__Iterator = new trunkShareRotation.<doFollowJ1>c__Iterator100();
		<doFollowJ1>c__Iterator.<>f__this = this;
		return <doFollowJ1>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator doFollowJ2()
	{
		trunkShareRotation.<doFollowJ2>c__Iterator101 <doFollowJ2>c__Iterator = new trunkShareRotation.<doFollowJ2>c__Iterator101();
		<doFollowJ2>c__Iterator.<>f__this = this;
		return <doFollowJ2>c__Iterator;
	}
}
