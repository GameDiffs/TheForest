using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class spinRagdoll : MonoBehaviour
{
	private void Start()
	{
		if (base.transform.GetComponent<Rigidbody>())
		{
			base.Invoke("spin", 0.05f);
		}
	}

	private void spin()
	{
		base.transform.GetComponent<Rigidbody>().AddTorque(20000f, 20000f, 20000f);
	}

	[DebuggerHidden]
	private IEnumerator spinForce()
	{
		spinRagdoll.<spinForce>c__IteratorFA <spinForce>c__IteratorFA = new spinRagdoll.<spinForce>c__IteratorFA();
		<spinForce>c__IteratorFA.<>f__this = this;
		return <spinForce>c__IteratorFA;
	}
}
