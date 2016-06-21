using System;
using System.Collections.Generic;
using UnityEngine;

[ComponentSerializerFor(typeof(BoxCollider))]
public class SerializeBoxCollider : ComponentSerializerExtensionBase<BoxCollider>
{
	public override IEnumerable<object> Save(BoxCollider target)
	{
		return new object[]
		{
			target.isTrigger,
			target.size.x,
			target.size.y,
			target.size.z,
			target.center.x,
			target.center.y,
			target.center.z,
			target.enabled
		};
	}

	public override void LoadInto(object[] data, BoxCollider instance)
	{
		instance.isTrigger = (bool)data[0];
		instance.size = new Vector3((float)data[1], (float)data[2], (float)data[3]);
		instance.center = new Vector3((float)data[4], (float)data[5], (float)data[6]);
		instance.enabled = (bool)data[7];
	}
}
