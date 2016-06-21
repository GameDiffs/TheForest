using Bolt;
using System;
using UnityEngine;

public class CoopAnimalFX : EntityBehaviour<IAnimalState>
{
	public GameObject FX_Fire;

	public GameObject FX_PickupTrigger;

	public override void Attached()
	{
		this.FX_Fire.SetActive(false);
		if (this.FX_PickupTrigger.activeSelf)
		{
			this.FX_PickupTrigger.SendMessage("OnSpawned", SendMessageOptions.DontRequireReceiver);
			this.FX_PickupTrigger.SetActive(false);
		}
		base.state.AddCallback("FX_Fire", new PropertyCallbackSimple(this.OnReceivedFire));
		base.state.AddCallback("FX_PickupTrigger", new PropertyCallbackSimple(this.OnReceivedPickupTrigger));
	}

	private void OnReceivedFire()
	{
		this.FX_Fire.SetActive(base.state.FX_Fire);
	}

	private void OnReceivedPickupTrigger()
	{
		this.FX_PickupTrigger.SetActive(base.state.FX_PickupTrigger);
	}

	public void Update()
	{
		if (BoltNetwork.isRunning && this.entity && this.entity.isAttached && this.entity.isOwner)
		{
			base.state.FX_Fire = this.FX_Fire.activeInHierarchy;
			if (this.FX_PickupTrigger)
			{
				base.state.FX_PickupTrigger = this.FX_PickupTrigger.activeInHierarchy;
			}
		}
	}
}
