using System;
using TheForest.Tools;

namespace TheForest.SerializableTaskSystem
{
	[Serializable]
	public class EnemyInSightCondition : ACondition
	{
		public override void Init()
		{
			EventRegistry.Enemy.Subscribe(TfEvent.EnemyInSight, new EventRegistry.SubscriberCallback(this.OnEnemyInSight));
		}

		public override void Clear()
		{
			EventRegistry.Enemy.Unsubscribe(TfEvent.EnemyInSight, new EventRegistry.SubscriberCallback(this.OnEnemyInSight));
			base.Clear();
		}

		public void OnEnemyInSight(object enemy)
		{
			if (!this._done)
			{
				this.SetDone();
			}
			this.Clear();
		}
	}
}
