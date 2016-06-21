using Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializer(typeof(WaitForSeconds))]
public class SerializeWaitForSeconds : SerializerExtensionBase<WaitForSeconds>
{
	public override IEnumerable<object> Save(WaitForSeconds target)
	{
		Type type = target.GetType();
		PropertyInfo property = type.GetProperty("m_seconds", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (property == null)
		{
			FieldInfo field = type.GetField("m_seconds", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return new object[]
			{
				field.GetValue(target)
			};
		}
		object obj = property.GetGetMethod().Invoke(target, null);
		return new object[]
		{
			obj
		};
	}

	public override object Load(object[] data, object instance)
	{
		return base.Load(data, instance);
	}
}
