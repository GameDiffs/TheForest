using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Items.Craft;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

namespace TheForest.Items.Core
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/World/Item Stash")]
	public class ItemStorage : MonoBehaviour, IItemStorage
	{
		[EnumFlags]
		public Item.Types _acceptedTypes;

		public int _slotCount;

		[ItemIdPicker]
		public int[] _blacklist = new int[0];

		[SerializeThis]
		protected List<InventoryItem> _usedSlots = new List<InventoryItem>();

		[SerializeThis]
		private int _usedSlotsCount;

		public Item.Types AcceptedTypes
		{
			get
			{
				return this._acceptedTypes;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this._usedSlots.Count == 0;
			}
		}

		public int ContentVersion
		{
			get;
			set;
		}

		public List<InventoryItem> UsedSlots
		{
			get
			{
				return this._usedSlots;
			}
		}

		private void Start()
		{
			base.enabled = false;
		}

		private void OnSerializing()
		{
			this._usedSlots.Capacity = this._usedSlots.Count;
			this._usedSlotsCount = this._usedSlots.Count;
		}

		private void OnDeserialized()
		{
			this._usedSlots.RemoveRange(this._usedSlotsCount, this._usedSlots.Count - this._usedSlotsCount);
		}

		private void UpdateFillIcon()
		{
			Scene.HudGui.StorageFill.fillAmount = (float)this._usedSlots.Count / (float)this._slotCount;
		}

		public void UpdateContentVersion()
		{
			this.ContentVersion = Mathf.FloorToInt(Mathf.Repeat((float)(this.ContentVersion + 1), 13371337f));
		}

		public void Add(int itemId, int amount, int maxAmountBonus)
		{
			if (this.Add(itemId, amount, (WeaponStatUpgrade.Types)(-2)) < amount)
			{
				this._usedSlots.LastOrDefault((InventoryItem s) => s._itemId == itemId)._maxAmountBonus = maxAmountBonus;
			}
		}

		public int Add(int itemId, int amount = 1, WeaponStatUpgrade.Types activeBonus = (WeaponStatUpgrade.Types)(-2))
		{
			if (!this._blacklist.Contains(itemId))
			{
				float num = 0f;
				int maxAmount = (num != 0f) ? Mathf.FloorToInt(1f / num) : 2147483647;
				InventoryItem inventoryItem = this._usedSlots.LastOrDefault((InventoryItem s) => s._itemId == itemId);
				if (inventoryItem != null)
				{
					this.UpdateContentVersion();
					amount = inventoryItem.Add(amount, false);
				}
				while (amount > 0 && (this._usedSlots.Count < this._slotCount || this._slotCount == 0))
				{
					inventoryItem = new InventoryItem
					{
						_itemId = itemId,
						_maxAmount = maxAmount
					};
					amount = inventoryItem.Add(amount, false);
					this._usedSlots.Add(inventoryItem);
					this.UpdateContentVersion();
				}
				this.UpdateFillIcon();
			}
			return amount;
		}

		public int Remove(int itemId, int amount = 1, WeaponStatUpgrade.Types activeBonus = (WeaponStatUpgrade.Types)(-2))
		{
			InventoryItem inventoryItem;
			while (amount > 0 && (inventoryItem = this._usedSlots.LastOrDefault((InventoryItem s) => s._itemId == itemId)) != null)
			{
				amount = inventoryItem.RemoveOverflow(amount);
				if (inventoryItem._amount == 0)
				{
					int index = this._usedSlots.IndexOf(inventoryItem);
					this._usedSlots.RemoveAt(index);
					this.UpdateContentVersion();
				}
			}
			this.UpdateFillIcon();
			return amount;
		}

		public void Open()
		{
			base.StartCoroutine(this.DelayedOpen());
		}

		[DebuggerHidden]
		private IEnumerator DelayedOpen()
		{
			ItemStorage.<DelayedOpen>c__Iterator14B <DelayedOpen>c__Iterator14B = new ItemStorage.<DelayedOpen>c__Iterator14B();
			<DelayedOpen>c__Iterator14B.<>f__this = this;
			return <DelayedOpen>c__Iterator14B;
		}

		public virtual void Close()
		{
		}
	}
}
