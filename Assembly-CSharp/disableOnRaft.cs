using System;
using UnityEngine;

public class disableOnRaft : MonoBehaviour
{
	public Collider coll;

	private void Start()
	{
		if (base.transform.root.GetComponent<raftOnLand>() && this.coll)
		{
			this.coll.enabled = false;
		}
	}
}
