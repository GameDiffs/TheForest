using System;

namespace TheForest.SerializableTaskSystem
{
	[Serializable]
	public class FalseCondition : ACondition
	{
		public override void Init()
		{
			this._done = false;
		}
	}
}
