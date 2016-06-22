using System;

namespace PathologicalGames
{
	public struct MessageData_TargetableOnHit
	{
		public HitEffectList effects;

		public Target target;

		public MessageData_TargetableOnHit(HitEffectList effects, Target target)
		{
			this.effects = effects;
			this.target = target;
		}
	}
}
