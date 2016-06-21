using Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializer(typeof(TextAsset)), Serializer(typeof(AnimationClip)), Serializer(typeof(Font)), Serializer(typeof(AudioClip)), SubTypeSerializer(typeof(Mesh)), SubTypeSerializer(typeof(Texture))]
public class SerializeAssetReference : SerializerExtensionBase<object>
{
	public static SerializeAssetReference instance = new SerializeAssetReference();

	public override IEnumerable<object> Save(object target)
	{
		return new object[]
		{
			SaveGameManager.Instance.GetAssetId(target as UnityEngine.Object)
		};
	}

	public override bool CanBeSerialized(Type targetType, object instance)
	{
		return instance == null || typeof(UnityEngine.Object).IsAssignableFrom(targetType);
	}

	public override object Load(object[] data, object instance)
	{
		return SaveGameManager.Instance.GetAsset((SaveGameManager.AssetReference)data[0]);
	}
}
