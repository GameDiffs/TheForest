using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.World
{
	[AddComponentMenu("Items/World/Alternating PickUp")]
	public class AlternatingPickUp : EdiblePickUp
	{
		[ItemIdPicker]
		public int _heldItemId;

		public Item.EquipmentSlot _heldItemSlot;

		[ItemIdPicker]
		public int _altItemId;

		public GameObject _altMyPickUp;

		protected override void GrabEnter()
		{
			if (LocalPlayer.Inventory.HasInSlot(this._heldItemSlot, this._heldItemId))
			{
				if (this._sheen)
				{
					this._sheen.SetActive(false);
				}
				if (this._myPickUp)
				{
					this._myPickUp.SetActive(false);
				}
				if (this._altMyPickUp)
				{
					this._altMyPickUp.SetActive(true);
				}
				base.enabled = true;
			}
			else
			{
				base.GrabEnter();
			}
		}

		protected override void GrabExit()
		{
			if (this._altMyPickUp)
			{
				this._altMyPickUp.SetActive(false);
			}
			base.GrabExit();
		}

		protected override bool MainEffect()
		{
			if (LocalPlayer.Inventory.HasInSlot(this._heldItemSlot, this._heldItemId))
			{
				this._itemId = this._altItemId;
				base.ForcePickup = true;
			}
			else
			{
				base.ForcePickup = false;
			}
			return base.MainEffect();
		}
	}
}
