using Serialization;
using System;
using System.Collections.Generic;
using UniLinq;

public class SerializerExtensionBase<T> : ISerializeObjectEx, ISerializeObject
{
	public object[] Serialize(object target)
	{
		return this.Save((T)((object)target)).ToArray<object>();
	}

	public object Deserialize(object[] data, object instance)
	{
		return this.Load(data, instance);
	}

	public virtual IEnumerable<object> Save(T target)
	{
		return new object[0];
	}

	public virtual object Load(object[] data, object instance)
	{
		return null;
	}

	public bool CanSerialize(Type targetType, object instance)
	{
		return instance == null || this.CanBeSerialized(targetType, instance);
	}

	public virtual bool CanBeSerialized(Type targetType, object instance)
	{
		return true;
	}
}
