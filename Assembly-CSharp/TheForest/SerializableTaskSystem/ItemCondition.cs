using System;
using System.Collections.Generic;
using TheForest.Tools;
using UnityEngine;

namespace TheForest.SerializableTaskSystem
{
	[Serializable]
	public class ItemCondition : ACondition
	{
		public ItemIdList[] _items;

		public override void Init()
		{
			if (this._items == null)
			{
				Debug.LogError("Broken ItemCondition, likely serializer didn't load it correctly");
			}
			else
			{
				EventRegistry.Player.Subscribe(TfEvent.AddedItem, new EventRegistry.SubscriberCallback(this.OnItemAdded));
			}
		}

		public override void Clear()
		{
			EventRegistry.Player.Unsubscribe(TfEvent.AddedItem, new EventRegistry.SubscriberCallback(this.OnItemAdded));
			base.Clear();
		}

		public void OnItemAdded(object o)
		{
			this.OnItemAdded((int)o);
		}

		public void OnItemAdded(int itemId)
		{
			bool flag = true;
			if (this._items != null)
			{
				for (int i = 0; i < this._items.Length; i++)
				{
					ItemIdList itemIdList = this._items[i];
					if (!itemIdList._done)
					{
						for (int j = 0; j < itemIdList._itemIds.Length; j++)
						{
							if (itemIdList._itemIds[j] == itemId)
							{
								itemIdList._done = true;
								break;
							}
						}
						if (!itemIdList._done)
						{
							flag = false;
						}
					}
				}
				if (flag)
				{
					this.SetDone();
					this.Clear();
				}
			}
			else
			{
				Debug.LogError("Broken ItemCondition, likely serializer didn't load it correctly");
			}
		}

		public override void SaveDone(ICollection<int> doneConditions)
		{
			if (this._done)
			{
				doneConditions.Add(this._id);
			}
			for (int i = 0; i < this._items.Length; i++)
			{
				this._items[i].SaveDone(doneConditions);
			}
		}

		public override void LoadDone(ICollection<int> doneConditions)
		{
			if (doneConditions.Contains(this._id))
			{
				this._done = true;
			}
			for (int i = 0; i < this._items.Length; i++)
			{
				this._items[i].LoadDone(doneConditions);
			}
		}
	}
}
