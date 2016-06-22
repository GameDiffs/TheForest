using System;
using UnityEngine;

namespace PathologicalGames
{
	public struct HitEffect
	{
		public string name;

		public float value;

		public float duration;

		public float hitTime;

		public float deltaDurationTime
		{
			get
			{
				return Mathf.Max(this.hitTime + this.duration - Time.time, 0f);
			}
		}

		public HitEffect(HitEffect hitEffect)
		{
			this.name = hitEffect.name;
			this.value = hitEffect.value;
			this.duration = hitEffect.duration;
			this.hitTime = hitEffect.hitTime;
		}

		public override string ToString()
		{
			return string.Format("(name '{0}', value {1}, duration {2}, hitTime {3}, deltaDurationTime {4})", new object[]
			{
				this.name,
				this.value,
				this.duration,
				this.hitTime,
				this.deltaDurationTime
			});
		}
	}
}
