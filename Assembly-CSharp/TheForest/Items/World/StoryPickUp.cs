using System;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.World
{
	[AddComponentMenu("Items/World/Story PickUp")]
	public class StoryPickUp : PickUp
	{
		protected const string PICKUP_EVENT = "event:/music/toy_pickup";

		protected override bool MainEffect()
		{
			bool flag = LocalPlayer.Stats.CheckItem(Item.EquipmentSlot.RightHand);
			if (flag)
			{
				LocalPlayer.Inventory.MemorizeItem(Item.EquipmentSlot.RightHand);
			}
			InventoryItemView inventoryItemView = LocalPlayer.Inventory.EquipmentSlotsPrevious[0];
			LocalPlayer.Inventory.Equip(this._itemId, true);
			LocalPlayer.Inventory.EquipmentSlotsPrevious[0] = inventoryItemView;
			FMOD_StudioSystem.instance.PlayOneShot("event:/music/toy_pickup", LocalPlayer.Transform.position, null);
			return true;
		}
	}
}
