using System;
using UnityEngine;

public class perchEnable : MonoBehaviour
{
	private SphereCollider perchCollider;

	public Collider currBird;

	private BurnDummy burn;

	private void Awake()
	{
		this.burn = base.transform.root.GetComponentInChildren<BurnDummy>();
	}

	private void OnEnable()
	{
		if (!this.perchCollider)
		{
			this.perchCollider = base.transform.GetComponent<SphereCollider>();
		}
		this.perchCollider.enabled = false;
		base.Invoke("checkAngle", 15f);
	}

	private void OnDisable()
	{
		if (this.currBird)
		{
			this.currBird.gameObject.SendMessage("Flee", SendMessageOptions.DontRequireReceiver);
		}
		this.currBird = null;
		if (this.perchCollider)
		{
			this.perchCollider.enabled = false;
		}
		base.CancelInvoke("checkAngle");
	}

	private void checkAngle()
	{
		if (this.burn)
		{
			if (this.burn._isBurning)
			{
				this.perchCollider.enabled = false;
				return;
			}
			if (this.burn._fire && this.burn._fire.activeSelf)
			{
				this.perchCollider.enabled = false;
				return;
			}
		}
		if (Vector3.Angle(base.transform.up, Vector3.up) > 90f)
		{
			this.perchCollider.enabled = false;
		}
		else
		{
			this.perchCollider.enabled = true;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("lb_bird"))
		{
			this.currBird = other;
		}
	}

	private void OnTriggerExit(Collider other)
	{
	}
}
