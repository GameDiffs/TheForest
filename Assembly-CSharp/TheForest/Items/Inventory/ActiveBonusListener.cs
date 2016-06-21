using System;
using System.Collections.Generic;
using TheForest.Items.Craft;
using UnityEngine;

namespace TheForest.Items.Inventory
{
	[DoNotSerializePublic, AddComponentMenu("Items/Inventory/Active Bonus Listener")]
	public class ActiveBonusListener : MonoBehaviour
	{
		public InventoryItemView _itemView;

		public List<GameObject> _targetGOs;

		public WeaponStatUpgrade.Types _bonusToActivate;

		private void Awake()
		{
			this._itemView.ActiveBonusChanged += new Action<WeaponStatUpgrade.Types>(this.OnActiveBonusChanged);
		}

		private void OnDestroy()
		{
			this._itemView.ActiveBonusChanged -= new Action<WeaponStatUpgrade.Types>(this.OnActiveBonusChanged);
		}

		private void OnActiveBonusChanged(WeaponStatUpgrade.Types bonus)
		{
			for (int i = 0; i < this._targetGOs.Count; i++)
			{
				this._targetGOs[i].SetActive(bonus == this._bonusToActivate);
			}
		}
	}
}
