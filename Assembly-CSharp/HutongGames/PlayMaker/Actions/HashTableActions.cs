using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public abstract class HashTableActions : CollectionsActions
	{
		internal PlayMakerHashTableProxy proxy;

		protected bool SetUpHashTableProxyPointer(GameObject aProxyGO, string nameReference)
		{
			if (aProxyGO == null)
			{
				return false;
			}
			this.proxy = base.GetHashTableProxyPointer(aProxyGO, nameReference, false);
			return this.proxy != null;
		}

		protected bool SetUpHashTableProxyPointer(PlayMakerHashTableProxy aProxy, string nameReference)
		{
			if (aProxy == null)
			{
				return false;
			}
			this.proxy = base.GetHashTableProxyPointer(aProxy.gameObject, nameReference, false);
			return this.proxy != null;
		}

		protected bool isProxyValid()
		{
			if (this.proxy == null)
			{
				Debug.LogError("HashTable proxy is null");
				return false;
			}
			if (this.proxy.hashTable == null)
			{
				Debug.LogError("HashTable undefined");
				return false;
			}
			return true;
		}
	}
}
