using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ParticleSystemDestroyer : MonoBehaviour
{
	public float minDuration = 8f;

	public float maxDuration = 10f;

	private float maxLifetime;

	private bool earlyStop;

	[DebuggerHidden]
	private IEnumerator Start()
	{
		ParticleSystemDestroyer.<Start>c__Iterator121 <Start>c__Iterator = new ParticleSystemDestroyer.<Start>c__Iterator121();
		<Start>c__Iterator.<>f__this = this;
		return <Start>c__Iterator;
	}

	public void Stop()
	{
		this.earlyStop = true;
	}
}
