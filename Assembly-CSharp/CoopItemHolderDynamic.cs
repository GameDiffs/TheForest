using Bolt;
using System;

public class CoopItemHolderDynamic : EntityBehaviour<IItemHolderDynamicState>
{
	public override void Attached()
	{
		base.state.Transform.SetTransforms(base.transform);
	}
}
