using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

namespace TheForest.Items.Craft
{
	[DoNotSerializePublic, AddComponentMenu("Items/Craft/Receipe Book")]
	public class ReceipeBook : MonoBehaviour
	{
		public ReceipeDatabase _receipeDatabase;

		public List<int> _availableReceipes;

		private IEnumerable<Receipe> _availableReceipesCache;

		private IEnumerable<Receipe> _availableUpgradesCache;

		public IEnumerable<Receipe> AvailableReceipesCache
		{
			get
			{
				return this._availableReceipesCache;
			}
		}

		public IEnumerable<Receipe> AvailableUpgradeCache
		{
			get
			{
				return this._availableUpgradesCache;
			}
		}

		private void Start()
		{
			this._availableReceipes = (from r in this._receipeDatabase._receipes
			select r._id).ToList<int>();
			this.RefreshCache();
		}

		private void OnDeserialized()
		{
		}

		public bool AddReceipe(Receipe receipe)
		{
			return this.AddReceipe(receipe._id);
		}

		public bool AddReceipe(int receipeId)
		{
			if (!this._availableReceipes.Contains(receipeId))
			{
				this._availableReceipes.Add(receipeId);
				this.RefreshCache();
				return true;
			}
			return false;
		}

		public bool RemoveReceipe(Receipe receipe)
		{
			return this.RemoveReceipe(receipe._id);
		}

		public bool RemoveReceipe(int receipeId)
		{
			if (this._availableReceipes.Contains(receipeId))
			{
				this._availableReceipes.Remove(receipeId);
				this.RefreshCache();
				return true;
			}
			return false;
		}

		private void RefreshCache()
		{
			this._availableReceipesCache = from r in this._receipeDatabase._receipes
			where !r._type.Equals(Receipe.Types.Upgrade) && this._availableReceipes.Contains(r._id)
			select r;
			this._availableUpgradesCache = from r in this._receipeDatabase._receipes
			where r._type.Equals(Receipe.Types.Upgrade) && this._availableReceipes.Contains(r._id)
			select r;
		}
	}
}
