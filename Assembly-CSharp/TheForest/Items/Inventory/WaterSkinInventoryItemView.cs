using System;
using TheForest.Items.Craft;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.Inventory
{
	[DoNotSerializePublic, AddComponentMenu("Items/Inventory/WaterSkin Inventory View")]
	public class WaterSkinInventoryItemView : InventoryItemView
	{
		protected override void UseEdible()
		{
			if (this._activeBonus == WeaponStatUpgrade.Types.DirtyWater)
			{
				LocalPlayer.Stats.HitFoodDelayed(10);
			}
			float num = Mathf.Min(base.ActiveBonusValue, LocalPlayer.Stats.Thirst);
			LocalPlayer.Stats.Thirst -= num * LocalPlayer.Stats.FoodPoisoning.EffectRatio;
			base.ActiveBonusValue -= num;
			if (base.ActiveBonusValue < 0.25f)
			{
				this.ActiveBonus = (WeaponStatUpgrade.Types)(-1);
			}
			if (base.ItemCache._usedSFX != Item.SFXCommands.None)
			{
				this._inventory.SendMessage("PlayInventorySound", base.ItemCache._usedSFX);
			}
		}
	}
}
