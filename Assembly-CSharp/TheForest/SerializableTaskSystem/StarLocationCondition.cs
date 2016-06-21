using System;
using TheForest.Tools;

namespace TheForest.SerializableTaskSystem
{
	[Serializable]
	public class StarLocationCondition : ACondition
	{
		public int _starNumber;

		public override void Init()
		{
			EventRegistry.Player.Subscribe(TfEvent.FoundStarLocation, new EventRegistry.SubscriberCallback(this.OnStarLocationVisited));
		}

		public override void Clear()
		{
			EventRegistry.Player.Unsubscribe(TfEvent.FoundStarLocation, new EventRegistry.SubscriberCallback(this.OnStarLocationVisited));
			base.Clear();
		}

		public virtual void OnStarLocationVisited(object o)
		{
			if (!this._done)
			{
				int num = (int)o;
				if (num == this._starNumber)
				{
					this.SetDone();
					this.Clear();
				}
			}
		}
	}
}
