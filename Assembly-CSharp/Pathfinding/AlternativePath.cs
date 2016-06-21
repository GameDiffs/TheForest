using System;
using UnityEngine;

namespace Pathfinding
{
	[AddComponentMenu("Pathfinding/Modifiers/Alternative Path")]
	[Serializable]
	public class AlternativePath : MonoModifier
	{
		public int penalty = 1000;

		public int randomStep = 10;

		private GraphNode[] prevNodes;

		private int prevSeed;

		private int prevPenalty;

		private bool waitingForApply;

		private readonly object lockObject = new object();

		private System.Random rnd = new System.Random();

		private readonly System.Random seedGenerator = new System.Random();

		private bool destroyed;

		private GraphNode[] toBeApplied;

		public override ModifierData input
		{
			get
			{
				return ModifierData.Original;
			}
		}

		public override ModifierData output
		{
			get
			{
				return ModifierData.All;
			}
		}

		public override void Apply(Path p, ModifierData source)
		{
			if (this == null)
			{
				return;
			}
			object obj = this.lockObject;
			lock (obj)
			{
				this.toBeApplied = p.path.ToArray();
				if (!this.waitingForApply)
				{
					this.waitingForApply = true;
					AstarPath.OnPathPreSearch = (OnPathDelegate)Delegate.Combine(AstarPath.OnPathPreSearch, new OnPathDelegate(this.ApplyNow));
				}
			}
		}

		public new void OnDestroy()
		{
			this.destroyed = true;
			object obj = this.lockObject;
			lock (obj)
			{
				if (!this.waitingForApply)
				{
					this.waitingForApply = true;
					AstarPath.OnPathPreSearch = (OnPathDelegate)Delegate.Combine(AstarPath.OnPathPreSearch, new OnPathDelegate(this.ClearOnDestroy));
				}
			}
			this.OnDestroy();
		}

		private void ClearOnDestroy(Path p)
		{
			object obj = this.lockObject;
			lock (obj)
			{
				AstarPath.OnPathPreSearch = (OnPathDelegate)Delegate.Remove(AstarPath.OnPathPreSearch, new OnPathDelegate(this.ClearOnDestroy));
				this.waitingForApply = false;
				this.InversePrevious();
			}
		}

		private void InversePrevious()
		{
			int seed = this.prevSeed;
			this.rnd = new System.Random(seed);
			if (this.prevNodes != null)
			{
				bool flag = false;
				int num = this.rnd.Next(this.randomStep);
				for (int i = num; i < this.prevNodes.Length; i += this.rnd.Next(1, this.randomStep))
				{
					if ((ulong)this.prevNodes[i].Penalty < (ulong)((long)this.prevPenalty))
					{
						flag = true;
					}
					this.prevNodes[i].Penalty = (uint)((ulong)this.prevNodes[i].Penalty - (ulong)((long)this.prevPenalty));
				}
				if (flag)
				{
					Debug.LogWarning("Penalty for some nodes has been reset while this modifier was active. Penalties might not be correctly set.");
				}
			}
		}

		private void ApplyNow(Path somePath)
		{
			object obj = this.lockObject;
			lock (obj)
			{
				this.waitingForApply = false;
				AstarPath.OnPathPreSearch = (OnPathDelegate)Delegate.Remove(AstarPath.OnPathPreSearch, new OnPathDelegate(this.ApplyNow));
				this.InversePrevious();
				if (!this.destroyed)
				{
					int seed = this.seedGenerator.Next();
					this.rnd = new System.Random(seed);
					if (this.toBeApplied != null)
					{
						int num = this.rnd.Next(this.randomStep);
						for (int i = num; i < this.toBeApplied.Length; i += this.rnd.Next(1, this.randomStep))
						{
							this.toBeApplied[i].Penalty = (uint)((ulong)this.toBeApplied[i].Penalty + (ulong)((long)this.penalty));
						}
					}
					this.prevPenalty = this.penalty;
					this.prevSeed = seed;
					this.prevNodes = this.toBeApplied;
				}
			}
		}
	}
}
