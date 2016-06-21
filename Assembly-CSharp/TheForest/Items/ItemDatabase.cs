using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

namespace TheForest.Items
{
	public class ItemDatabase : ScriptableObject
	{
		public int _lastId;

		public Item[] _items;

		private static ItemDatabase _instance;

		private Dictionary<int, Item> _itemsCache;

		public static Item[] Items
		{
			get
			{
				return ItemDatabase._instance._items;
			}
		}

		public void OnEnable()
		{
			base.hideFlags = HideFlags.None;
			if (ItemDatabase._instance == null)
			{
				ItemDatabase._instance = this;
				this._itemsCache = this._items.ToDictionary((Item item) => item._id);
			}
		}

		public static bool IsItemidValid(int id)
		{
			return ItemDatabase._instance._itemsCache.ContainsKey(id);
		}

		public static Item ItemById(int id)
		{
			return ItemDatabase._instance._itemsCache[id];
		}

		public static Item ItemByName(string name)
		{
			string b = name.ToLower();
			for (int i = 0; i < ItemDatabase._instance._items.Length; i++)
			{
				if (ItemDatabase._instance._items[i]._name.ToLower() == b)
				{
					return ItemDatabase._instance._items[i];
				}
			}
			return null;
		}

		public static IEnumerable<Item> ItemsByType(Item.Types typeMask)
		{
			return from i in ItemDatabase._instance._items
			where (i._type & typeMask) != (Item.Types)0
			select i;
		}
	}
}
