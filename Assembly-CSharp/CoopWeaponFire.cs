using Bolt;
using System;

public class CoopWeaponFire : EntityBehaviour<IFireParticle>
{
	public override void Attached()
	{
		base.state.Transform.SetTransforms(base.transform);
	}
}
