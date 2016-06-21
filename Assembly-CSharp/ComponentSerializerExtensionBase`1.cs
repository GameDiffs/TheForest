using Serialization;
using System;
using System.Collections.Generic;
using UniLinq;
using UnityEngine;

public abstract class ComponentSerializerExtensionBase<T> : IComponentSerializer where T : Component
{
	public abstract IEnumerable<object> Save(T target);

	public abstract void LoadInto(object[] data, T instance);

	public byte[] Serialize(Component component)
	{
		return UnitySerializer.Serialize(this.Save((T)((object)component)).ToArray<object>());
	}

	public void Deserialize(byte[] data, Component instance)
	{
		object[] data2 = UnitySerializer.Deserialize<object[]>(data);
		this.LoadInto(data2, (T)((object)instance));
	}
}
