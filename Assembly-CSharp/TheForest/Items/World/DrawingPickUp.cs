using System;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.World
{
	[AddComponentMenu("Items/World/Drawing PickUp")]
	public class DrawingPickUp : StoryPickUp
	{
		public int _id = -2;

		protected override void OwnershipCheck()
		{
			if (LocalPlayer.Inventory)
			{
				if ((LocalPlayer.Inventory.InventoryItemViewsCache[this._itemId][0] as DrawingsInventoryItemView).HasId(this._id))
				{
					base.ClearOut(false);
				}
				else
				{
					this._destroyTarget.SetActive(true);
				}
			}
		}

		protected override bool MainEffect()
		{
			if (base.MainEffect())
			{
				(LocalPlayer.Inventory.InventoryItemViewsCache[this._itemId][0] as DrawingsInventoryItemView).SetLast(this._id);
				return true;
			}
			return false;
		}
	}
}
