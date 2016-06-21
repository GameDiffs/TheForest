using System;
using UnityEngine;

namespace Pathfinding
{
	[Serializable]
	public abstract class PathModifier : IPathModifier
	{
		[NonSerialized]
		public Seeker seeker;

		[SerializeField]
		private int priority;

		public abstract ModifierData input
		{
			get;
		}

		public abstract ModifierData output
		{
			get;
		}

		public int Priority
		{
			get
			{
				return this.priority;
			}
			set
			{
				this.priority = value;
			}
		}

		public void Awake(Seeker s)
		{
			this.seeker = s;
			if (s != null)
			{
				s.RegisterModifier(this);
			}
		}

		public void OnDestroy(Seeker s)
		{
			if (s != null)
			{
				s.DeregisterModifier(this);
			}
		}

		public void PreProcess(Path p)
		{
		}

		public abstract void Apply(Path p, ModifierData source);
	}
}
