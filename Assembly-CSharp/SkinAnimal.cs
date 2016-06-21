using Bolt;
using System;
using TheForest.Items;
using TheForest.Items.Craft;
using TheForest.Utils;
using UnityEngine;

public class SkinAnimal : EntityEventListener
{
	public Material SkinnedMat;

	public Renderer MyBody;

	public GameObject FleshTrigger;

	public GameObject Sheen;

	public GameObject MyPickUp;

	public bool Croc;

	public bool Lizard;

	public bool Rabbit;

	[ItemIdPicker]
	public int _itemId;

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

	public override void OnEvent(SkinnedAnimal evnt)
	{
		if (evnt.FromSelf)
		{
			this.SinglePlayer_Skinning();
		}
		else
		{
			this.SetSkinned();
		}
	}

	private void Update()
	{
		if (TheForest.Utils.Input.GetButtonDown("Take"))
		{
			LocalPlayer.Sfx.PlayWhoosh();
			if (BoltNetwork.isRunning)
			{
				this.entity.Freeze(false);
				SkinnedAnimal.Create(this.entity).Send();
			}
			else
			{
				this.SinglePlayer_Skinning();
			}
		}
	}

	private void SinglePlayer_Skinning()
	{
		if (this.Croc)
		{
			LocalPlayer.Inventory.AddItem(this._itemId, 1, false, false, (WeaponStatUpgrade.Types)(-2));
			LocalPlayer.Inventory.AddItem(this._itemId, 1, false, false, (WeaponStatUpgrade.Types)(-2));
			LocalPlayer.Inventory.AddItem(this._itemId, 1, false, false, (WeaponStatUpgrade.Types)(-2));
			LocalPlayer.Inventory.AddItem(this._itemId, 1, false, false, (WeaponStatUpgrade.Types)(-2));
			this.SetSkinned();
		}
		if (this.Lizard)
		{
			LocalPlayer.Inventory.AddItem(this._itemId, 1, false, false, (WeaponStatUpgrade.Types)(-2));
			this.SetSkinned();
		}
		if (this.Rabbit)
		{
			LocalPlayer.Inventory.AddItem(this._itemId, 1, false, false, (WeaponStatUpgrade.Types)(-2));
			this.SetSkinned();
		}
	}

	public void SetSkinned()
	{
		this.MyBody.material = this.SkinnedMat;
		this.FleshTrigger.SetActive(true);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
