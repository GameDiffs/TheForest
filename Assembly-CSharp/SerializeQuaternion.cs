using Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializer(typeof(Quaternion))]
public class SerializeQuaternion : SerializerExtensionBase<Quaternion>
{
	public override IEnumerable<object> Save(Quaternion target)
	{
		return new object[]
		{
			target.x,
			target.y,
			target.z,
			target.w
		};
	}

	public override object Load(object[] data, object instance)
	{
		return new UnitySerializer.DeferredSetter((Dictionary<string, object> d) => new Quaternion((float)data[0], (float)data[1], (float)data[2], (float)data[3]));
	}
}
