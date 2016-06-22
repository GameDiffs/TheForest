using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PathologicalGames
{
	[AddComponentMenu("Path-o-logical/TargetPro/CollisionTargetTracker")]
	public class CollisionTargetTracker : TargetTracker
	{
		private TargetList allTargets = new TargetList();

		public Collider coll;

		public override TargetList targets
		{
			get
			{
				this._targets.Clear();
				if (this.numberOfTargets == 0)
				{
					return this._targets;
				}
				List<Target> list = new List<Target>(this.allTargets);
				foreach (Target current in this.allTargets)
				{
					if (!current.gameObject.activeInHierarchy)
					{
						list.Remove(current);
					}
				}
				if (list.Count == 0)
				{
					return this._targets;
				}
				if (this.numberOfTargets == -1)
				{
					this._targets.AddRange(list);
				}
				else
				{
					int num = Mathf.Clamp(this.numberOfTargets, 0, list.Count);
					for (int i = 0; i < num; i++)
					{
						this._targets.Add(list[i]);
					}
				}
				if (this.onPostSortDelegates != null)
				{
					this.onPostSortDelegates(this._targets);
				}
				return this._targets;
			}
		}

		protected override void Awake()
		{
			this.xform = base.transform;
			this.coll = base.GetComponent<Collider>();
			if (this.coll == null)
			{
				throw new Exception("No collider or compound (child) collider found;");
			}
			if (this.coll.isTrigger)
			{
				throw new Exception("CollisionTargetTrackers do not work with trigger colliders.It is designed to work with Physics OnCollider events only.");
			}
		}

		private void OnCollisionEnter(Collision collisionInfo)
		{
			if (!this.IsInLayerMask(collisionInfo.gameObject))
			{
				return;
			}
			Target item = new Target(collisionInfo.transform, this);
			if (item.targetable == null)
			{
				return;
			}
			if (!item.targetable.isTargetable)
			{
				return;
			}
			if (!this.allTargets.Contains(item))
			{
				this.allTargets.Add(item);
			}
		}

		private void OnCollisionExit(Collision collisionInfo)
		{
			Target target = default(Target);
			foreach (Target current in this.allTargets)
			{
				if (current.gameObject == collisionInfo.gameObject)
				{
					target = current;
				}
			}
			if (target == Target.Null)
			{
				return;
			}
			base.StartCoroutine(this.DelayRemove(target));
		}

		[DebuggerHidden]
		private IEnumerator DelayRemove(Target target)
		{
			CollisionTargetTracker.<DelayRemove>c__IteratorC <DelayRemove>c__IteratorC = new CollisionTargetTracker.<DelayRemove>c__IteratorC();
			<DelayRemove>c__IteratorC.target = target;
			<DelayRemove>c__IteratorC.<$>target = target;
			<DelayRemove>c__IteratorC.<>f__this = this;
			return <DelayRemove>c__IteratorC;
		}

		protected override void OnEnable()
		{
		}

		protected override void OnDisable()
		{
		}

		private bool IsInLayerMask(GameObject obj)
		{
			LayerMask layerMask = 1 << obj.layer;
			LayerMask targetLayers = this.targetLayers;
			return (targetLayers.value & layerMask.value) != 0;
		}
	}
}
