using Bolt;
using System;

public class CoopTreeHouse : EntityBehaviour<ITreeHouseState>
{
	public override void Attached()
	{
		base.state.Transform.SetTransforms(base.transform);
	}
}
