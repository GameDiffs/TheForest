using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.UI
{
	[AddComponentMenu("Items/UI/Item Amount Label")]
	public class ItemAmountLabel : MonoBehaviour
	{
		public int _itemId;

		public UILabel _label;

		private void LateUpdate()
		{
			if (this._itemId > 0 && this._label.enabled && ItemDatabase.IsItemidValid(this._itemId))
			{
				this._label.text = LocalPlayer.Inventory.AmountOf(this._itemId, false).ToString();
			}
		}
	}
}
