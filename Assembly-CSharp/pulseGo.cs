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
		pulseGo.<doPulse>c__IteratorE7 <doPulse>c__IteratorE = new pulseGo.<doPulse>c__IteratorE7();
		<doPulse>c__IteratorE.<>f__this = this;
		return <doPulse>c__IteratorE;
	}
}
