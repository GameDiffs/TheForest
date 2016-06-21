using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[HutongGames.PlayMaker.Tooltip("Collections base action - don't use!")]
	public abstract class CollectionsActions : FsmStateAction
	{
		public enum FsmVariableEnum
		{
			FsmGameObject,
			FsmInt,
			FsmFloat,
			FsmString,
			FsmBool,
			FsmVector2,
			FsmVector3,
			FsmRect,
			FsmQuaternion,
			FsmColor,
			FsmMaterial,
			FsmTexture,
			FsmObject
		}

		protected PlayMakerHashTableProxy GetHashTableProxyPointer(GameObject aProxy, string nameReference, bool silent)
		{
			if (aProxy == null)
			{
				if (!silent)
				{
					Debug.LogError("Null Proxy");
				}
				return null;
			}
			PlayMakerHashTableProxy[] components = aProxy.GetComponents<PlayMakerHashTableProxy>();
			if (components.Length > 1)
			{
				if (nameReference == string.Empty && !silent)
				{
					Debug.LogWarning("Several HashTable Proxies coexists on the same GameObject and no reference is given to find the expected HashTable");
				}
				PlayMakerHashTableProxy[] array = components;
				for (int i = 0; i < array.Length; i++)
				{
					PlayMakerHashTableProxy playMakerHashTableProxy = array[i];
					if (playMakerHashTableProxy.referenceName == nameReference)
					{
						return playMakerHashTableProxy;
					}
				}
				if (nameReference != string.Empty)
				{
					if (!silent)
					{
						Debug.LogError("HashTable Proxy not found for reference <" + nameReference + ">");
					}
					return null;
				}
			}
			else if (components.Length > 0)
			{
				if (nameReference != string.Empty && nameReference != components[0].referenceName)
				{
					if (!silent)
					{
						Debug.LogError("HashTable Proxy reference do not match");
					}
					return null;
				}
				return components[0];
			}
			if (!silent)
			{
				Debug.LogError("HashTable not found");
			}
			return null;
		}

		protected PlayMakerArrayListProxy GetArrayListProxyPointer(GameObject aProxy, string nameReference, bool silent)
		{
			if (aProxy == null)
			{
				if (!silent)
				{
					Debug.LogError("Null Proxy");
				}
				return null;
			}
			PlayMakerArrayListProxy[] components = aProxy.GetComponents<PlayMakerArrayListProxy>();
			if (components.Length > 1)
			{
				if (nameReference == string.Empty && !silent)
				{
					Debug.LogError("Several ArrayList Proxies coexists on the same GameObject and no reference is given to find the expected ArrayList");
				}
				PlayMakerArrayListProxy[] array = components;
				for (int i = 0; i < array.Length; i++)
				{
					PlayMakerArrayListProxy playMakerArrayListProxy = array[i];
					if (playMakerArrayListProxy.referenceName == nameReference)
					{
						return playMakerArrayListProxy;
					}
				}
				if (nameReference != string.Empty)
				{
					if (!silent)
					{
						this.LogError("ArrayList Proxy not found for reference <" + nameReference + ">");
					}
					return null;
				}
			}
			else if (components.Length > 0)
			{
				if (nameReference != string.Empty && nameReference != components[0].referenceName)
				{
					if (!silent)
					{
						Debug.LogError("ArrayList Proxy reference do not match");
					}
					return null;
				}
				return components[0];
			}
			if (!silent)
			{
				this.LogError("ArrayList proxy not found");
			}
			return null;
		}
	}
}
