using System;
using System.Collections.Generic;

namespace TheForest.SerializableTaskSystem
{
	[Serializable]
	public class ListAllCondition : ACondition
	{
		public ACondition[] _conditions;

		public virtual ACondition[] Conditions
		{
			get
			{
				return this._conditions;
			}
		}

		public override void Init()
		{
			this.CheckCompletion();
			if (!this._done)
			{
				for (int i = 0; i < this.Conditions.Length; i++)
				{
					this.Conditions[i].Prepare(new Action(this.CheckCompletion));
					this.Conditions[i].Init();
				}
			}
		}

		public void CheckCompletion()
		{
			if (!this._done)
			{
				for (int i = 0; i < this.Conditions.Length; i++)
				{
					if (!this.Conditions[i]._done)
					{
						return;
					}
				}
				this.SetDone();
			}
		}

		public override void Clear()
		{
			for (int i = 0; i < this.Conditions.Length; i++)
			{
				this.Conditions[i].Clear();
			}
			base.Clear();
		}

		public override void SaveDone(ICollection<int> doneConditions)
		{
			base.SaveDone(doneConditions);
			for (int i = 0; i < this.Conditions.Length; i++)
			{
				this.Conditions[i].SaveDone(doneConditions);
			}
		}

		public override void LoadDone(ICollection<int> doneConditions)
		{
			base.LoadDone(doneConditions);
			for (int i = 0; i < this.Conditions.Length; i++)
			{
				this.Conditions[i].LoadDone(doneConditions);
			}
		}
	}
}
