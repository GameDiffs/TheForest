using System;
using UnityEngine;

namespace PathologicalGames
{
	public struct Target : IComparable<Target>
	{
		public GameObject gameObject;

		public Transform transform;

		public Targetable targetable;

		public TargetTracker targetTracker;

		public FireController fireController;

		public Projectile projectile;

		private static Target _Null = default(Target);

		public static Target Null
		{
			get
			{
				return Target._Null;
			}
		}

		public bool isSpawned
		{
			get
			{
				return !(this.gameObject == null) && this.gameObject.activeInHierarchy;
			}
		}

		public Target(Transform transform, TargetTracker targetTracker)
		{
			this.gameObject = transform.gameObject;
			this.transform = transform;
			this.targetable = this.transform.GetComponent<Targetable>();
			this.targetTracker = targetTracker;
			if (targetTracker is Projectile)
			{
				this.projectile = (Projectile)targetTracker;
			}
			else
			{
				this.projectile = null;
			}
			this.fireController = null;
		}

		public Target(Target otherTarget)
		{
			this.gameObject = otherTarget.gameObject;
			this.transform = otherTarget.transform;
			this.targetable = otherTarget.targetable;
			this.targetTracker = otherTarget.targetTracker;
			this.fireController = otherTarget.fireController;
			this.projectile = otherTarget.projectile;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object other)
		{
			return other != null && this == (Target)other;
		}

		public int CompareTo(Target obj)
		{
			return (!(this.gameObject == obj.gameObject)) ? 0 : 1;
		}

		public static bool operator ==(Target tA, Target tB)
		{
			return tA.gameObject == tB.gameObject;
		}

		public static bool operator !=(Target tA, Target tB)
		{
			return tA.gameObject != tB.gameObject;
		}
	}
}
