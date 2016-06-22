using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathologicalGames
{
	public class SpawnPoolsDict : IDictionary<string, SpawnPool>, ICollection<KeyValuePair<string, SpawnPool>>, IEnumerable<KeyValuePair<string, SpawnPool>>, IEnumerable
	{
		public delegate void OnCreatedDelegate(SpawnPool pool);

		internal Dictionary<string, SpawnPoolsDict.OnCreatedDelegate> onCreatedDelegates = new Dictionary<string, SpawnPoolsDict.OnCreatedDelegate>();

		private Dictionary<string, SpawnPool> _pools = new Dictionary<string, SpawnPool>();

		bool ICollection<KeyValuePair<string, SpawnPool>>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public int Count
		{
			get
			{
				return this._pools.Count;
			}
		}

		public SpawnPool this[string key]
		{
			get
			{
				SpawnPool result;
				try
				{
					result = this._pools[key];
				}
				catch (KeyNotFoundException)
				{
					string message = string.Format("A Pool with the name '{0}' not found. \nPools={1}", key, this.ToString());
					throw new KeyNotFoundException(message);
				}
				return result;
			}
			set
			{
				string message = "Cannot set PoolManager.Pools[key] directly. SpawnPools add themselves to PoolManager.Pools when created, so there is no need to set them explicitly. Create pools using PoolManager.Pools.Create() or add a SpawnPool component to a GameObject.";
				throw new NotImplementedException(message);
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				string message = "If you need this, please request it.";
				throw new NotImplementedException(message);
			}
		}

		public ICollection<SpawnPool> Values
		{
			get
			{
				string message = "If you need this, please request it.";
				throw new NotImplementedException(message);
			}
		}

		private bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		void ICollection<KeyValuePair<string, SpawnPool>>.CopyTo(KeyValuePair<string, SpawnPool>[] array, int arrayIndex)
		{
			string message = "PoolManager.Pools cannot be copied";
			throw new NotImplementedException(message);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._pools.GetEnumerator();
		}

		public void AddOnCreatedDelegate(string poolName, SpawnPoolsDict.OnCreatedDelegate createdDelegate)
		{
			if (!this.onCreatedDelegates.ContainsKey(poolName))
			{
				this.onCreatedDelegates.Add(poolName, createdDelegate);
				return;
			}
			Dictionary<string, SpawnPoolsDict.OnCreatedDelegate> dictionary;
			Dictionary<string, SpawnPoolsDict.OnCreatedDelegate> expr_25 = dictionary = this.onCreatedDelegates;
			SpawnPoolsDict.OnCreatedDelegate a = dictionary[poolName];
			expr_25[poolName] = (SpawnPoolsDict.OnCreatedDelegate)Delegate.Combine(a, createdDelegate);
		}

		public void RemoveOnCreatedDelegate(string poolName, SpawnPoolsDict.OnCreatedDelegate createdDelegate)
		{
			if (!this.onCreatedDelegates.ContainsKey(poolName))
			{
				throw new KeyNotFoundException("No OnCreatedDelegates found for pool name '" + poolName + "'.");
			}
			Dictionary<string, SpawnPoolsDict.OnCreatedDelegate> dictionary;
			Dictionary<string, SpawnPoolsDict.OnCreatedDelegate> expr_2D = dictionary = this.onCreatedDelegates;
			SpawnPoolsDict.OnCreatedDelegate source = dictionary[poolName];
			expr_2D[poolName] = (SpawnPoolsDict.OnCreatedDelegate)Delegate.Remove(source, createdDelegate);
		}

		public SpawnPool Create(string poolName)
		{
			GameObject gameObject = new GameObject(poolName + "Pool");
			return gameObject.AddComponent<SpawnPool>();
		}

		public SpawnPool Create(string poolName, GameObject owner)
		{
			if (!this.assertValidPoolName(poolName))
			{
				return null;
			}
			string name = owner.gameObject.name;
			SpawnPool result;
			try
			{
				owner.gameObject.name = poolName;
				result = owner.AddComponent<SpawnPool>();
			}
			finally
			{
				owner.gameObject.name = name;
			}
			return result;
		}

		private bool assertValidPoolName(string poolName)
		{
			string text = poolName.Replace("Pool", string.Empty);
			if (text != poolName)
			{
				string message = string.Format("'{0}' has the word 'Pool' in it. This word is reserved for GameObject defaul naming. The pool name has been changed to '{1}'", poolName, text);
				Debug.LogWarning(message);
				poolName = text;
			}
			if (this.ContainsKey(poolName))
			{
				Debug.Log(string.Format("A pool with the name '{0}' already exists", poolName));
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			string[] array = new string[this._pools.Count];
			this._pools.Keys.CopyTo(array, 0);
			return string.Format("[{0}]", string.Join(", ", array));
		}

		public bool Destroy(string poolName)
		{
			SpawnPool spawnPool;
			if (!this._pools.TryGetValue(poolName, out spawnPool))
			{
				Debug.LogError(string.Format("PoolManager: Unable to destroy '{0}'. Not in PoolManager", poolName));
				return false;
			}
			UnityEngine.Object.Destroy(spawnPool.gameObject);
			return true;
		}

		public void DestroyAll()
		{
			foreach (KeyValuePair<string, SpawnPool> current in this._pools)
			{
				UnityEngine.Object.Destroy(current.Value);
			}
			this._pools.Clear();
		}

		internal void Add(SpawnPool spawnPool)
		{
			if (this.ContainsKey(spawnPool.poolName))
			{
				Debug.LogError(string.Format("A pool with the name '{0}' already exists. This should only happen if a SpawnPool with this name is added to a scene twice.", spawnPool.poolName));
				return;
			}
			this._pools.Add(spawnPool.poolName, spawnPool);
			if (this.onCreatedDelegates.ContainsKey(spawnPool.poolName))
			{
				this.onCreatedDelegates[spawnPool.poolName](spawnPool);
			}
		}

		public void Add(string key, SpawnPool value)
		{
			string message = "SpawnPools add themselves to PoolManager.Pools when created, so there is no need to Add() them explicitly. Create pools using PoolManager.Pools.Create() or add a SpawnPool component to a GameObject.";
			throw new NotImplementedException(message);
		}

		internal bool Remove(SpawnPool spawnPool)
		{
			if (!this.ContainsKey(spawnPool.poolName))
			{
				Debug.LogError(string.Format("PoolManager: Unable to remove '{0}'. Pool not in PoolManager", spawnPool.poolName));
				return false;
			}
			this._pools.Remove(spawnPool.poolName);
			return true;
		}

		public bool Remove(string poolName)
		{
			string message = "SpawnPools can only be destroyed, not removed and kept alive outside of PoolManager. There are only 2 legal ways to destroy a SpawnPool: Destroy the GameObject directly, if you have a reference, or use PoolManager.Destroy(string poolName).";
			throw new NotImplementedException(message);
		}

		public bool ContainsKey(string poolName)
		{
			return this._pools.ContainsKey(poolName);
		}

		public bool TryGetValue(string poolName, out SpawnPool spawnPool)
		{
			return this._pools.TryGetValue(poolName, out spawnPool);
		}

		public bool Contains(KeyValuePair<string, SpawnPool> item)
		{
			string message = "Use PoolManager.Pools.Contains(string poolName) instead.";
			throw new NotImplementedException(message);
		}

		public void Add(KeyValuePair<string, SpawnPool> item)
		{
			string message = "SpawnPools add themselves to PoolManager.Pools when created, so there is no need to Add() them explicitly. Create pools using PoolManager.Pools.Create() or add a SpawnPool component to a GameObject.";
			throw new NotImplementedException(message);
		}

		public void Clear()
		{
			string message = "Use PoolManager.Pools.DestroyAll() instead.";
			throw new NotImplementedException(message);
		}

		private void CopyTo(KeyValuePair<string, SpawnPool>[] array, int arrayIndex)
		{
			string message = "PoolManager.Pools cannot be copied";
			throw new NotImplementedException(message);
		}

		public bool Remove(KeyValuePair<string, SpawnPool> item)
		{
			string message = "SpawnPools can only be destroyed, not removed and kept alive outside of PoolManager. There are only 2 legal ways to destroy a SpawnPool: Destroy the GameObject directly, if you have a reference, or use PoolManager.Destroy(string poolName).";
			throw new NotImplementedException(message);
		}

		public IEnumerator<KeyValuePair<string, SpawnPool>> GetEnumerator()
		{
			return this._pools.GetEnumerator();
		}
	}
}
