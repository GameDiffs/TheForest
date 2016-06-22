using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathologicalGames
{
	public class HitEffectList : List<HitEffect>
	{
		public HitEffectList()
		{
		}

		public HitEffectList(HitEffectList hitEffectList) : base(hitEffectList)
		{
		}

		public override string ToString()
		{
			string[] effectStrings = new string[base.Count];
			int i = 0;
			base.ForEach(delegate(HitEffect effect)
			{
				effectStrings[i] = effect.ToString();
				i++;
			});
			return string.Join(", ", effectStrings);
		}

		public HitEffectList CopyWithHitTime()
		{
			HitEffectList newlist = new HitEffectList();
			base.ForEach(delegate(HitEffect effect)
			{
				effect.hitTime = Time.time;
				newlist.Add(effect);
			});
			return newlist;
		}
	}
}
