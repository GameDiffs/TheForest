using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Buildings.Interfaces;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[AddComponentMenu("Buildings/Creation/Wall Architect")]
	public class WallArchitect : MonoBehaviour, IStructureSupport
	{
		[Serializable]
		public class HorizontalSegment
		{
			public Vector3 _p1;

			public Vector3 _p2;

			public Vector3 _axis;

			public float _length;

			public Transform _root;
		}

		[Serializable]
		public class Edge
		{
			public Vector3 _p1;

			public Vector3 _p2;

			public float _hlength;

			public WallArchitect.HorizontalSegment[] _segments;

			public Transform _root;
		}

		[Serializable]
		public class WallAddition
		{
			public int _edgeNum;

			public int _segmentNum;
		}

		public WallChunkArchitect _chunkArchitectPrefab;

		public Transform _logPrefab;

		public Renderer _logRenderer;

		public float _closureSnappingDistance = 2.5f;

		public float _maxSegmentHorizontalScale = 2f;

		public int _maxPoints = 50;

		public float _minAngleBetweenEdges = 90f;

		public LayerMask _floorLayers;

		protected bool _checkGround = true;

		protected List<Vector3> _multiPointsPositions;

		protected GameObject _wallRoot;

		protected List<WallArchitect.Edge> _edges;

		protected WallArchitect.Edge _tmpEdge;

		protected float _logLength;

		protected float _logWidth;

		protected float _maxSegmentHorizontalLength;

		protected Queue<Transform> _logPool;

		protected Queue<Transform> _newPool;

		protected Material _logMat;

		private bool CanLock
		{
			get
			{
				bool flag = this._multiPointsPositions.Count < this._maxPoints && (this.CurrentSupport == null || this.CurrentSupport.GetMultiPointsPositions() != null);
				if (flag && this._multiPointsPositions.Count > 0)
				{
					Vector3 to = base.transform.position - this._multiPointsPositions[this._multiPointsPositions.Count - 1];
					flag = (to.sqrMagnitude > this.MinLockLength);
					if (this._multiPointsPositions.Count > 1)
					{
						Vector3 from = this._multiPointsPositions[this._multiPointsPositions.Count - 2] - this._multiPointsPositions[this._multiPointsPositions.Count - 1];
						flag = (flag && Vector3.Angle(from, to) >= this._minAngleBetweenEdges);
					}
				}
				return flag;
			}
		}

		protected virtual float MinLockLength
		{
			get
			{
				return this._logLength * this._logLength;
			}
		}

		public virtual float MaxSegmentHorizontalLength
		{
			get
			{
				return this._logRenderer.bounds.size.z * this._maxSegmentHorizontalScale;
			}
		}

		public List<Vector3> MultiPointsPositions
		{
			get
			{
				return this._multiPointsPositions;
			}
			set
			{
				this._multiPointsPositions = value;
			}
		}

		public float LogWidth
		{
			get
			{
				return this._logWidth;
			}
		}

		protected virtual float SegmentPointTestOffset
		{
			get
			{
				return this._logWidth * 3.5f;
			}
		}

		private IStructureSupport CurrentSupport
		{
			get;
			set;
		}

		[SerializeThis]
		public bool Enslaved
		{
			get;
			set;
		}

		private void Awake()
		{
			if (!LevelSerializer.IsDeserializing)
			{
				base.StartCoroutine(this.DelayedAwake());
			}
			else
			{
				base.enabled = false;
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayedAwake()
		{
			WallArchitect.<DelayedAwake>c__Iterator129 <DelayedAwake>c__Iterator = new WallArchitect.<DelayedAwake>c__Iterator129();
			<DelayedAwake>c__Iterator.<>f__this = this;
			return <DelayedAwake>c__Iterator;
		}

		private void Update()
		{
			bool flag = this._multiPointsPositions.Count >= 2;
			if (LocalPlayer.Create.BuildingPlacer.Clear != flag)
			{
				Scene.HudGui.PlaceWallIcon.SetActive(flag);
				LocalPlayer.Create.BuildingPlacer.Clear = flag;
			}
			bool flag2 = false;
			if (TheForest.Utils.Input.GetButtonDown("AltFire") && this._multiPointsPositions.Count > 0)
			{
				UnityEngine.Object.Destroy(this._tmpEdge._root.gameObject);
				this._tmpEdge = null;
				if (this._multiPointsPositions.Count > 1)
				{
					UnityEngine.Object.Destroy(this._edges[this._edges.Count - 1]._root.gameObject);
					this._edges.RemoveAt(this._edges.Count - 1);
				}
				this._multiPointsPositions.RemoveAt(this._multiPointsPositions.Count - 1);
			}
			bool canLock = this.CanLock;
			if (canLock && this._multiPointsPositions.Count > 1)
			{
				Vector3 forward = this._multiPointsPositions[this._multiPointsPositions.Count - 2] - this._multiPointsPositions[this._multiPointsPositions.Count - 1];
				Scene.HudGui.AngleAndDistanceGizmoWall.gameObject.SetActive(true);
				Scene.HudGui.AngleAndDistanceGizmoWall.position = this._multiPointsPositions.Last<Vector3>() + Vector3.up;
				Scene.HudGui.AngleAndDistanceGizmoWall.rotation = Quaternion.LookRotation(forward);
			}
			else
			{
				Scene.HudGui.AngleAndDistanceGizmoWall.gameObject.SetActive(false);
			}
			if (TheForest.Utils.Input.GetButtonDown("Fire1") && canLock)
			{
				this.LockCurrentPoint();
				base.GetComponent<Renderer>().enabled = false;
			}
			bool flag3 = this._multiPointsPositions.Count == 0 && this.CurrentSupport != null && this.CurrentSupport.GetMultiPointsPositions() != null;
			if (TheForest.Utils.Input.GetButtonAfterDelay("Craft", 0.5f) && flag3)
			{
				this._multiPointsPositions = new List<Vector3>(this.CurrentSupport.GetMultiPointsPositions());
				float height = this.CurrentSupport.GetHeight();
				for (int i = 0; i < this._multiPointsPositions.Count; i++)
				{
					Vector3 value = this._multiPointsPositions[i];
					value.y += height;
					this._multiPointsPositions[i] = value;
				}
				this._checkGround = false;
				this._newPool = new Queue<Transform>();
				this.CreateStructure();
				flag3 = false;
				flag2 = true;
			}
			bool flag4 = this._multiPointsPositions.Count > 2 && Vector3.Distance(this._multiPointsPositions.First<Vector3>(), this._multiPointsPositions.Last<Vector3>()) > this._logLength;
			if (flag4 && TheForest.Utils.Input.GetButtonDown("Rotate"))
			{
				this._multiPointsPositions.Add(this._multiPointsPositions.First<Vector3>());
				this._newPool = new Queue<Transform>();
				WallArchitect.Edge edge = this.CreateEdge(this._multiPointsPositions[this._multiPointsPositions.Count - 2], this._multiPointsPositions[this._multiPointsPositions.Count - 1]);
				edge._root.parent = this._wallRoot.transform;
				this._edges.Add(edge);
				this._logPool.Clear();
				flag2 = true;
			}
			this.ShowTempWall();
			if (flag2)
			{
				LocalPlayer.Create.PlaceGhost(false);
			}
			Scene.HudGui.LockPositionIcon.SetActive(this._multiPointsPositions.Count < this._maxPoints && canLock);
			Scene.HudGui.UnlockPositionIcon.SetActive(this._multiPointsPositions.Count > 0);
			Scene.HudGui.AutoFillPointsIcon.SetActive(flag3);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("structure") || other.CompareTag("jumpObject"))
			{
				IStructureSupport structureSupport = (IStructureSupport)other.GetComponentInParent(typeof(IStructureSupport));
				if (structureSupport != null)
				{
					this.CurrentSupport = structureSupport;
					return;
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("structure") || other.CompareTag("jumpObject"))
			{
				IStructureSupport structureSupport = (IStructureSupport)other.GetComponentInParent(typeof(IStructureSupport));
				if (structureSupport == this.CurrentSupport)
				{
					this.CurrentSupport = null;
				}
			}
		}

		private void OnDestroy()
		{
			if (Scene.HudGui)
			{
				Scene.HudGui.PlaceWallIcon.SetActive(false);
				Scene.HudGui.LockPositionIcon.SetActive(false);
				Scene.HudGui.UnlockPositionIcon.SetActive(false);
				Scene.HudGui.AutoFillPointsIcon.SetActive(false);
				Scene.HudGui.AngleAndDistanceGizmo.gameObject.SetActive(false);
				Scene.HudGui.SnappingGridGizmo.gameObject.SetActive(false);
			}
			if (LocalPlayer.Create)
			{
				LocalPlayer.Create.Grabber.ClosePlace();
			}
			this.Clear();
		}

		private void OnDeserialized()
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		[DebuggerHidden]
		private IEnumerator OnPlaced()
		{
			WallArchitect.<OnPlaced>c__Iterator12A <OnPlaced>c__Iterator12A = new WallArchitect.<OnPlaced>c__Iterator12A();
			<OnPlaced>c__Iterator12A.<>f__this = this;
			return <OnPlaced>c__Iterator12A;
		}

		public void Clear()
		{
			if (this._edges != null)
			{
				foreach (WallArchitect.Edge current in this._edges)
				{
					if (current._root)
					{
						UnityEngine.Object.Destroy(current._root.gameObject);
					}
				}
				this._edges.Clear();
			}
			if (this._tmpEdge != null && this._tmpEdge._root)
			{
				UnityEngine.Object.Destroy(this._tmpEdge._root.gameObject);
			}
			if (this._wallRoot)
			{
				UnityEngine.Object.Destroy(this._wallRoot);
			}
		}

		private bool LockCurrentPoint()
		{
			bool result = false;
			if (this._multiPointsPositions.Count > 2 && Vector3.Distance(base.transform.position, this._multiPointsPositions[0]) < this._closureSnappingDistance)
			{
				this._multiPointsPositions.Add(this._multiPointsPositions[0]);
				result = true;
			}
			else
			{
				this._multiPointsPositions.Add(base.transform.position);
			}
			if (this._multiPointsPositions.Count > 1)
			{
				this._newPool = new Queue<Transform>();
				WallArchitect.Edge edge = this.CreateEdge(this._multiPointsPositions[this._multiPointsPositions.Count - 2], this._multiPointsPositions[this._multiPointsPositions.Count - 1]);
				edge._root.parent = this._wallRoot.transform;
				this._edges.Add(edge);
				this._logPool.Clear();
			}
			return result;
		}

		protected virtual void ShowTempWall()
		{
			Vector3 p = (this._multiPointsPositions.Count <= 0) ? (base.transform.position - LocalPlayer.MainCamTr.right / 2f) : this._multiPointsPositions[this._multiPointsPositions.Count - 1];
			Vector3 p2;
			if (this._multiPointsPositions.Count > 2 && Vector3.Distance(base.transform.position, this._multiPointsPositions[0]) < this._closureSnappingDistance)
			{
				p2 = this._multiPointsPositions[0];
			}
			else if (this._multiPointsPositions.Count == 0)
			{
				p2 = base.transform.position + LocalPlayer.MainCamTr.right / 2f;
			}
			else
			{
				p2 = base.transform.position;
			}
			GameObject gameObject = null;
			if (this._tmpEdge != null && this._tmpEdge._root)
			{
				gameObject = this._tmpEdge._root.gameObject;
			}
			this._newPool = new Queue<Transform>();
			this._tmpEdge = this.CreateEdge(p, p2);
			this._tmpEdge._root.name = "TempWall";
			this._logPool = this._newPool;
			if (gameObject)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}

		private Vector3 GetSegmentPointFloorPosition(Vector3 segmentPoint)
		{
			if (!this._checkGround)
			{
				return segmentPoint;
			}
			segmentPoint.y += this.SegmentPointTestOffset;
			RaycastHit raycastHit;
			if (Physics.SphereCast(segmentPoint, this._logWidth * 0.4f, Vector3.down, out raycastHit, this._logLength * 2f + this.SegmentPointTestOffset, Scene.ValidateFloorLayers(segmentPoint, this._floorLayers.value)))
			{
				return raycastHit.point;
			}
			return segmentPoint;
		}

		protected WallArchitect.Edge CreateEdge(Vector3 p1, Vector3 p2)
		{
			WallArchitect.Edge edge = this.CalcEdge(p1, p2);
			edge._root = this.SpawnEdge(edge);
			return edge;
		}

		private WallArchitect.Edge CalcEdge(Vector3 p1, Vector3 p2)
		{
			WallArchitect.Edge edge = new WallArchitect.Edge
			{
				_p1 = p1,
				_p2 = p2
			};
			edge._hlength = Vector3.Scale(p2 - p1, new Vector3(1f, 0f, 1f)).magnitude;
			edge._segments = new WallArchitect.HorizontalSegment[Mathf.CeilToInt(edge._hlength / this._maxSegmentHorizontalLength)];
			float d = edge._hlength / (float)edge._segments.Length;
			float num = this._logWidth / 2f * 0.98f;
			Vector3 b = (p2 - p1).normalized * d;
			Vector3 vector = p1;
			for (int i = 0; i < edge._segments.Length; i++)
			{
				WallArchitect.HorizontalSegment horizontalSegment = new WallArchitect.HorizontalSegment();
				horizontalSegment._p1 = vector;
				horizontalSegment._p2 = ((i + 1 != edge._segments.Length) ? this.GetSegmentPointFloorPosition(vector + b) : p2);
				horizontalSegment._axis = (horizontalSegment._p2 - horizontalSegment._p1).normalized * d;
				horizontalSegment._length = Vector3.Distance(horizontalSegment._p1, horizontalSegment._p2);
				vector = horizontalSegment._p2;
				WallArchitect.HorizontalSegment expr_13C_cp_0 = horizontalSegment;
				expr_13C_cp_0._p1.y = expr_13C_cp_0._p1.y + num;
				WallArchitect.HorizontalSegment expr_150_cp_0 = horizontalSegment;
				expr_150_cp_0._p2.y = expr_150_cp_0._p2.y + num;
				edge._segments[i] = horizontalSegment;
			}
			return edge;
		}

		protected virtual Transform SpawnEdge(WallArchitect.Edge edge)
		{
			Transform transform = new GameObject("WallEdge").transform;
			transform.transform.position = edge._p1;
			Vector3 b = new Vector3(0f, this._logWidth * 0.95f, 0f);
			for (int i = 0; i < edge._segments.Length; i++)
			{
				WallArchitect.HorizontalSegment horizontalSegment = edge._segments[i];
				Quaternion rotation = Quaternion.LookRotation(horizontalSegment._axis);
				Vector3 vector = horizontalSegment._p1;
				Transform transform2 = new GameObject("Segment" + i).transform;
				transform2.parent = transform;
				transform2.LookAt(horizontalSegment._axis);
				horizontalSegment._root = transform2;
				transform2.position = horizontalSegment._p1;
				Vector3 localScale = new Vector3(1f, 1f, horizontalSegment._length / this._logLength);
				Vector3 vector2 = new Vector3(1f, 1f, 0.31f + (localScale.z - 1f) / 2f);
				float num = 1f - vector2.z / localScale.z;
				for (int j = 0; j < 5; j++)
				{
					Transform transform3 = this.NewLog(vector, rotation);
					transform3.parent = transform2;
					this._newPool.Enqueue(transform3);
					transform3.localScale = localScale;
					vector += b;
				}
			}
			return transform;
		}

		protected Transform NewLog(Vector3 position, Quaternion rotation)
		{
			if (this._logPool.Count > 0)
			{
				Transform transform = this._logPool.Dequeue();
				transform.position = position;
				transform.rotation = rotation;
				return transform;
			}
			Transform transform2 = (Transform)UnityEngine.Object.Instantiate(this._logPrefab, position, rotation);
			transform2.GetComponentInChildren<Renderer>().sharedMaterial = this._logMat;
			return transform2;
		}

		private void CreateStructure()
		{
			for (int i = 1; i < this._multiPointsPositions.Count; i++)
			{
				WallArchitect.Edge item = this.CalcEdge(this._multiPointsPositions[i - 1], this._multiPointsPositions[i]);
				this._edges.Add(item);
			}
		}

		public float GetLevel()
		{
			return this._multiPointsPositions[0].y + this.GetHeight();
		}

		public virtual float GetHeight()
		{
			return 4.3f * this._logWidth;
		}

		public List<Vector3> GetMultiPointsPositions()
		{
			return this._multiPointsPositions;
		}
	}
}
