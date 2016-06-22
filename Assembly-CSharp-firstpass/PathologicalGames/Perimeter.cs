using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PathologicalGames
{
	[RequireComponent(typeof(Rigidbody))]
	public class Perimeter : MonoBehaviour, IList<Target>, ICollection<Target>, IEnumerable<Target>, IEnumerable
	{
		internal TargetTracker targetTracker;

		internal bool dirty = true;

		private TargetList targets = new TargetList();

		private Transform xform;

		public Target this[int index]
		{
			get
			{
				return this.targets[index];
			}
			set
			{
				throw new NotImplementedException("Read-only.");
			}
		}

		public bool IsReadOnly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int Count
		{
			get
			{
				return this.targets.Count;
			}
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			Perimeter.GetEnumerator>c__IteratorF getEnumerator>c__IteratorF = new Perimeter.GetEnumerator>c__IteratorF();
			getEnumerator>c__IteratorF.<>f__this = this;
			return getEnumerator>c__IteratorF;
		}

		private void Awake()
		{
			this.xform = base.transform;
			base.GetComponent<Rigidbody>().isKinematic = true;
		}

		[DebuggerHidden]
		private IEnumerator UpdateSort()
		{
			Perimeter.<UpdateSort>c__Iterator10 <UpdateSort>c__Iterator = new Perimeter.<UpdateSort>c__Iterator10();
			<UpdateSort>c__Iterator.<>f__this = this;
			return <UpdateSort>c__Iterator;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!this.IsInLayerMask(other.gameObject))
			{
				return;
			}
			Target target = new Target(other.transform, this.targetTracker);
			if (target.targetable == null)
			{
				return;
			}
			if (!target.targetable.isTargetable)
			{
				return;
			}
			this.Add(target);
		}

		private void OnTriggerExit(Collider other)
		{
			this.Remove(other.transform);
		}

		public void Add(Target target)
		{
			bool flag = false;
			if (this.targets.Count == 0)
			{
				flag = true;
			}
			this.targets.Add(target);
			target.targetable.perimeters.Add(this);
			this.dirty = true;
			target.targetable.OnDetected(this.targetTracker);
			if (flag && this.targetTracker.sortingStyle != TargetTracker.SORTING_STYLES.None)
			{
				base.StartCoroutine(this.UpdateSort());
			}
		}

		public bool Remove(Transform xform)
		{
			return this.Remove(new Target(xform, this.targetTracker));
		}

		public bool Remove(Targetable targetable)
		{
			return this.Remove(new Target
			{
				gameObject = targetable.gameObject,
				transform = targetable.transform,
				targetable = targetable
			});
		}

		public bool Remove(Target target)
		{
			if (!this.targets.Remove(target))
			{
				return false;
			}
			target.targetable.perimeters.Remove(this);
			this.dirty = true;
			if (target.transform == null || this.xform == null || this.xform.parent == null)
			{
				return false;
			}
			target.targetable.OnNotDetected(this.targetTracker);
			return true;
		}

		public void Clear()
		{
			foreach (Target current in this.targets)
			{
				current.targetable.OnNotDetected(this.targetTracker);
			}
			this.targets.Clear();
			this.dirty = true;
		}

		public bool Contains(Transform transform)
		{
			return this.targets.Contains(new Target(transform, this.targetTracker));
		}

		public bool Contains(Target target)
		{
			return this.targets.Contains(target);
		}

		[DebuggerHidden]
		public IEnumerator<Target> GetEnumerator()
		{
			Perimeter.<GetEnumerator>c__Iterator11 <GetEnumerator>c__Iterator = new Perimeter.<GetEnumerator>c__Iterator11();
			<GetEnumerator>c__Iterator.<>f__this = this;
			return <GetEnumerator>c__Iterator;
		}

		public void CopyTo(Target[] array, int arrayIndex)
		{
			this.targets.CopyTo(array, arrayIndex);
		}

		public int IndexOf(Target item)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, Target item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			string[] array = new string[this.targets.Count];
			int num = 0;
			foreach (Target current in this.targets)
			{
				if (current.transform == null)
				{
					array[num] = "null";
					num++;
				}
				else
				{
					string text = string.Format("{0}:Layer={1}", current.transform.name, LayerMask.LayerToName(current.gameObject.layer));
					switch (this.targetTracker.sortingStyle)
					{
					case TargetTracker.SORTING_STYLES.Nearest:
					case TargetTracker.SORTING_STYLES.Farthest:
					{
						float distToPos = current.targetable.GetDistToPos(this.xform.position);
						text += string.Format(",Dist={0}", distToPos);
						break;
					}
					case TargetTracker.SORTING_STYLES.NearestToDestination:
					case TargetTracker.SORTING_STYLES.FarthestFromDestination:
						text += string.Format(",DistToDest={0}", current.targetable.distToDest);
						break;
					}
					array[num] = text;
					num++;
				}
			}
			return string.Format("[{0}]", string.Join(", ", array));
		}

		private bool IsInLayerMask(GameObject obj)
		{
			LayerMask layerMask = 1 << obj.layer;
			LayerMask targetLayers = this.targetTracker.targetLayers;
			return (targetLayers.value & layerMask.value) != 0;
		}
	}
}
