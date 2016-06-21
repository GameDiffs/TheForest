using System;
using System.Collections.Generic;
using TheForest.Items.Utils;
using UniLinq;

namespace TheForest.Items.Craft
{
	[Serializable]
	public class Receipe
	{
		public enum Types
		{
			Craft,
			Upgrade
		}

		public int _id;

		public Receipe.Types _type;

		public string _name;

		public bool _batchUpgrade;

		public Item.Types _productItemType = (Item.Types)(-1);

		public int _productItemID;

		public RandomRange _productItemAmount;

		public ReceipeIngredient[] _ingredients;

		public WeaponStatUpgrade[] _weaponStatUpgrades;

		private string hash;

		public string IngredientHash
		{
			get
			{
				if (string.IsNullOrEmpty(this.hash))
				{
					this.hash = Receipe.IngredientsToRecipeHash(this._ingredients);
				}
				return this.hash;
			}
		}

		public static string IngredientsToRecipeHash(IEnumerable<ReceipeIngredient> ingredients)
		{
			return string.Join("_", (from i in ingredients
			orderby i._itemID
			select i._itemID.ToString()).ToArray<string>());
		}
	}
}
