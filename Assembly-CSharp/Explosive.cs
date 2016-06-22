using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Explosive : MonoBehaviour
{
	public Transform explosionPrefab;

	private bool exploded;

	public float detonationImpactVelocity = 10f;

	public float sizeMultiplier = 1f;

	public bool reset = true;

	public float resetTimeDelay = 10f;

	private void Start()
	{
	}

	[DebuggerHidden]
	private IEnumerator OnCollisionEnter(Collision col)
	{
		Explosive.<OnCollisionEnter>c__Iterator122 <OnCollisionEnter>c__Iterator = new Explosive.<OnCollisionEnter>c__Iterator122();
		<OnCollisionEnter>c__Iterator.col = col;
		<OnCollisionEnter>c__Iterator.<$>col = col;
		<OnCollisionEnter>c__Iterator.<>f__this = this;
		return <OnCollisionEnter>c__Iterator;
	}

	public void Reset()
	{
		this.exploded = false;
	}
}
