using System;
using UnityEngine;

namespace TheForest.Items.Inventory
{
	[Serializable]
	public class InventoryItem
	{
		public int _itemId;

		public int _amount;

		public int _maxAmount;

		public int _maxAmountBonus;

		public int MaxAmount
		{
			get
			{
				return this._maxAmount + this._maxAmountBonus;
			}
		}

		public int Add(int amount, bool isEquiped)
		{
			this._amount += amount;
			int num = (!isEquiped) ? 0 : 1;
			if (this._amount + num > this.MaxAmount)
			{
				int num2 = this._amount + num - this.MaxAmount;
				this._amount -= num2;
				return num2;
			}
			return 0;
		}

		public int RemoveOverflow(int amount)
		{
			int num = Mathf.Max(amount - this._amount, 0);
			this._amount -= amount - num;
			return num;
		}

		public bool Remove(int amount)
		{
			if (this._amount - amount >= 0)
			{
				this._amount -= amount;
				return true;
			}
			return false;
		}
	}
}
