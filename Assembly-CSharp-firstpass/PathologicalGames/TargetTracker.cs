using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathologicalGames
{
	[AddComponentMenu("Path-o-logical/TargetPro/TargetTracker")]
	public class TargetTracker : MonoBehaviour
	{
		public enum PERIMETER_SHAPES
		{
			Capsule,
			Box,
			Sphere
		}

		public enum SORTING_STYLES
		{
			None,
			Nearest,
			Farthest,
			NearestToDestination,
			FarthestFromDestination,
			MostPowerful,
			LeastPowerful
		}

		public interface iTargetComparer : IComparer<Target>
		{
			int Compare(Target targetA, Target targetB);
		}

		public class TargetComparer : IComparer<Target>, TargetTracker.iTargetComparer
		{
			private Transform perimeterPos;

			private TargetTracker.SORTING_STYLES sortStyle;

			public TargetComparer(TargetTracker.SORTING_STYLES sortStyle, Transform perimeterPos)
			{
				this.perimeterPos = perimeterPos;
				this.sortStyle = sortStyle;
			}

			public int Compare(Target targetA, Target targetB)
			{
				switch (this.sortStyle)
				{
				case TargetTracker.SORTING_STYLES.None:
					throw new NotImplementedException("Unexpected option. SORT_OPTIONS.NONE should bypass sorting altogether.");
				case TargetTracker.SORTING_STYLES.Nearest:
				{
					float distToPos = targetA.targetable.GetDistToPos(this.perimeterPos.position);
					float distToPos2 = targetB.targetable.GetDistToPos(this.perimeterPos.position);
					return distToPos.CompareTo(distToPos2);
				}
				case TargetTracker.SORTING_STYLES.Farthest:
				{
					float distToPos3 = targetA.targetable.GetDistToPos(this.perimeterPos.position);
					return targetB.targetable.GetDistToPos(this.perimeterPos.position).CompareTo(distToPos3);
				}
				case TargetTracker.SORTING_STYLES.NearestToDestination:
					return targetA.targetable.distToDest.CompareTo(targetB.targetable.distToDest);
				case TargetTracker.SORTING_STYLES.FarthestFromDestination:
					return targetB.targetable.distToDest.CompareTo(targetA.targetable.distToDest);
				case TargetTracker.SORTING_STYLES.MostPowerful:
					return targetB.targetable.strength.CompareTo(targetA.targetable.strength);
				case TargetTracker.SORTING_STYLES.LeastPowerful:
					return targetA.targetable.strength.CompareTo(targetB.targetable.strength);
				default:
					throw new NotImplementedException(string.Format("Unexpected option '{0}'.", this.sortStyle));
				}
			}
		}

		public delegate void OnPostSortDelegate(TargetList targets);

		public int numberOfTargets = 1;

		[SerializeField]
		private TargetTracker.SORTING_STYLES _sortingStyle = TargetTracker.SORTING_STYLES.Nearest;

		public float sortInterval = 0.1f;

		[SerializeField]
		private Vector3 _range = Vector3.one;

		public LayerMask targetLayers;

		[SerializeField]
		private TargetTracker.PERIMETER_SHAPES _perimeterShape = TargetTracker.PERIMETER_SHAPES.Sphere;

		[SerializeField]
		private Vector3 _perimeterPositionOffset = Vector3.zero;

		[SerializeField]
		private Vector3 _perimeterRotationOffset = Vector3.zero;

		[SerializeField]
		private int _perimeterLayer;

		public DEBUG_LEVELS debugLevel;

		public bool drawGizmo;

		public Color gizmoColor = new Color(0f, 0.7f, 1f, 1f);

		public bool overrideGizmoVisibility;

		protected TargetList _targets = new TargetList();

		public Transform xform;

		protected TargetTracker.OnPostSortDelegate onPostSortDelegates;

		public TargetTracker.SORTING_STYLES sortingStyle
		{
			get
			{
				return this._sortingStyle;
			}
			set
			{
				this._sortingStyle = value;
				if (this.perimeter != null)
				{
					this.perimeter.dirty = true;
				}
			}
		}

		public Vector3 range
		{
			get
			{
				return this._range;
			}
			set
			{
				this._range = value;
				if (this.perimeter != null)
				{
					this.UpdatePerimeterRange();
				}
			}
		}

		public TargetTracker.PERIMETER_SHAPES perimeterShape
		{
			get
			{
				return this._perimeterShape;
			}
			set
			{
				this._perimeterShape = value;
				if (this.perimeter == null)
				{
					return;
				}
				this.UpdatePerimeterShape();
			}
		}

		public Vector3 perimeterPositionOffset
		{
			get
			{
				return this._perimeterPositionOffset;
			}
			set
			{
				this._perimeterPositionOffset = value;
				if (this.perimeter == null)
				{
					return;
				}
				this.perimeter.transform.localPosition = value;
			}
		}

		public Vector3 perimeterRotationOffset
		{
			get
			{
				return this._perimeterRotationOffset;
			}
			set
			{
				this._perimeterRotationOffset = value;
				if (this.perimeter == null)
				{
					return;
				}
				this.perimeter.transform.localRotation = Quaternion.Euler(value);
			}
		}

		public int perimeterLayer
		{
			get
			{
				return this._perimeterLayer;
			}
			set
			{
				this._perimeterLayer = value;
				if (this.perimeter == null)
				{
					return;
				}
				this.perimeter.gameObject.layer = value;
			}
		}

		public Color defaultGizmoColor
		{
			get
			{
				return new Color(0f, 0.7f, 1f, 1f);
			}
		}

		public Perimeter perimeter
		{
			get;
			private set;
		}

		public virtual TargetList targets
		{
			get
			{
				this._targets.Clear();
				if (this.perimeter == null)
				{
					return this._targets;
				}
				if (this.numberOfTargets == 0 || this.perimeter.Count == 0)
				{
					return this._targets;
				}
				if (this.numberOfTargets == -1)
				{
					this._targets.AddRange(this.perimeter);
				}
				else
				{
					int num = Mathf.Clamp(this.numberOfTargets, 0, this.perimeter.Count);
					for (int i = 0; i < num; i++)
					{
						this._targets.Add(this.perimeter[i]);
					}
				}
				if (this.onPostSortDelegates != null)
				{
					this.onPostSortDelegates(this._targets);
				}
				return this._targets;
			}
		}

		protected virtual void Awake()
		{
			this.xform = base.transform;
			GameObject gameObject = new GameObject(base.name + "_Perimeter");
			gameObject.transform.parent = this.xform;
			gameObject.SetActive(false);
			gameObject.SetActive(true);
			gameObject.transform.localPosition = this.perimeterPositionOffset;
			gameObject.transform.localRotation = Quaternion.Euler(this.perimeterRotationOffset);
			gameObject.layer = this.perimeterLayer;
			this.perimeter = gameObject.AddComponent<Perimeter>();
			this.perimeter.targetTracker = this;
			this.perimeter.enabled = false;
			this.UpdatePerimeterShape();
			this.UpdatePerimeterRange();
		}

		private void UpdatePerimeterShape()
		{
			GameObject gameObject = this.perimeter.gameObject;
			Collider component = gameObject.GetComponent<Collider>();
			switch (this.perimeterShape)
			{
			case TargetTracker.PERIMETER_SHAPES.Capsule:
				if (component is CapsuleCollider)
				{
					return;
				}
				break;
			case TargetTracker.PERIMETER_SHAPES.Box:
				if (component is BoxCollider)
				{
					return;
				}
				break;
			case TargetTracker.PERIMETER_SHAPES.Sphere:
				if (component is SphereCollider)
				{
					return;
				}
				break;
			}
			if (component != null)
			{
				UnityEngine.Object.Destroy(component);
			}
			Collider collider = null;
			switch (this.perimeterShape)
			{
			case TargetTracker.PERIMETER_SHAPES.Capsule:
				collider = gameObject.AddComponent<CapsuleCollider>();
				break;
			case TargetTracker.PERIMETER_SHAPES.Box:
				collider = gameObject.AddComponent<BoxCollider>();
				break;
			case TargetTracker.PERIMETER_SHAPES.Sphere:
				collider = gameObject.AddComponent<SphereCollider>();
				break;
			}
			collider.isTrigger = true;
			this.perimeter.dirty = true;
		}

		private void UpdatePerimeterRange()
		{
			Vector3 normalizedRange = this.GetNormalizedRange();
			Collider component = this.perimeter.GetComponent<Collider>();
			if (component is SphereCollider)
			{
				SphereCollider sphereCollider = (SphereCollider)component;
				sphereCollider.radius = normalizedRange.x;
			}
			else if (component is BoxCollider)
			{
				BoxCollider boxCollider = (BoxCollider)component;
				boxCollider.size = normalizedRange;
			}
			else if (component is CapsuleCollider)
			{
				CapsuleCollider capsuleCollider = (CapsuleCollider)component;
				capsuleCollider.radius = normalizedRange.x;
				capsuleCollider.height = normalizedRange.y;
			}
			else
			{
				Debug.LogWarning("Unsupported collider type.");
			}
			this.perimeter.dirty = true;
		}

		public Vector3 GetNormalizedRange()
		{
			Vector3 zero = Vector3.zero;
			switch (this.perimeterShape)
			{
			case TargetTracker.PERIMETER_SHAPES.Capsule:
				zero = new Vector3(this._range.x, this._range.y * 2f, this._range.x);
				break;
			case TargetTracker.PERIMETER_SHAPES.Box:
				zero = new Vector3(this._range.x * 2f, this._range.y, this._range.z * 2f);
				break;
			case TargetTracker.PERIMETER_SHAPES.Sphere:
				zero = new Vector3(this._range.x, this._range.x, this._range.x);
				break;
			}
			return zero;
		}

		protected virtual void OnEnable()
		{
			this.perimeter.enabled = true;
		}

		protected virtual void OnDisable()
		{
			if (this.perimeter == null)
			{
				return;
			}
			this.perimeter.Clear();
			this.perimeter.enabled = false;
		}

		public void AddOnPostSortDelegate(TargetTracker.OnPostSortDelegate del)
		{
			this.onPostSortDelegates = (TargetTracker.OnPostSortDelegate)Delegate.Combine(this.onPostSortDelegates, del);
		}

		public void SetOnPostSortDelegate(TargetTracker.OnPostSortDelegate del)
		{
			this.onPostSortDelegates = del;
		}

		public void RemoveOnPostSortDelegate(TargetTracker.OnPostSortDelegate del)
		{
			this.onPostSortDelegates = (TargetTracker.OnPostSortDelegate)Delegate.Remove(this.onPostSortDelegates, del);
		}
	}
}
