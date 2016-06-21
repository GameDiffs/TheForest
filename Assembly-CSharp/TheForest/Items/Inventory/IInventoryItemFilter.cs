using System;
using TheForest.Items.Craft;

namespace TheForest.Items.Inventory
{
	public interface IInventoryItemFilter
	{
		bool Owns(int itemId);

		int AmountOf(int itemId, bool allowFallback);

		bool AddItem(int itemId, int amount, bool preventAutoEquip, bool fromCraftingCog, WeaponStatUpgrade.Types activeBonus);

		bool RemoveItem(int itemId, int amount, bool allowAmountOverflow);
	}
}
