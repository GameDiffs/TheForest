using System;
using UnityEngine;

public class clsdrop : MonoBehaviour
{
	public bool vargamautodrop = true;

	public Collider vargamcollider;

	private void Start()
	{
		if (this.vargamautodrop)
		{
			this.metdrop(false);
		}
	}

	public void metdrop(bool varpenablecollider = false)
	{
		if (base.GetComponent<Rigidbody>() != null)
		{
			if (base.GetComponent<Rigidbody>().isKinematic)
			{
				base.GetComponent<Rigidbody>().isKinematic = false;
			}
		}
		else
		{
			Debug.LogWarning("Trying to drop a weightless object (needs a rigidbody)", base.gameObject);
		}
		if (varpenablecollider)
		{
			if (this.vargamcollider != null)
			{
				this.vargamcollider.enabled = true;
			}
			else
			{
				Debug.LogWarning("Trying to enable a null collider", base.gameObject);
			}
		}
		base.transform.parent = null;
	}

	public void metdrop(Vector3 varpforce, bool varpenablecollider = true)
	{
		if (varpenablecollider)
		{
			if (this.vargamcollider != null)
			{
				this.vargamcollider.enabled = true;
			}
			else
			{
				Debug.LogWarning("Trying to enable a null collider", base.gameObject);
			}
		}
		if (base.GetComponent<Rigidbody>() != null)
		{
			if (base.GetComponent<Rigidbody>().isKinematic)
			{
				base.GetComponent<Rigidbody>().isKinematic = false;
				base.GetComponent<Rigidbody>().AddForce(varpforce, ForceMode.VelocityChange);
			}
		}
		else
		{
			Debug.LogWarning("Trying to drop a weightless object (needs a rigidbody)", base.gameObject);
		}
		base.transform.parent = null;
	}

	public void metdrop(Vector3 varpforce, ForceMode varpmode, bool varpenablecollider = true)
	{
		if (base.GetComponent<Rigidbody>() != null)
		{
			if (base.GetComponent<Rigidbody>().isKinematic)
			{
				base.GetComponent<Rigidbody>().isKinematic = false;
				base.GetComponent<Rigidbody>().AddForce(varpforce, varpmode);
			}
		}
		else
		{
			Debug.LogWarning("Trying to drop a weightless object (needs a rigidbody)", base.gameObject);
		}
		if (varpenablecollider)
		{
			if (this.vargamcollider != null)
			{
				this.vargamcollider.enabled = true;
			}
			else
			{
				Debug.LogWarning("Trying to enable a null collider", base.gameObject);
			}
		}
		base.transform.parent = null;
	}
}
