using System;
using TheForest.Items;
using TheForest.Items.Craft;
using UnityEngine;

namespace TheForest.UI.Crafting
{
	public class ReceipeProductView : MonoBehaviour
	{
		public UITexture _icon;

		public UILabel _title;

		public void ShowReceipe(Receipe receipe, HudGui.InventoryItemInfo iii)
		{
			if (iii != null)
			{
				this._icon.mainTexture = iii._icon;
				this._icon.enabled = true;
			}
			else
			{
				this._icon.enabled = false;
			}
			if (receipe._type == Receipe.Types.Craft)
			{
				this._title.text = ((iii == null) ? ItemDatabase.ItemById(receipe._productItemID)._name : iii._titleText);
				if (receipe._productItemAmount > 1)
				{
					UILabel expr_82 = this._title;
					expr_82.text = expr_82.text + " x" + receipe._productItemAmount;
				}
			}
			else
			{
				string name = ItemDatabase.ItemById(receipe._ingredients[1]._itemID)._name;
				string text = name;
				switch (text)
				{
				case "Feather":
					this._title.text = "+Speed ";
					goto IL_29B;
				case "Tooth":
					this._title.text = "+Damage -Speed ";
					goto IL_29B;
				case "Booze":
					if (!this.CheckUpgradeBonus(receipe))
					{
						this._title.text = "+Damage ";
					}
					goto IL_29B;
				case "Treesap":
					this._title.text = "Sticky ";
					goto IL_29B;
				case "Cloth":
					this._title.text = "Burning ";
					goto IL_29B;
				case "OrangePaint":
					this._title.text = "Orange ";
					goto IL_29B;
				case "BluePaint":
					this._title.text = "Blue ";
					goto IL_29B;
				case "Battery":
					this._title.text = "Recharged ";
					goto IL_29B;
				case "Cassette 1":
				case "Cassette 2":
				case "Cassette 3":
				case "Cassette 4":
				case "Cassette 5":
					this._title.text = name + " ";
					goto IL_29B;
				}
				this._title.text = string.Empty;
				IL_29B:
				UILabel expr_2A1 = this._title;
				expr_2A1.text += ((iii == null) ? ItemDatabase.ItemById(receipe._productItemID)._name : iii._titleText);
			}
		}

		private bool CheckUpgradeBonus(Receipe r)
		{
			if (r._weaponStatUpgrades != null && r._weaponStatUpgrades.Length > 0)
			{
				WeaponStatUpgrade.Types type = r._weaponStatUpgrades[0]._type;
				if (type == WeaponStatUpgrade.Types.BurningWeaponExtra)
				{
					this._title.text = "Extra burning ";
					return true;
				}
				if (type == WeaponStatUpgrade.Types.Incendiary)
				{
					this._title.text = "Incendiary ";
					return true;
				}
			}
			return false;
		}
	}
}
