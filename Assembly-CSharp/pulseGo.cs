using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class pulseGo : MonoBehaviour
{
	public float interval;

	private void Start()
	{
		base.InvokeRepeating("initPulse", 0f, this.interval);
	}

	private void initPulse()
	{
		base.StartCoroutine("doPulse");
	}

	[DebuggerHidden]
	private IEnumerator doPulse()
	{
		pulseGo.<doPulse>c__IteratorEA <doPulse>c__IteratorEA = new pulseGo.<doPulse>c__IteratorEA();
		<doPulse>c__IteratorEA.<>f__this = this;
		return <doPulse>c__IteratorEA;
	}
}
