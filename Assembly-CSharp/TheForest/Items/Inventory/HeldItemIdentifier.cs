using System;
using UnityEngine;

namespace TheForest.Items.Inventory
{
	[AddComponentMenu("Items/Inventory/Held Item Identifier")]
	public class HeldItemIdentifier : MonoBehaviour
	{
		[ItemIdPicker(Item.Types.Equipment)]
		public int _itemId;
	}
}
