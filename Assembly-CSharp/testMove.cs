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
		testMove.<doMove>c__Iterator100 <doMove>c__Iterator = new testMove.<doMove>c__Iterator100();
		<doMove>c__Iterator.<>f__this = this;
		return <doMove>c__Iterator;
	}
}
