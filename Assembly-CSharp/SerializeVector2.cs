using Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializer(typeof(Vector2))]
public class SerializeVector2 : SerializerExtensionBase<Vector2>
{
	public override IEnumerable<object> Save(Vector2 target)
	{
		return new object[]
		{
			target.x,
			target.y
		};
	}

	public override object Load(object[] data, object instance)
	{
		return new Vector2((float)data[0], (float)data[1]);
	}
}
