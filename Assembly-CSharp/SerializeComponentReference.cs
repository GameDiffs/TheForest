using Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SubTypeSerializer(typeof(Component))]
public class SerializeComponentReference : SerializerExtensionBase<Component>
{
	public override IEnumerable<object> Save(Component target)
	{
		SaveGameManager.AssetReference assetId = SaveGameManager.Instance.GetAssetId(target);
		if (assetId.index != -1)
		{
			return new object[]
			{
				null,
				true,
				target.GetType().FullName,
				assetId
			};
		}
		int num = target.gameObject.GetComponents(target.GetType()).FindIndex((Component c) => c == target);
		if (UniqueIdentifier.GetByName(target.gameObject.GetId()) != null)
		{
			return new object[]
			{
				target.gameObject.GetId(),
				true,
				target.GetType().FullName,
				string.Empty,
				num
			};
		}
		return new object[]
		{
			target.gameObject.GetId(),
			false,
			target.GetType().FullName,
			string.Empty,
			num
		};
	}

	public override object Load(object[] data, object instance)
	{
		if (data[3] != null && data[3].GetType() == typeof(SaveGameManager.AssetReference))
		{
			return SaveGameManager.Instance.GetAsset((SaveGameManager.AssetReference)data[3]);
		}
		if (data.Length == 5)
		{
			return new UnitySerializer.DeferredSetter(delegate(Dictionary<string, object> d)
			{
				GameObject byName = UniqueIdentifier.GetByName((string)data[0]);
				if (byName == null)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"Could not find reference to ",
						data[0],
						" a ",
						(string)data[2]
					}));
					return null;
				}
				Component[] components = byName.GetComponents(UnitySerializer.GetTypeEx(data[2]));
				if (components.Length == 0)
				{
					return null;
				}
				if (components.Length <= (int)data[4])
				{
					data[4] = 0;
				}
				return (!(byName != null)) ? null : components[(int)data[4]];
			})
			{
				enabled = (bool)data[1]
			};
		}
		return new UnitySerializer.DeferredSetter(delegate(Dictionary<string, object> d)
		{
			GameObject byName = UniqueIdentifier.GetByName((string)data[0]);
			return (!(byName != null)) ? null : byName.GetComponent(UnitySerializer.GetTypeEx(data[2]));
		})
		{
			enabled = (bool)data[1]
		};
	}
}
