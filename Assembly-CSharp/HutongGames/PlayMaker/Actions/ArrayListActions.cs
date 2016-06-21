using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	public abstract class ArrayListActions : CollectionsActions
	{
		internal PlayMakerArrayListProxy proxy;

		protected bool SetUpArrayListProxyPointer(GameObject aProxyGO, string nameReference)
		{
			if (aProxyGO == null)
			{
				return false;
			}
			this.proxy = base.GetArrayListProxyPointer(aProxyGO, nameReference, false);
			return this.proxy != null;
		}

		protected bool SetUpArrayListProxyPointer(PlayMakerArrayListProxy aProxy, string nameReference)
		{
			if (aProxy == null)
			{
				return false;
			}
			this.proxy = base.GetArrayListProxyPointer(aProxy.gameObject, nameReference, false);
			return this.proxy != null;
		}

		public bool isProxyValid()
		{
			if (this.proxy == null)
			{
				this.LogError("ArrayList proxy is null");
				return false;
			}
			if (this.proxy.arrayList == null)
			{
				this.LogError("ArrayList undefined");
				return false;
			}
			return true;
		}
	}
}
