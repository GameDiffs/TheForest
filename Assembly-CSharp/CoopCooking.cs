using Bolt;
using System;

public class CoopCooking : EntityBehaviour<ICookingState>
{
	public override void Attached()
	{
		base.state.Transform.SetTransforms(base.transform);
	}
}
