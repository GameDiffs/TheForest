using System;
using TheForest.Utils;
using UnityEngine.Events;

namespace TheForest.SerializableTaskSystem
{
	[Serializable]
	public class CookFoodCondition : ACondition
	{
		public override void Init()
		{
			GameStats.CookedFood.AddListener(new UnityAction(this.OnFoodCooked));
		}

		public override void Clear()
		{
			GameStats.CookedFood.RemoveListener(new UnityAction(this.OnFoodCooked));
			base.Clear();
		}

		public void OnFoodCooked()
		{
			if (!this._done)
			{
				this.SetDone();
				this.Clear();
			}
		}
	}
}
