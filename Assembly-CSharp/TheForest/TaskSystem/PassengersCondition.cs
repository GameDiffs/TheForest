using System;
using TheForest.Utils;
using UnityEngine.Events;

namespace TheForest.TaskSystem
{
	[DoNotSerializePublic]
	[Serializable]
	public class PassengersCondition : ACondition
	{
		public override void Init()
		{
			GameStats.FoundPassenger.AddListener(new UnityAction(this.OnPassengerFound));
		}

		public override void Clear()
		{
			GameStats.FoundPassenger.RemoveListener(new UnityAction(this.OnPassengerFound));
			base.Clear();
		}

		public void OnPassengerFound()
		{
			if (!this._done && Scene.GameStats._stats._passengersFound == LocalPlayer.PassengerManifest._foundGOs.Length)
			{
				this.SetDone();
				this.Clear();
			}
		}
	}
}
