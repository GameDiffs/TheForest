using System;
using TheForest.Graphics;
using UnityEngine;

public class houseboatOnLand : MonoBehaviour
{
	public float massOnLand;

	public float massOnWater;

	private Buoyancy _bu;

	private Floating _fl;

	private Rigidbody _rb;

	private float _startDrag;

	private float _startAngularDrag;

	private void Awake()
	{
		this._bu = base.transform.GetComponent<Buoyancy>();
		this._fl = base.transform.GetComponent<Floating>();
		this._rb = base.transform.GetComponent<Rigidbody>();
		this._startDrag = this._rb.drag;
		this._startAngularDrag = this._rb.angularDrag;
	}

	private void LateUpdate()
	{
		if (this._bu.InWater)
		{
			if (!this._fl.enabled)
			{
				this._fl.enabled = true;
				this._rb.mass = this.massOnWater;
				this._rb.angularDrag = this._startAngularDrag;
				this._rb.drag = this._startDrag;
			}
			else
			{
				base.transform.up = Vector3.Lerp(base.transform.up, Vector3.up, Time.deltaTime);
			}
		}
		else if (this._fl.enabled)
		{
			this._fl.enabled = false;
			this._rb.mass = this.massOnLand;
			this._rb.angularDrag = 0.05f;
			this._rb.drag = 0.05f;
		}
		if (this._rb.velocity.sqrMagnitude > 9f)
		{
			this._rb.velocity = this._rb.velocity.normalized * 3f;
		}
	}
}
