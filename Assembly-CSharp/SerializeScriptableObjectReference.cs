using Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SubTypeSerializer(typeof(ScriptableObject))]
public class SerializeScriptableObjectReference : SerializerExtensionBase<object>
{
	public override IEnumerable<object> Save(object target)
	{
		SaveGameManager.AssetReference assetId = SaveGameManager.Instance.GetAssetId(target as UnityEngine.Object);
		if (assetId.index == -1)
		{
			byte[] array = UnitySerializer.SerializeForDeserializeInto(target);
			return new object[]
			{
				true,
				target.GetType().FullName,
				array
			};
		}
		return new object[]
		{
			false,
			assetId
		};
	}

	public override bool CanBeSerialized(Type targetType, object instance)
	{
		return instance != null;
	}

	public override object Load(object[] data, object instance)
	{
		if ((bool)data[0])
		{
			ScriptableObject scriptableObject = ScriptableObject.CreateInstance(UnitySerializer.GetTypeEx(data[1]));
			UnitySerializer.DeserializeInto((byte[])data[2], scriptableObject);
			return scriptableObject;
		}
		return SaveGameManager.Instance.GetAsset((SaveGameManager.AssetReference)data[1]);
	}
}
