using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathologicalGames
{
	[AddComponentMenu("Path-o-logical/TargetPro/Targetable"), RequireComponent(typeof(Rigidbody))]
	public class Targetable : MonoBehaviour
	{
		public delegate void OnDetectedDelegate(TargetTracker source);

		public delegate void OnNotDetectedDelegate(TargetTracker source);

		public delegate void OnHitDelegate(HitEffectList effects, Target target);

		public delegate void OnHitColliderDelegate(HitEffectList effects, Target target, Collider other);

		public bool isTargetable = true;

		public DEBUG_LEVELS debugLevel;

		public List<Perimeter> perimeters = new List<Perimeter>();

		public Transform xform;

		public GameObject go;

		private Targetable.OnDetectedDelegate onDetectedDelegates;

		private Targetable.OnNotDetectedDelegate onNotDetectedDelegates;

		private Targetable.OnHitDelegate onHitDelegates;

		private Targetable.OnHitColliderDelegate onHitColliderDelegates;

		[HideInInspector]
		public List<Vector3> waypoints = new List<Vector3>();

		public float strength
		{
			get;
			set;
		}

		public float distToDest
		{
			get
			{
				if (this.waypoints.Count == 0)
				{
					return 0f;
				}
				float num = this.GetDistToPos(this.waypoints[0]);
				for (int i = 0; i < this.waypoints.Count - 2; i++)
				{
					num += (this.waypoints[i] - this.waypoints[i + 1]).sqrMagnitude;
				}
				return num;
			}
		}

		private void Awake()
		{
			this.xform = base.transform;
			this.go = base.gameObject;
		}

		private void OnDisable()
		{
			this.CleanUp();
		}

		private void OnDetroy()
		{
			this.CleanUp();
		}

		private void CleanUp()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			List<Perimeter> list = new List<Perimeter>(this.perimeters);
			foreach (Perimeter current in list)
			{
				if (current.Count != 0 && !(current.targetTracker == null))
				{
					current.Remove(this);
				}
			}
		}

		public void OnHit(HitEffectList effects, Target target)
		{
			this.OnHit(effects, target, null);
		}

		public void OnHit(HitEffectList effects, Target target, Collider other)
		{
			effects = effects.CopyWithHitTime();
			if (this.onHitDelegates != null)
			{
				this.onHitDelegates(effects, target);
			}
			if (this.onHitColliderDelegates != null)
			{
				this.onHitColliderDelegates(effects, target, other);
			}
		}

		internal void OnDetected(TargetTracker source)
		{
			if (this.onDetectedDelegates != null)
			{
				this.onDetectedDelegates(source);
			}
		}

		internal void OnNotDetected(TargetTracker source)
		{
			if (this.onNotDetectedDelegates != null)
			{
				this.onNotDetectedDelegates(source);
			}
		}

		public float GetDistToPos(Vector3 other)
		{
			return (this.xform.position - other).sqrMagnitude;
		}

		public void AddOnDetectedDelegate(Targetable.OnDetectedDelegate del)
		{
			this.onDetectedDelegates = (Targetable.OnDetectedDelegate)Delegate.Combine(this.onDetectedDelegates, del);
		}

		public void SetOnDetectedDelegate(Targetable.OnDetectedDelegate del)
		{
			this.onDetectedDelegates = del;
		}

		public void RemoveOnDetectedDelegate(Targetable.OnDetectedDelegate del)
		{
			this.onDetectedDelegates = (Targetable.OnDetectedDelegate)Delegate.Remove(this.onDetectedDelegates, del);
		}

		public void AddOnNotDetectedDelegate(Targetable.OnNotDetectedDelegate del)
		{
			this.onNotDetectedDelegates = (Targetable.OnNotDetectedDelegate)Delegate.Combine(this.onNotDetectedDelegates, del);
		}

		public void SetOnNotDetectedDelegate(Targetable.OnNotDetectedDelegate del)
		{
			this.onNotDetectedDelegates = del;
		}

		public void RemoveOnNotDetectedDelegate(Targetable.OnNotDetectedDelegate del)
		{
			this.onNotDetectedDelegates = (Targetable.OnNotDetectedDelegate)Delegate.Remove(this.onNotDetectedDelegates, del);
		}

		public void AddOnHitDelegate(Targetable.OnHitDelegate del)
		{
			this.onHitDelegates = (Targetable.OnHitDelegate)Delegate.Combine(this.onHitDelegates, del);
		}

		public void SetOnHitDelegate(Targetable.OnHitDelegate del)
		{
			this.onHitDelegates = del;
		}

		public void RemoveOnHitDelegate(Targetable.OnHitDelegate del)
		{
			this.onHitDelegates = (Targetable.OnHitDelegate)Delegate.Remove(this.onHitDelegates, del);
		}

		public void AddOnHitColliderDelegate(Targetable.OnHitColliderDelegate del)
		{
			this.onHitColliderDelegates = (Targetable.OnHitColliderDelegate)Delegate.Combine(this.onHitColliderDelegates, del);
		}

		public void SetOnHitColliderDelegate(Targetable.OnHitColliderDelegate del)
		{
			this.onHitColliderDelegates = del;
		}

		public void RemoveOnHitColliderDelegate(Targetable.OnHitColliderDelegate del)
		{
			this.onHitColliderDelegates = (Targetable.OnHitColliderDelegate)Delegate.Remove(this.onHitColliderDelegates, del);
		}
	}
}
