using System;
using TheForest.Items;

namespace TheForest.SerializableTaskSystem
{
	[Serializable]
	public class ItemIdList : ACondition
	{
		[ItemIdPicker]
		public int[] _itemIds;

		public override void Init()
		{
		}
	}
}
