using PathologicalGames;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class playerNoiseDetection : MonoBehaviour
{
	private TargetTracker tracker;

	public GameObject soundDetect;

	private SphereCollider soundCollider;

	private float pulseInterval;

	private float initRange;

	private void Awake()
	{
		if (this.soundDetect)
		{
			this.soundCollider = this.soundDetect.GetComponent<SphereCollider>();
		}
		this.initRange = this.soundCollider.radius;
	}

	private void Start()
	{
		this.pulseInterval = 0.35f;
		base.InvokeRepeating("initPulse", 0f, this.pulseInterval);
	}

	[DebuggerHidden]
	private IEnumerator setNoiseRange(float range)
	{
		playerNoiseDetection.<setNoiseRange>c__IteratorDE <setNoiseRange>c__IteratorDE = new playerNoiseDetection.<setNoiseRange>c__IteratorDE();
		<setNoiseRange>c__IteratorDE.range = range;
		<setNoiseRange>c__IteratorDE.<$>range = range;
		<setNoiseRange>c__IteratorDE.<>f__this = this;
		return <setNoiseRange>c__IteratorDE;
	}

	[DebuggerHidden]
	private IEnumerator resetNoiseRange()
	{
		playerNoiseDetection.<resetNoiseRange>c__IteratorDF <resetNoiseRange>c__IteratorDF = new playerNoiseDetection.<resetNoiseRange>c__IteratorDF();
		<resetNoiseRange>c__IteratorDF.<>f__this = this;
		return <resetNoiseRange>c__IteratorDF;
	}

	private void initPulse()
	{
		base.StartCoroutine("doPulse");
	}

	[DebuggerHidden]
	private IEnumerator doPulse()
	{
		playerNoiseDetection.<doPulse>c__IteratorE0 <doPulse>c__IteratorE = new playerNoiseDetection.<doPulse>c__IteratorE0();
		<doPulse>c__IteratorE.<>f__this = this;
		return <doPulse>c__IteratorE;
	}
}
