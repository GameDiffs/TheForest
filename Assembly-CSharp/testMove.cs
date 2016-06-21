using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class testMove : MonoBehaviour
{
	private Rigidbody rb;

	private BoxCollider col;

	private float val;

	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.col = base.GetComponent<BoxCollider>();
		base.StartCoroutine("doMove");
		this.val = UnityEngine.Random.Range(-50f, 50f);
	}

	[DebuggerHidden]
	private IEnumerator doMove()
	{
		testMove.<doMove>c__IteratorFD <doMove>c__IteratorFD = new testMove.<doMove>c__IteratorFD();
		<doMove>c__IteratorFD.<>f__this = this;
		return <doMove>c__IteratorFD;
	}
}
