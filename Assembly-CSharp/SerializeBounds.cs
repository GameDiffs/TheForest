using Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializer(typeof(Bounds))]
public class SerializeBounds : SerializerExtensionBase<Bounds>
{
	public override IEnumerable<object> Save(Bounds target)
	{
		return new object[]
		{
			target.center.x,
			target.center.y,
			target.center.z,
			target.size.x,
			target.size.y,
			target.size.z
		};
	}

	public override object Load(object[] data, object instance)
	{
		return new Bounds(new Vector3((float)data[0], (float)data[1], (float)data[2]), new Vector3((float)data[3], (float)data[4], (float)data[5]));
	}
}
