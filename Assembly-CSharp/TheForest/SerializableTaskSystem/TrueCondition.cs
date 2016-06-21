using System;

namespace TheForest.SerializableTaskSystem
{
	[Serializable]
	public class TrueCondition : ACondition
	{
		public override void Init()
		{
			this.SetDone();
		}
	}
}
