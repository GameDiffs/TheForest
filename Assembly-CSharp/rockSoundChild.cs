using System;
using UnityEngine;

public class rockSoundChild : MonoBehaviour
{
	private rockSound parent;

	private void Awake()
	{
		this.parent = base.GetComponentInParent<rockSound>();
		if (this.parent == null)
		{
			base.enabled = false;
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (this.parent != null)
		{
			this.parent.OnCollisionEnter(other);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.parent != null)
		{
			this.parent.OnTriggerEnter(other);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (this.parent != null)
		{
			this.parent.OnTriggerExit(other);
		}
	}
}
