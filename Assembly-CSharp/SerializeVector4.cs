using Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializer(typeof(Vector4))]
public class SerializeVector4 : SerializerExtensionBase<Vector4>
{
	public override IEnumerable<object> Save(Vector4 target)
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
		if (!float.IsNaN((float)data[0]) && !float.IsNaN((float)data[1]) && !float.IsNaN((float)data[2]) && !float.IsNaN((float)data[3]))
		{
			return new Vector4((float)data[0], (float)data[1], (float)data[2], (float)data[3]);
		}
		return Vector4.zero;
	}
}
