using System;

namespace PathologicalGames
{
	[Serializable]
	public class HitEffectGUIBacker
	{
		public string name = "Effect";

		public float value;

		public float duration;

		public HitEffectGUIBacker()
		{
		}

		public HitEffectGUIBacker(HitEffect effect)
		{
			this.name = effect.name;
			this.value = effect.value;
			this.duration = effect.duration;
		}

		public HitEffect GetHitEffect()
		{
			return new HitEffect
			{
				name = this.name,
				value = this.value,
				duration = this.duration
			};
		}
	}
}
