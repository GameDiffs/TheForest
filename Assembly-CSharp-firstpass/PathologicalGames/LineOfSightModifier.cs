using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathologicalGames
{
	[AddComponentMenu("Path-o-logical/TargetPro/Modifier - Line of Sight"), RequireComponent(typeof(TargetTracker))]
	public class LineOfSightModifier : MonoBehaviour
	{
		public enum TEST_MODE
		{
			SinglePoint,
			SixPoint
		}

		public LayerMask targetTrackerLayerMask;

		public LayerMask fireControllerLayerMask;

		public LineOfSightModifier.TEST_MODE testMode;

		public float radius = 1f;

		public DEBUG_LEVELS debugLevel;

		[HideInInspector]
		public TargetTracker tracker;

		[HideInInspector]
		public FireController fireCtrl;

		private void Awake()
		{
			this.tracker = base.GetComponent<TargetTracker>();
			this.fireCtrl = base.GetComponent<FireController>();
			this.tracker.AddOnPostSortDelegate(new TargetTracker.OnPostSortDelegate(this.FilterTrackerTargetList));
			if (this.fireCtrl != null)
			{
				this.fireCtrl.AddOnPreFireDelegate(new FireController.OnPreFireDelegate(this.FilterFireTargetList));
			}
		}

		private void FilterTrackerTargetList(TargetList targets)
		{
			if (this.targetTrackerLayerMask.value == 0)
			{
				return;
			}
			Vector3 position = this.tracker.perimeter.transform.position;
			LayerMask mask = this.targetTrackerLayerMask;
			this.FilterTargetList(targets, mask, position, Color.red);
		}

		private void FilterFireTargetList(TargetList targets)
		{
			if (this.fireControllerLayerMask.value == 0)
			{
				return;
			}
			Vector3 position;
			if (this.fireCtrl.emitter != null)
			{
				position = this.fireCtrl.emitter.position;
			}
			else
			{
				position = this.fireCtrl.transform.position;
			}
			LayerMask mask = this.fireControllerLayerMask;
			this.FilterTargetList(targets, mask, position, Color.yellow);
		}

		private void FilterTargetList(TargetList targets, LayerMask mask, Vector3 fromPos, Color debugLineColor)
		{
			List<Target> list = new List<Target>(targets);
			foreach (Target current in list)
			{
				bool flag = false;
				Vector3 position = current.targetable.xform.position;
				if (this.testMode == LineOfSightModifier.TEST_MODE.SixPoint)
				{
					foreach (Vector3 current2 in new List<Vector3>
					{
						new Vector3(position.x + this.radius, position.y, position.z),
						new Vector3(position.x, position.y + this.radius, position.z),
						new Vector3(position.x, position.y, position.z + this.radius),
						new Vector3(position.x - this.radius, position.y, position.z),
						new Vector3(position.x, position.y - this.radius, position.z),
						new Vector3(position.x, position.y, position.z - this.radius)
					})
					{
						flag = Physics.Linecast(fromPos, current2, mask);
						if (!flag)
						{
							break;
						}
					}
				}
				else
				{
					flag = Physics.Linecast(fromPos, position, mask);
				}
				if (flag)
				{
					targets.Remove(current);
				}
			}
		}
	}
}
