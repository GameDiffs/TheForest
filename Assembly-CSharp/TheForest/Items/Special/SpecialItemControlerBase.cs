using System;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.Special
{
	public abstract class SpecialItemControlerBase : MonoBehaviour
	{
		public enum Buttons
		{
			None,
			SurvivalBook,
			Utility,
			Walkman,
			Lighter,
			WalkyTalky,
			Map
		}

		[ItemIdPicker(Item.Types.Special)]
		public int _itemId;

		public SpecialItemControlerBase.Buttons _button;

		protected bool _checkRemoval;

		protected string _buttonCached;

		protected virtual bool IsActive
		{
			get
			{
				return LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.LeftHand, this._itemId);
			}
		}

		protected virtual void Start()
		{
			if (this._button != SpecialItemControlerBase.Buttons.None)
			{
				this._buttonCached = this._button.ToString();
			}
			LocalPlayer.Inventory.SpecialItemsControlers[this._itemId] = this;
			base.enabled = (this._button != SpecialItemControlerBase.Buttons.None && LocalPlayer.Inventory.Owns(this._itemId));
		}

		protected virtual void Update()
		{
			if (LocalPlayer.Inventory.enabled && this.CurrentViewTest() && TheForest.Utils.Input.GetButtonDown(this._buttonCached))
			{
				if (!this.IsActive)
				{
					this.OnActivating();
				}
				else
				{
					this.OnDeactivating();
				}
			}
			if (this._checkRemoval)
			{
				this._checkRemoval = false;
				if (!LocalPlayer.Inventory.Owns(this._itemId))
				{
					base.enabled = false;
				}
			}
		}

		public abstract bool ToggleSpecial(bool enable);

		public virtual bool ToggleSpecialCraft(bool enable)
		{
			return true;
		}

		public virtual void PickedUpSpecialItem(int itemId)
		{
			if (itemId == this._itemId && this._button != SpecialItemControlerBase.Buttons.None)
			{
				base.enabled = true;
			}
		}

		public virtual void LostSpecialItem(int itemId)
		{
			if (itemId == this._itemId)
			{
				this._checkRemoval = true;
			}
		}

		protected virtual bool CurrentViewTest()
		{
			return LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.World;
		}

		protected abstract void OnActivating();

		protected abstract void OnDeactivating();
	}
}
