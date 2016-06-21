using Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializer(typeof(Material))]
public class SerializeMaterial : SerializerExtensionBase<Material>
{
	public override IEnumerable<object> Save(Material target)
	{
		StoreMaterials store = this.GetStore();
		if (target.GetInstanceID() >= 0)
		{
			return new object[]
			{
				true,
				SaveGameManager.Instance.GetAssetId(target)
			};
		}
		return new object[]
		{
			false,
			target.shader.name,
			target.name,
			target.renderQueue,
			(!(store != null)) ? null : store.GetValues(target)
		};
	}

	private StoreMaterials GetStore()
	{
		if (SerializeRenderer.Store != null)
		{
			return SerializeRenderer.Store;
		}
		if (UnitySerializer.currentlySerializingObject is Component)
		{
			Component component = UnitySerializer.currentlySerializingObject as Component;
			return component.GetComponent<StoreMaterials>();
		}
		return null;
	}

	public override object Load(object[] data, object instance)
	{
		if ((bool)data[0])
		{
			return SaveGameManager.Instance.GetAsset((SaveGameManager.AssetReference)data[1]);
		}
		Shader shader = Shader.Find((string)data[1]);
		Material material = new Material(shader);
		material.name = (string)data[2];
		StoreMaterials store = this.GetStore();
		material.renderQueue = (int)data[3];
		if (data[4] != null && store != null)
		{
			store.SetValues(material, (List<StoreMaterials.StoredValue>)data[4]);
		}
		return material;
	}

	public override bool CanBeSerialized(Type targetType, object instance)
	{
		Material material = instance as Material;
		return material && ((Shader.Find(material.shader.name) != null && material.GetInstanceID() < 0 && SerializeRenderer.Store != null) || SaveGameManager.Instance.GetAssetId(instance as UnityEngine.Object).index != -1);
	}
}
