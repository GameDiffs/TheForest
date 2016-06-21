using System;
using UnityEngine;

public class clsragdollhelper : MonoBehaviour
{
	private bool varcanragdoll;

	private clsragdollify varlocalragdollifier;

	private void Start()
	{
		base.GetComponent<Animation>().wrapMode = WrapMode.Loop;
		this.varlocalragdollifier = base.GetComponent<clsragdollify>();
		if (this.varlocalragdollifier != null && this.varlocalragdollifier.vargamragdoll != null)
		{
			this.varcanragdoll = true;
		}
	}

	public Transform metgoragdoll(Vector3 varpspeed = default(Vector3))
	{
		Transform result = null;
		if (this.varcanragdoll)
		{
			result = this.varlocalragdollifier.metgoragdoll(varpspeed);
			UnityEngine.Object.Destroy(base.gameObject);
		}
		return result;
	}
}
