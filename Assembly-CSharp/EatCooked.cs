using Bolt;
using System;
using TheForest.Items;
using TheForest.Items.Craft;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

public class EatCooked : EntityBehaviour
{
	public GameObject Sheen;

	public GameObject MyPickUp;

	public bool IsWater;

	public bool IsLimb;

	public float Size = 1f;

	public bool Burnt;

	[ItemIdPicker]
	public int _remainsItemId;

	public int _remainsYield;

	private void Awake()
	{
		base.enabled = false;
	}

	private void GrabEnter()
	{
		this.Sheen.SetActive(false);
		this.MyPickUp.SetActive(true);
		base.enabled = true;
	}

	private void GrabExit()
	{
		this.Sheen.SetActive(true);
		this.MyPickUp.SetActive(false);
		base.enabled = false;
	}

	private void Update()
	{
		if (TheForest.Utils.Input.GetButtonAfterDelay("Take", 0.5f))
		{
			LocalPlayer.Sfx.PlayWhoosh();
			if (!this.IsWater)
			{
				if (!this.Burnt)
				{
					Cook componentInParent = base.GetComponentInParent<Cook>();
					switch ((!componentInParent) ? DecayingInventoryItemView.DecayStates.Fresh : componentInParent._decayState)
					{
					case DecayingInventoryItemView.DecayStates.None:
					case DecayingInventoryItemView.DecayStates.Fresh:
						LocalPlayer.Stats.AteFreshMeat(this.IsLimb, this.Size);
						break;
					case DecayingInventoryItemView.DecayStates.Edible:
						LocalPlayer.Stats.AteEdibleMeat(this.IsLimb, this.Size);
						break;
					case DecayingInventoryItemView.DecayStates.Spoilt:
						LocalPlayer.Stats.AteSpoiltMeat(this.IsLimb, this.Size);
						break;
					}
				}
				else
				{
					LocalPlayer.Stats.AteBurnt(this.IsLimb, this.Size);
				}
				if (this._remainsYield > 0)
				{
					LocalPlayer.Inventory.AddItem(this._remainsItemId, this._remainsYield, false, false, (WeaponStatUpgrade.Types)(-2));
				}
			}
			else
			{
				LocalPlayer.Sfx.PlayDrink();
				LocalPlayer.Stats.Thirst = 0f;
			}
			if (BoltNetwork.isRunning && this.entity && this.entity.isAttached)
			{
				RequestDestroy requestDestroy = RequestDestroy.Create(GlobalTargets.Everyone);
				requestDestroy.Entity = this.entity;
				requestDestroy.Send();
			}
			else
			{
				UnityEngine.Object.Destroy(base.transform.parent.gameObject);
			}
		}
	}
}
