using System;
using UnityEngine;

public class pulseCollider : MonoBehaviour
{
	public Collider _collider;

	public float interval;

	private bool canPulse;

	private void Start()
	{
		if (!base.IsInvoking("initPulse"))
		{
			base.InvokeRepeating("initPulse", 0f, this.interval);
		}
	}

	private void OnEnable()
	{
		if (!base.IsInvoking("initPulse"))
		{
			base.InvokeRepeating("initPulse", 0f, this.interval);
		}
	}

	private void OnDisable()
	{
		base.CancelInvoke("initPulse");
	}

	private void Update()
	{
		if (this.canPulse)
		{
			this._collider.enabled = true;
		}
		else
		{
			this._collider.enabled = false;
		}
	}

	private void initPulse()
	{
		if (this.canPulse)
		{
			this.canPulse = false;
		}
		else
		{
			this.canPulse = true;
		}
	}
}
