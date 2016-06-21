using Bolt;
using System;
using UnityEngine;

public class FlareWeapon : EntityBehaviour
{
	public int Amount = 8;

	public GameObject MyFire;

	private void Awake()
	{
		if (BoltNetwork.isRunning && this.entity.isAttached && !this.entity.isOwner)
		{
			base.enabled = false;
		}
		else
		{
			base.Invoke("TurnOff", (float)this.Amount);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		this.MyFire.SetActive(false);
	}

	private void Update()
	{
		if ((!BoltNetwork.isRunning || (this.entity && this.entity.isAttached && this.entity.isOwner)) && !base.GetComponent<Rigidbody>().isKinematic && base.GetComponent<Rigidbody>().velocity.sqrMagnitude < 0.1f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void TurnOff()
	{
		if (!BoltNetwork.isRunning || (this.entity && this.entity.isAttached && this.entity.isOwner))
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
