using System;
using UnityEngine;

public class logChecker : MonoBehaviour
{
	private Rigidbody rb;

	public bool grounded;

	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.grounded = false;
		this.rb.angularDrag = 0.05f;
		this.rb.drag = 0.05f;
	}

	private void OnCollisionEnter(Collision col)
	{
		if (col == null)
		{
			return;
		}
		if (this.rb == null)
		{
			return;
		}
		if (col.gameObject == null)
		{
			return;
		}
		if (col.gameObject.CompareTag("Player"))
		{
			this.grounded = false;
			this.rb.drag = 0.05f;
			this.rb.angularDrag = 0.05f;
		}
		else if (col.gameObject.CompareTag("TerrainMain"))
		{
			this.grounded = true;
			this.rb.drag = 0.05f;
			this.rb.angularDrag = 8f;
		}
	}

	private void OnCollisionExit(Collision col)
	{
		if (col == null)
		{
			return;
		}
		if (col.gameObject == null)
		{
			return;
		}
		if (this.rb == null)
		{
			return;
		}
		if (col.gameObject.CompareTag("Player"))
		{
			this.grounded = false;
			this.rb.drag = 0.05f;
			this.rb.angularDrag = 8f;
		}
		if (col.gameObject.CompareTag("TerrainMain"))
		{
			this.rb.drag = 0.05f;
			this.rb.angularDrag = 0.05f;
			this.grounded = false;
		}
	}
}
