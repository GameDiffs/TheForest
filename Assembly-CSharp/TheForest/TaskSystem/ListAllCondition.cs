using System;

namespace TheForest.TaskSystem
{
	[DoNotSerializePublic]
	[Serializable]
	public class ListAllCondition : ACondition
	{
		[SerializeThis]
		public ACondition[] _conditions;

		public override void Init()
		{
			this.CheckCompletion();
			if (!this._done)
			{
				for (int i = 0; i < this._conditions.Length; i++)
				{
					this._conditions[i].Prepare(new Action(this.CheckCompletion));
					this._conditions[i].Init();
				}
			}
		}

		public void CheckCompletion()
		{
			if (!this._done)
			{
				for (int i = 0; i < this._conditions.Length; i++)
				{
					if (!this._conditions[i]._done)
					{
						return;
					}
				}
				this.SetDone();
			}
		}

		public override void Clear()
		{
			for (int i = 0; i < this._conditions.Length; i++)
			{
				this._conditions[i].Clear();
			}
			base.Clear();
		}
	}
}
