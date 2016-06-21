using Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializer(typeof(GameObject))]
public class SerializeGameObjectReference : SerializerExtensionBase<GameObject>
{
	static SerializeGameObjectReference()
	{
		UnitySerializer.CanSerialize += ((Type tp) => !typeof(Bounds).IsAssignableFrom(tp) && !typeof(MeshFilter).IsAssignableFrom(tp));
	}

	public override IEnumerable<object> Save(GameObject target)
	{
		SaveGameManager.AssetReference assetId = SaveGameManager.Instance.GetAssetId(target);
		if (assetId.index != -1)
		{
			return new object[]
			{
				0,
				true,
				null,
				assetId
			};
		}
		return new object[]
		{
			target.GetId(),
			UniqueIdentifier.GetByName(target.gameObject.GetId()) != null
		};
	}

	public override object Load(object[] data, object instance)
	{
		if (instance != null)
		{
			return instance;
		}
		if (data.Length > 3)
		{
			return SaveGameManager.Instance.GetAsset((SaveGameManager.AssetReference)data[3]) as GameObject;
		}
		return instance ?? new UnitySerializer.DeferredSetter((Dictionary<string, object> d) => UniqueIdentifier.GetByName((string)data[0]))
		{
			enabled = (bool)data[1]
		};
	}
}
