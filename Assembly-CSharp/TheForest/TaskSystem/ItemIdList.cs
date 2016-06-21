using System;
using TheForest.Items;

namespace TheForest.TaskSystem
{
	[DoNotSerializePublic]
	[Serializable]
	public class ItemIdList
	{
		[ItemIdPicker]
		public int[] _itemIds;

		[SerializeThis]
		public bool _done;
	}
}
