using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheForest.SerializableTaskSystem
{
	[Serializable]
	public class ListAnyCondition : ACondition
	{
		[HideInInspector]
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
					if (this._conditions[i]._done)
					{
						this.SetDone();
						return;
					}
				}
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

		public override void SaveDone(ICollection<int> doneConditions)
		{
			if (this._done)
			{
				doneConditions.Add(this._id);
			}
			for (int i = 0; i < this._conditions.Length; i++)
			{
				this._conditions[i].SaveDone(doneConditions);
			}
		}

		public override void LoadDone(ICollection<int> doneConditions)
		{
			if (doneConditions.Contains(this._id))
			{
				this._done = true;
			}
			for (int i = 0; i < this._conditions.Length; i++)
			{
				this._conditions[i].LoadDone(doneConditions);
			}
		}
	}
}
