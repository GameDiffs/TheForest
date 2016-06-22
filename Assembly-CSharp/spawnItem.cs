using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class spawnItem : MonoBehaviour
{
	public GameObject[] pickupSpawn;

	private Vector3 pos;

	private void invokePickupSpawn()
	{
		base.StartCoroutine("doPickupSpawn");
	}

	[DebuggerHidden]
	public IEnumerator doPickupSpawn()
	{
		spawnItem.<doPickupSpawn>c__IteratorFA <doPickupSpawn>c__IteratorFA = new spawnItem.<doPickupSpawn>c__IteratorFA();
		<doPickupSpawn>c__IteratorFA.<>f__this = this;
		return <doPickupSpawn>c__IteratorFA;
	}
}
