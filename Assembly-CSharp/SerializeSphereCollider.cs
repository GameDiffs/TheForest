using System;
using System.Collections.Generic;
using UnityEngine;

[ComponentSerializerFor(typeof(SphereCollider))]
public class SerializeSphereCollider : ComponentSerializerExtensionBase<SphereCollider>
{
	public override IEnumerable<object> Save(SphereCollider target)
	{
		return new object[]
		{
			target.isTrigger,
			target.radius,
			target.center.x,
			target.center.y,
			target.center.z,
			target.enabled
		};
	}

	public override void LoadInto(object[] data, SphereCollider instance)
	{
		instance.isTrigger = (bool)data[0];
		instance.radius = (float)data[1];
		instance.center = new Vector3((float)data[2], (float)data[3], (float)data[4]);
		instance.enabled = (bool)data[5];
	}
}
