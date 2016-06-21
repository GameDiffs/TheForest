using System;
using UnityEngine;

namespace TheForest.TaskSystem
{
	[DoNotSerializePublic]
	[Serializable]
	public class ListAnyCondition : ACondition
	{
		[SerializeThis, HideInInspector]
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
	}
}
