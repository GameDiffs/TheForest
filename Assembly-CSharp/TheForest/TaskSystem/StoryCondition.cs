using System;
using TheForest.Tools;
using TheForest.Utils;

namespace TheForest.TaskSystem
{
	[DoNotSerializePublic]
	[Serializable]
	public class StoryCondition : ACondition
	{
		public GameStats.StoryElements _type;

		public override void Init()
		{
			EventRegistry.Player.Subscribe(TfEvent.StoryProgress, new EventRegistry.SubscriberCallback(this.OnStoryProgress));
		}

		public override void Clear()
		{
			EventRegistry.Player.Unsubscribe(TfEvent.StoryProgress, new EventRegistry.SubscriberCallback(this.OnStoryProgress));
			base.Clear();
		}

		public virtual void OnStoryProgress(object o)
		{
			if (!this._done)
			{
				GameStats.StoryElements storyElements = (GameStats.StoryElements)((int)o);
				if (storyElements == this._type)
				{
					this.SetDone();
					this.Clear();
				}
			}
		}
	}
}
