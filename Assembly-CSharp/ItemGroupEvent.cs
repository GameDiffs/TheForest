using System;
using TheForest.Items;

[Serializable]
public class ItemGroupEvent
{
	[ItemIdPicker(Item.Types.Equipment)]
	public int[] _itemIds;

	public string eventPath;
}
