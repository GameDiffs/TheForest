using System;
using System.Collections.Generic;

namespace TheForest.SerializableTaskSystem
{
	[Serializable]
	public abstract class ACondition
	{
		public int _id = -1;

		public bool _allowInMp = true;

		public bool _done;

		public Action OnStatusChange;

		public abstract void Init();

		public virtual void Prepare(Action onStatusChange)
		{
			if (this._allowInMp || !BoltNetwork.isRunning)
			{
				this.OnStatusChange = onStatusChange;
			}
		}

		public virtual void Clear()
		{
			this.OnStatusChange = null;
		}

		public virtual void SetDone()
		{
			if (!this._done)
			{
				this._done = true;
				this.OnStatusChange();
				this.OnStatusChange = null;
			}
		}

		public virtual void SaveDone(ICollection<int> doneConditions)
		{
			if (this._done)
			{
				doneConditions.Add(this._id);
			}
		}

		public virtual void LoadDone(ICollection<int> doneConditions)
		{
			if (doneConditions.Contains(this._id))
			{
				this._done = true;
			}
		}
	}
}
