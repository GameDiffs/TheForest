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
		Explosive.<OnCollisionEnter>c__Iterator11F <OnCollisionEnter>c__Iterator11F = new Explosive.<OnCollisionEnter>c__Iterator11F();
		<OnCollisionEnter>c__Iterator11F.col = col;
		<OnCollisionEnter>c__Iterator11F.<$>col = col;
		<OnCollisionEnter>c__Iterator11F.<>f__this = this;
		return <OnCollisionEnter>c__Iterator11F;
	}

	public void Reset()
	{
		this.exploded = false;
	}
}
