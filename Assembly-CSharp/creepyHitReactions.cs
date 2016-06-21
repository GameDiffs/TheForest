using System;
using UnityEngine;

public class creepyHitReactions : MonoBehaviour
{
	private Transform tr;

	private mutantScriptSetup setup;

	private void Start()
	{
		this.tr = base.transform;
		this.setup = base.transform.GetComponentInChildren<mutantScriptSetup>();
	}

	private void lookAtExplosion(Vector3 pos)
	{
		Vector3 worldPosition = pos;
		worldPosition.y = this.tr.position.y;
		this.tr.LookAt(worldPosition, this.tr.up);
	}

	private void staggerFromHit()
	{
		this.setup.animator.SetTriggerReflected("hitTrigger");
	}
}
