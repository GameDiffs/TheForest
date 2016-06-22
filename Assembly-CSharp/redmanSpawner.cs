using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class redmanSpawner : MonoBehaviour
{
	public GameObject redMan;

	public GameObject yachtSpawn;

	public GameObject caveSpawn1;

	public GameObject caveSpawn2;

	public GameObject caveSpawn3;

	public GameObject[] cliffSpawn;

	public int canSeeRedDay;

	private float leaveTime;

	private Animator animator;

	[SerializeThis]
	public bool doneYacht;

	[SerializeThis]
	public bool doneCave1;

	[SerializeThis]
	public bool doneCave2;

	[SerializeThis]
	public bool doneCave3;

	public bool doneCliffSpawn;

	private void Start()
	{
		if (BoltNetwork.isRunning)
		{
			return;
		}
		base.StartCoroutine("doYachtSpawn");
		base.StartCoroutine("doCliffSpawn");
		this.canSeeRedDay = Clock.Day;
	}

	private void OnDeserialized()
	{
		this.canSeeRedDay = Clock.Day;
	}

	[DebuggerHidden]
	private IEnumerator doYachtSpawn()
	{
		redmanSpawner.<doYachtSpawn>c__IteratorEE <doYachtSpawn>c__IteratorEE = new redmanSpawner.<doYachtSpawn>c__IteratorEE();
		<doYachtSpawn>c__IteratorEE.<>f__this = this;
		return <doYachtSpawn>c__IteratorEE;
	}

	[DebuggerHidden]
	private IEnumerator doCave1Spawn()
	{
		redmanSpawner.<doCave1Spawn>c__IteratorEF <doCave1Spawn>c__IteratorEF = new redmanSpawner.<doCave1Spawn>c__IteratorEF();
		<doCave1Spawn>c__IteratorEF.<>f__this = this;
		return <doCave1Spawn>c__IteratorEF;
	}

	[DebuggerHidden]
	private IEnumerator doCave2Spawn()
	{
		redmanSpawner.<doCave2Spawn>c__IteratorF0 <doCave2Spawn>c__IteratorF = new redmanSpawner.<doCave2Spawn>c__IteratorF0();
		<doCave2Spawn>c__IteratorF.<>f__this = this;
		return <doCave2Spawn>c__IteratorF;
	}

	[DebuggerHidden]
	private IEnumerator doCliffSpawn()
	{
		redmanSpawner.<doCliffSpawn>c__IteratorF1 <doCliffSpawn>c__IteratorF = new redmanSpawner.<doCliffSpawn>c__IteratorF1();
		<doCliffSpawn>c__IteratorF.<>f__this = this;
		return <doCliffSpawn>c__IteratorF;
	}

	[DebuggerHidden]
	private IEnumerator doRedmanOnCliff(Transform pos)
	{
		redmanSpawner.<doRedmanOnCliff>c__IteratorF2 <doRedmanOnCliff>c__IteratorF = new redmanSpawner.<doRedmanOnCliff>c__IteratorF2();
		<doRedmanOnCliff>c__IteratorF.pos = pos;
		<doRedmanOnCliff>c__IteratorF.<$>pos = pos;
		<doRedmanOnCliff>c__IteratorF.<>f__this = this;
		return <doRedmanOnCliff>c__IteratorF;
	}

	[DebuggerHidden]
	private IEnumerator removeRedman(GameObject go, float t)
	{
		redmanSpawner.<removeRedman>c__IteratorF3 <removeRedman>c__IteratorF = new redmanSpawner.<removeRedman>c__IteratorF3();
		<removeRedman>c__IteratorF.t = t;
		<removeRedman>c__IteratorF.go = go;
		<removeRedman>c__IteratorF.<$>t = t;
		<removeRedman>c__IteratorF.<$>go = go;
		<removeRedman>c__IteratorF.<>f__this = this;
		return <removeRedman>c__IteratorF;
	}
}
