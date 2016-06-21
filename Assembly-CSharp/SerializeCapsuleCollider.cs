using System;
using System.Collections.Generic;
using UnityEngine;

[ComponentSerializerFor(typeof(CapsuleCollider))]
public class SerializeCapsuleCollider : ComponentSerializerExtensionBase<CapsuleCollider>
{
	public override IEnumerable<object> Save(CapsuleCollider target)
	{
		return new object[]
		{
			target.isTrigger,
			target.radius,
			target.center.x,
			target.center.y,
			target.center.z,
			target.height,
			target.enabled
		};
	}

	public override void LoadInto(object[] data, CapsuleCollider instance)
	{
		instance.isTrigger = (bool)data[0];
		instance.radius = (float)data[1];
		instance.center = new Vector3((float)data[2], (float)data[3], (float)data[4]);
		instance.height = (float)data[5];
		instance.enabled = (bool)data[6];
	}
}
