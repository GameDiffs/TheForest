using Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializer(typeof(Vector3))]
public class SerializeVector3 : SerializerExtensionBase<Vector3>
{
	public override IEnumerable<object> Save(Vector3 target)
	{
		return new object[]
		{
			target.x,
			target.y,
			target.z
		};
	}

	public override object Load(object[] data, object instance)
	{
		return new UnitySerializer.DeferredSetter(delegate(Dictionary<string, object> d)
		{
			if (!float.IsNaN((float)data[0]) && !float.IsNaN((float)data[1]) && !float.IsNaN((float)data[2]))
			{
				return new Vector3((float)data[0], (float)data[1], (float)data[2]);
			}
			return Vector3.zero;
		});
	}
}
