using System;

namespace TheForest.TaskSystem
{
	[DoNotSerializePublic]
	[Serializable]
	public class TrueCondition : ACondition
	{
		public override void Init()
		{
			this.SetDone();
		}
	}
}
