using System;
using UnityEngine;

namespace TheForest.Items
{
	public class ItemIdPicker : PropertyAttribute
	{
		public Item.Types Type
		{
			get;
			set;
		}

		public bool Restricted
		{
			get;
			private set;
		}

		public ItemIdPicker()
		{
			this.Type = (Item.Types)(-1);
			this.Restricted = false;
		}

		public ItemIdPicker(Item.Types restrictedToType)
		{
			this.Type = restrictedToType;
			this.Restricted = true;
		}
	}
}
