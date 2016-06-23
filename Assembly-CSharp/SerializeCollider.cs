using System;
using System.Collections.Generic;
using UnityEngine;

[ComponentSerializerFor(typeof(TerrainCollider)), ComponentSerializerFor(typeof(MeshCollider)), ComponentSerializerFor(typeof(WheelCollider))]
public class SerializeCollider : ComponentSerializerExtensionBase<Collider>
{
	public override IEnumerable<object> Save(Collider target)
	{
		return new object[]
		{
			target.isTrigger,
			target.enabled
		};
	}

	public override void LoadInto(object[] data, Collider instance)
	{
		instance.isTrigger = (bool)data[0];
		instance.enabled = (bool)data[1];
	}
}
