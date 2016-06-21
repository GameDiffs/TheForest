using System;
using TheForest.Items.Craft;
using TheForest.Utils;

namespace TheForest.Items.Special
{
	public class FlashLightControler : SpecialItemControlerBase
	{
		[ItemIdPicker(Item.Types.Edible)]
		public int _batteryItemId;

		public float _batteryUpsurgeDelay = 10f;

		public float _batteryUpsurgeValue = 1.5f;

		private float _nextBatteryUpsurge;

		protected override bool IsActive
		{
			get
			{
				return LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.LeftHand, LocalPlayer.Inventory.LastLight._itemId);
			}
		}

		public override bool ToggleSpecial(bool enable)
		{
			if (!enable)
			{
				return true;
			}
			if (LocalPlayer.Stats.BatteryCharge > 0f)
			{
				if (LocalPlayer.Inventory.LastLight != this)
				{
					LocalPlayer.Inventory.StashLeftHand();
					LocalPlayer.Inventory.LastLight = this;
				}
				LocalPlayer.Tuts.HideLighter();
				return true;
			}
			return false;
		}

		public override void PickedUpSpecialItem(int itemId)
		{
			if (itemId == this._itemId)
			{
				base.enabled = true;
				if (LocalPlayer.Inventory.AmountOf(this._itemId, false) > 1)
				{
					LocalPlayer.Inventory.RemoveItem(this._itemId, 1, false);
					LocalPlayer.Inventory.AddItem(this._batteryItemId, 1, false, false, (WeaponStatUpgrade.Types)(-2));
				}
			}
		}

		protected override void OnActivating()
		{
			if (LocalPlayer.Inventory.LastLight == this && !LocalPlayer.Animator.GetBool("drawBowBool"))
			{
				LocalPlayer.Inventory.TurnOnLastLight();
			}
		}

		protected override void OnDeactivating()
		{
			if (LocalPlayer.Inventory.LastLight == this)
			{
				LocalPlayer.Inventory.StashLeftHand();
			}
		}
	}
}
