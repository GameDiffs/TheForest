using System;
using TheForest.Items.Craft;

namespace TheForest.Items.Core
{
	public interface IItemStorage
	{
		Item.Types AcceptedTypes
		{
			get;
		}

		bool IsEmpty
		{
			get;
		}

		int Add(int itemId, int amount = 1, WeaponStatUpgrade.Types activeBonus = (WeaponStatUpgrade.Types)(-2));

		int Remove(int itemId, int amount = 1, WeaponStatUpgrade.Types activeBonus = (WeaponStatUpgrade.Types)(-2));

		void Open();

		void Close();
	}
}
