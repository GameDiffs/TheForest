using System;
using System.Collections.Generic;
using UnityEngine;

[ComponentSerializerFor(typeof(Terrain))]
public class SerializeTerrain : ComponentSerializerExtensionBase<Terrain>
{
	public override IEnumerable<object> Save(Terrain target)
	{
		return new object[]
		{
			target.enabled
		};
	}

	public override void LoadInto(object[] data, Terrain instance)
	{
		instance.enabled = (bool)data[0];
	}
}
