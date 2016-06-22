using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items.Core;
using TheForest.Items.Craft;
using TheForest.Items.Inventory;
using TheForest.Utils;

namespace TheForest.Items.Special
{
	public class MetalTinTrayControler : SpecialItemControlerBase
	{
		public ItemStorage _storage;

		protected override bool IsActive
		{
			get
			{
				return LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.RightHand, this._itemId);
			}
		}

		public override bool ToggleSpecial(bool enable)
		{
			if (enable)
			{
				LocalPlayer.Inventory._craftingCog.Storage = null;
			}
			else if (!this._storage.Equals(LocalPlayer.Inventory._craftingCog.Storage))
			{
				this.EmptyToInventory();
			}
			return true;
		}

		public override bool ToggleSpecialCraft(bool enable)
		{
			if (enable)
			{
				if (LocalPlayer.Inventory._craftingCog.Storage == null || this._storage.Equals(LocalPlayer.Inventory._craftingCog.Storage))
				{
					LocalPlayer.Inventory._craftingCog.Storage = this._storage;
					this._storage.Open();
					return true;
				}
				return false;
			}
			else
			{
				if (LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.Inventory && this._storage.Equals(LocalPlayer.Inventory._craftingCog.Storage) && !LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.RightHand, this._itemId) && !LocalPlayer.Inventory.HasInNextSlot(Item.EquipmentSlot.RightHand, this._itemId))
				{
					this.EmptyToInventory();
					LocalPlayer.Inventory._craftingCog.Storage = null;
					return true;
				}
				if (this._storage.Equals(LocalPlayer.Inventory._craftingCog.Storage))
				{
					LocalPlayer.Inventory._craftingCog.Storage = null;
				}
				return false;
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayedToggleSpecialCraft()
		{
			MetalTinTrayControler.<DelayedToggleSpecialCraft>c__Iterator16F <DelayedToggleSpecialCraft>c__Iterator16F = new MetalTinTrayControler.<DelayedToggleSpecialCraft>c__Iterator16F();
			<DelayedToggleSpecialCraft>c__Iterator16F.<>f__this = this;
			return <DelayedToggleSpecialCraft>c__Iterator16F;
		}

		protected override void OnActivating()
		{
			if (!LocalPlayer.Animator.GetBool("drawBowBool"))
			{
				LocalPlayer.Inventory.Equip(this._itemId, false);
			}
		}

		protected override void OnDeactivating()
		{
			LocalPlayer.Inventory.StashEquipedWeapon(true);
		}

		private void EmptyToInventory()
		{
			for (int i = 0; i < this._storage.UsedSlots.Count; i++)
			{
				LocalPlayer.Inventory.AddItem(this._storage.UsedSlots[i]._itemId, this._storage.UsedSlots[i]._amount, true, true, (WeaponStatUpgrade.Types)(-2));
			}
			this._storage.Close();
			this._storage.UsedSlots.Clear();
			this._storage.UpdateContentVersion();
		}
	}
}
