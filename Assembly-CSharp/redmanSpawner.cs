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
		redmanSpawner.<doYachtSpawn>c__IteratorEB <doYachtSpawn>c__IteratorEB = new redmanSpawner.<doYachtSpawn>c__IteratorEB();
		<doYachtSpawn>c__IteratorEB.<>f__this = this;
		return <doYachtSpawn>c__IteratorEB;
	}

	[DebuggerHidden]
	private IEnumerator doCave1Spawn()
	{
		redmanSpawner.<doCave1Spawn>c__IteratorEC <doCave1Spawn>c__IteratorEC = new redmanSpawner.<doCave1Spawn>c__IteratorEC();
		<doCave1Spawn>c__IteratorEC.<>f__this = this;
		return <doCave1Spawn>c__IteratorEC;
	}

	[DebuggerHidden]
	private IEnumerator doCave2Spawn()
	{
		redmanSpawner.<doCave2Spawn>c__IteratorED <doCave2Spawn>c__IteratorED = new redmanSpawner.<doCave2Spawn>c__IteratorED();
		<doCave2Spawn>c__IteratorED.<>f__this = this;
		return <doCave2Spawn>c__IteratorED;
	}

	[DebuggerHidden]
	private IEnumerator doCliffSpawn()
	{
		redmanSpawner.<doCliffSpawn>c__IteratorEE <doCliffSpawn>c__IteratorEE = new redmanSpawner.<doCliffSpawn>c__IteratorEE();
		<doCliffSpawn>c__IteratorEE.<>f__this = this;
		return <doCliffSpawn>c__IteratorEE;
	}

	[DebuggerHidden]
	private IEnumerator doRedmanOnCliff(Transform pos)
	{
		redmanSpawner.<doRedmanOnCliff>c__IteratorEF <doRedmanOnCliff>c__IteratorEF = new redmanSpawner.<doRedmanOnCliff>c__IteratorEF();
		<doRedmanOnCliff>c__IteratorEF.pos = pos;
		<doRedmanOnCliff>c__IteratorEF.<$>pos = pos;
		<doRedmanOnCliff>c__IteratorEF.<>f__this = this;
		return <doRedmanOnCliff>c__IteratorEF;
	}

	[DebuggerHidden]
	private IEnumerator removeRedman(GameObject go, float t)
	{
		redmanSpawner.<removeRedman>c__IteratorF0 <removeRedman>c__IteratorF = new redmanSpawner.<removeRedman>c__IteratorF0();
		<removeRedman>c__IteratorF.t = t;
		<removeRedman>c__IteratorF.go = go;
		<removeRedman>c__IteratorF.<$>t = t;
		<removeRedman>c__IteratorF.<$>go = go;
		<removeRedman>c__IteratorF.<>f__this = this;
		return <removeRedman>c__IteratorF;
	}
}
