using Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializer(typeof(Color))]
public class SerializeColor : SerializerExtensionBase<Color>
{
	public override IEnumerable<object> Save(Color target)
	{
		return new object[]
		{
			target.r,
			target.g,
			target.b,
			target.a
		};
	}

	public override object Load(object[] data, object instance)
	{
		return new Color((float)data[0], (float)data[1], (float)data[2], (float)data[3]);
	}
}
