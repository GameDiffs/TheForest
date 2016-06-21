using System;
using System.Collections.Generic;

namespace TheForest.SerializableTaskSystem
{
	[Serializable]
	public class Task : ACondition
	{
		public bool _available;

		public virtual ACondition _availableCondition
		{
			get;
			set;
		}

		public virtual ACondition _completeCondition
		{
			get;
			set;
		}

		public override void Init()
		{
			if (!this._done)
			{
				if (!this._available)
				{
					if (this._availableCondition != null)
					{
						this._availableCondition.Prepare(new Action(this.SetAvailable));
						this._availableCondition.Init();
					}
				}
				else if (this._completeCondition != null)
				{
					this._completeCondition.Prepare(new Action(this.SetDone));
					this._completeCondition.Init();
				}
			}
		}

		public virtual void SetAvailable()
		{
			if (!this._available)
			{
				this._available = true;
				this.OnStatusChange();
				this.Init();
			}
		}

		public override void SaveDone(ICollection<int> doneConditions)
		{
			base.SaveDone(doneConditions);
			if (this._availableCondition != null)
			{
				this._availableCondition.SaveDone(doneConditions);
			}
			if (this._completeCondition != null)
			{
				this._completeCondition.SaveDone(doneConditions);
			}
		}

		public override void LoadDone(ICollection<int> doneConditions)
		{
			base.LoadDone(doneConditions);
			if (this._availableCondition != null)
			{
				this._availableCondition.LoadDone(doneConditions);
				if (!this._available)
				{
					this._available = (this._availableCondition == null || this._availableCondition._done);
				}
			}
			if (this._completeCondition != null)
			{
				this._completeCondition.LoadDone(doneConditions);
				if (!this._done)
				{
					this._available = (this._completeCondition != null && this._completeCondition._done);
				}
			}
		}
	}
}
