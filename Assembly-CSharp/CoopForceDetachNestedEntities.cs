using Bolt;
using System;

public class CoopForceDetachNestedEntities : EntityBehaviour
{
	public override void Detached()
	{
		BoltEntity[] componentsInChildren = base.GetComponentsInChildren<BoltEntity>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			BoltEntity boltEntity = componentsInChildren[i];
			boltEntity.transform.parent = null;
		}
	}
}
