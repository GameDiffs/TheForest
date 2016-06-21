using System;
using UnityEngine;

namespace Pathfinding
{
	[Serializable]
	public abstract class MonoModifier : MonoBehaviour, IPathModifier
	{
		[NonSerialized]
		public Seeker seeker;

		[SerializeField]
		private int priority;

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

		public abstract ModifierData input
		{
			get;
		}

		public abstract ModifierData output
		{
			get;
		}

		public void OnEnable()
		{
		}

		public void OnDisable()
		{
		}

		public void Awake()
		{
			this.seeker = base.GetComponent<Seeker>();
			if (this.seeker != null)
			{
				this.seeker.RegisterModifier(this);
			}
		}

		public void OnDestroy()
		{
			if (this.seeker != null)
			{
				this.seeker.DeregisterModifier(this);
			}
		}

		public void PreProcess(Path p)
		{
		}

		public abstract void Apply(Path p, ModifierData source);
	}
}
