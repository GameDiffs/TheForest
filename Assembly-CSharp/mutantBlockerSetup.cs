using System;
using TheForest.Utils;
using UnityEngine;

public class mutantBlockerSetup : MonoBehaviour
{
	private Collider _col;

	private void Start()
	{
		if (!CoopPeerStarter.DedicatedHost)
		{
			this._col = base.GetComponent<Collider>();
			this.setBlockerCollision();
		}
		else
		{
			base.enabled = false;
		}
	}

	private void OnEnable()
	{
		if (!base.IsInvoking("setBlockerCollision"))
		{
			base.InvokeRepeating("setBlockerCollision", 1f, 10f);
		}
	}

	private void OnDisable()
	{
		base.CancelInvoke("setBlockerCollision");
	}

	private void setBlockerCollision()
	{
		Collider component = LocalPlayer.Transform.GetComponent<CapsuleCollider>();
		Collider component2 = LocalPlayer.Transform.GetComponent<SphereCollider>();
		if (component && component.enabled && this._col.enabled)
		{
			Physics.IgnoreCollision(this._col, component);
		}
		if (component2 && component2.enabled && this._col.enabled)
		{
			Physics.IgnoreCollision(this._col, component2);
		}
	}
}
