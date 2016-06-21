using System;

namespace TheForest.Items.Craft
{
	[SerializeAll]
	[Serializable]
	public class ReceipeIngredient
	{
		[ItemIdPicker]
		public int _itemID;

		public int _amount;

		public override bool Equals(object obj)
		{
			if (obj is ReceipeIngredient)
			{
				ReceipeIngredient receipeIngredient = (ReceipeIngredient)obj;
				return this._itemID == receipeIngredient._itemID && this._amount == receipeIngredient._amount;
			}
			if (obj is int)
			{
				int num = (int)obj;
				return this._itemID == num;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
