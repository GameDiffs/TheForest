using System;
using TheForest.Items.Craft;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.Inventory
{
	[DoNotSerializePublic, AddComponentMenu("Items/Inventory/Active Bonus Listener (Material switch)")]
	public class ActiveBonusListenerMat : MonoBehaviour
	{
		[Serializable]
		public class ArrowBonusMat
		{
			public Material _material;

			public WeaponStatUpgrade.Types _bonusToActivate;
		}

		public InventoryItemView[] _itemViews;

		public Material _defaultMaterial;

		[NameFromProperty("_bonusToActivate")]
		public ActiveBonusListenerMat.ArrowBonusMat[] _bonuses;

		private void Awake()
		{
			for (int i = 0; i < this._itemViews.Length; i++)
			{
				this._itemViews[i].ActiveBonusChanged += new Action<WeaponStatUpgrade.Types>(this.OnActiveBonusChanged);
			}
		}

		private void LateUpdate()
		{
			for (int i = 0; i < this._itemViews.Length; i++)
			{
				this.ToggleItemViewMat(this._itemViews[i]);
			}
			base.enabled = false;
		}

		private void OnDestroy()
		{
			for (int i = 0; i < this._itemViews.Length; i++)
			{
				this._itemViews[i].ActiveBonusChanged -= new Action<WeaponStatUpgrade.Types>(this.OnActiveBonusChanged);
			}
		}

		public void ToggleItemViewMat(InventoryItemView iiv)
		{
			if (iiv.NormalMaterial)
			{
				Material material = this._defaultMaterial;
				for (int i = 0; i < this._bonuses.Length; i++)
				{
					if (iiv.ActiveBonus == this._bonuses[i]._bonusToActivate)
					{
						material = this._bonuses[i]._material;
						break;
					}
				}
				if (!iiv.NormalMaterial.Equals(material))
				{
					iiv.NormalMaterial = material;
					Renderer component = iiv.GetComponent<Renderer>();
					if (component && !component.sharedMaterial.Equals(material))
					{
						component.sharedMaterial = material;
					}
				}
				return;
			}
		}

		private void OnActiveBonusChanged(WeaponStatUpgrade.Types bonus)
		{
			base.enabled = true;
		}
	}
}
