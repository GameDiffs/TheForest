using System;

namespace TheForest.TaskSystem
{
	[DoNotSerializePublic]
	[Serializable]
	public class FalseCondition : ACondition
	{
		public override void Init()
		{
			this._done = false;
		}
	}
}
