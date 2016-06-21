using Bolt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Buildings.Interfaces;
using TheForest.Buildings.World;
using TheForest.Items;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Foundation Architect")]
	public class FoundationArchitect : MonoBehaviour, IStructureSupport, ICoopStructure, ICoopAnchorStructure
	{
		public enum Modes
		{
			Auto,
			Manual,
			ManualSlave
		}

		[Serializable]
		public class VerticalSegment
		{
			public bool _diag;
		}

		[Serializable]
		public class HorizontalSegment
		{
			public Vector3 _p1;

			public Vector3 _p2;

			public float _height;

			public FoundationArchitect.VerticalSegment[] _segments;
		}

		[Serializable]
		public class Edge
		{
			public bool _isFirst;

			public bool _isClosing;

			public Vector3 _p1;

			public Vector3 _p2;

			public Vector3 _axis;

			public float _hlength;

			public FoundationArchitect.HorizontalSegment[] _segments;

			public int _totalLogs;
		}

		public delegate bool SegmentTierValidator(int segmentNum, int tierNum);

		private const float MINIMUM_AUDIBLE_WIND_HEIGHT = 2f;

		public FoundationArchitect.Modes _mode;

		[SerializeThis]
		public bool _wasPlaced;

		[SerializeThis]
		public bool _wasBuilt;

		[SerializeThis]
		public bool _aboveGround;

		public Transform _logPrefab;

		public Renderer _logRenderer;

		public float _closureSnappingDistance = 2.5f;

		public float _maxLogScale = 2f;

		public float _maxScaleLogCost = 1f;

		public float _minAngleBetweenEdges = 90f;

		public int _maxPoints = 10;

		public float _maxHeightPercentWithoutDiagonal = 0.65f;

		public LayerMask _floorLayers;

		public Craft_Structure _craftStructure;

		[ItemIdPicker(Item.Types.Equipment)]
		public int _logItemId;

		public GameObject _logTextPrefab;

		public GameObject _logIconPrefab;

		public BoxCollider _autoModeBounds;

		private bool _initialized;

		[SerializeThis]
		private List<Vector3> _multiPointsPositions;

		[SerializeThis]
		private int _multiPointsPositionsCount;

		private List<FoundationArchitect.Edge> _edges;

		private GameObject _foundationRoot;

		private List<GameObject> _edgesGo;

		private FoundationArchitect.Edge _tmpEdge;

		private GameObject _tmpEdgeGo;

		private float _logLength;

		private float _logWidth;

		private float _maxHeightWithoutDiagonal;

		private float _maxSegmentHorizontalLength;

		private float _minEdgeLength;

		private Stack<Transform> _logPool;

		private Stack<Transform> _newPool;

		private Material _logMat;

		private List<StructureAnchor> _anchors = new List<StructureAnchor>();

		private List<FoundationChunkTier> _chunks = new List<FoundationChunkTier>();

		public FoundationArchitect.SegmentTierValidator SegmentTierValidation = (int segmentNum, int tierNum) => true;

		public List<StructureAnchor> Anchors
		{
			get
			{
				return this._anchors;
			}
			set
			{
				this._anchors = value;
			}
		}

		[SerializeThis]
		public bool Enslaved
		{
			get;
			set;
		}

		public bool WasBuilt
		{
			get
			{
				return this._wasBuilt;
			}
			set
			{
				this._wasBuilt = value;
			}
		}

		public bool WasPlaced
		{
			get
			{
				return this._wasPlaced;
			}
			set
			{
				this._wasPlaced = value;
			}
		}

		public int MultiPointsCount
		{
			get
			{
				return this._multiPointsPositionsCount;
			}
			set
			{
				this._multiPointsPositionsCount = value;
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

		public List<FoundationArchitect.Edge> Edges
		{
			get
			{
				return this._edges;
			}
		}

		private bool InClosureSnappingRange
		{
			get
			{
				return this._multiPointsPositions.Count > 2 && Vector3.Distance(base.transform.position, this._multiPointsPositions[0]) <= this._closureSnappingDistance;
			}
		}

		public float LogWidth
		{
			get
			{
				return this._logWidth;
			}
		}

		public int ChunkCount
		{
			get;
			private set;
		}

		public float MaxHeight
		{
			get;
			private set;
		}

		public GameObject FoundationRoot
		{
			get
			{
				return this._foundationRoot;
			}
		}

		public IProtocolToken CustomToken
		{
			get;
			set;
		}

		private void Awake()
		{
			base.StartCoroutine(this.DelayedAwake(LevelSerializer.IsDeserializing));
			base.enabled = false;
		}

		[DebuggerHidden]
		private IEnumerator DelayedAwake(bool isDeserializing)
		{
			FoundationArchitect.<DelayedAwake>c__Iterator135 <DelayedAwake>c__Iterator = new FoundationArchitect.<DelayedAwake>c__Iterator135();
			<DelayedAwake>c__Iterator.isDeserializing = isDeserializing;
			<DelayedAwake>c__Iterator.<$>isDeserializing = isDeserializing;
			<DelayedAwake>c__Iterator.<>f__this = this;
			return <DelayedAwake>c__Iterator;
		}

		private void LateUpdate()
		{
			if (BoltNetwork.isRunning && this._wasPlaced)
			{
				this._craftStructure.enabled = true;
			}
			switch (this._mode)
			{
			case FoundationArchitect.Modes.Auto:
				this.UpdateAuto();
				break;
			case FoundationArchitect.Modes.Manual:
			case FoundationArchitect.Modes.ManualSlave:
				this.UpdateManual();
				break;
			}
		}

		private void UpdateAuto()
		{
			if (this._logMat)
			{
				this._logMat.SetColor("_TintColor", (!LocalPlayer.Create.BuildingPlacer.Clear) ? LocalPlayer.Create.BuildingPlacer.RedMat.GetColor("_TintColor") : LocalPlayer.Create.BuildingPlacer.ClearMat.GetColor("_TintColor"));
				GameObject foundationRoot = this._foundationRoot;
				if (this._edges != null)
				{
					this._edges.Clear();
				}
				if (this._edgesGo != null)
				{
					this._edgesGo.Clear();
				}
				this._aboveGround = !Scene.Atmosphere.InACave;
				this.RecalcPointsFromAutoBounds();
				this._foundationRoot = new GameObject("FoundationRoot");
				this._foundationRoot.transform.parent = base.transform;
				this._newPool = new Stack<Transform>();
				this.CreateStructure(false);
				this._logPool = this._newPool;
				UnityEngine.Object.Destroy(foundationRoot);
			}
		}

		private void UpdateManual()
		{
			bool flag = this._multiPointsPositions.Count >= 3;
			if (LocalPlayer.Create.BuildingPlacer.Clear != flag || Scene.HudGui.PlaceWallIcon.activeSelf != flag)
			{
				Scene.HudGui.PlaceWallIcon.SetActive(flag);
				LocalPlayer.Create.BuildingPlacer.Clear = flag;
			}
			bool flag2 = this._multiPointsPositions.Count < this._maxPoints;
			if (flag2 && this._multiPointsPositions.Count > 0)
			{
				Vector3 to = base.transform.position - this._multiPointsPositions[this._multiPointsPositions.Count - 1];
				flag2 = (to.sqrMagnitude > this._minEdgeLength * this._minEdgeLength);
				if (this._multiPointsPositions.Count > 1)
				{
					Vector3 vector = this._multiPointsPositions[this._multiPointsPositions.Count - 2] - this._multiPointsPositions[this._multiPointsPositions.Count - 1];
					flag2 = (flag2 && Vector3.Angle(vector, to) >= this._minAngleBetweenEdges);
					Scene.HudGui.AngleAndDistanceGizmo.gameObject.SetActive(true);
					Scene.HudGui.AngleAndDistanceGizmo.position = this._multiPointsPositions.Last<Vector3>();
					Scene.HudGui.AngleAndDistanceGizmo.rotation = Quaternion.LookRotation(vector);
				}
				else
				{
					Scene.HudGui.AngleAndDistanceGizmo.gameObject.SetActive(false);
				}
			}
			else
			{
				Scene.HudGui.AngleAndDistanceGizmo.gameObject.SetActive(false);
			}
			if (TheForest.Utils.Input.GetButtonDown("Fire1") && flag2)
			{
				bool inClosureSnappingRange = this.InClosureSnappingRange;
				Vector3 vector2;
				if (!inClosureSnappingRange)
				{
					vector2 = this.GetCurrentEdgePoint();
				}
				else
				{
					vector2 = this._multiPointsPositions[0];
				}
				this._multiPointsPositions.Add(vector2);
				if (this._multiPointsPositions.Count > 1)
				{
					this._newPool = new Stack<Transform>();
					this.CreateEdge(this._multiPointsPositions[this._multiPointsPositions.Count - 2], vector2, this._multiPointsPositions.Count == 2, inClosureSnappingRange);
					this._logPool.Clear();
				}
				else
				{
					this._aboveGround = !Scene.Atmosphere.InACave;
				}
				if (inClosureSnappingRange)
				{
					LocalPlayer.Create.PlaceGhost(false);
				}
			}
			if (TheForest.Utils.Input.GetButtonDown("AltFire") && this._multiPointsPositions.Count > 0)
			{
				if (this._multiPointsPositions.Count > 1)
				{
					UnityEngine.Object.Destroy(this._edgesGo[this._edgesGo.Count - 1]);
					this._edgesGo.RemoveAt(this._edgesGo.Count - 1);
					this._edges.RemoveAt(this._edges.Count - 1);
				}
				this._multiPointsPositions.RemoveAt(this._multiPointsPositions.Count - 1);
			}
			bool flag3 = this._multiPointsPositions.Count > 2 && Vector3.Distance(this._multiPointsPositions.First<Vector3>(), this._multiPointsPositions.Last<Vector3>()) > this._minEdgeLength;
			if (flag3 && TheForest.Utils.Input.GetButtonDown("Rotate"))
			{
				this._multiPointsPositions.Add(this._multiPointsPositions.First<Vector3>());
				this._newPool = new Stack<Transform>();
				this.CreateEdge(this._multiPointsPositions[this._multiPointsPositions.Count - 2], this._multiPointsPositions[this._multiPointsPositions.Count - 1], false, true);
				this._logPool.Clear();
				LocalPlayer.Create.PlaceGhost(false);
			}
			if (this._multiPointsPositions.Count > 0)
			{
				bool inClosureSnappingRange2 = this.InClosureSnappingRange;
				Vector3 vector2;
				if (!inClosureSnappingRange2)
				{
					vector2 = this.GetCurrentEdgePoint();
				}
				else
				{
					vector2 = this._multiPointsPositions[0];
				}
				base.GetComponent<Renderer>().enabled = false;
				base.transform.localScale = Vector3.one;
				this._tmpEdge = this.CalcEdge(this._multiPointsPositions[this._multiPointsPositions.Count - 1], vector2);
				this._newPool = new Stack<Transform>();
				this._tmpEdge._isFirst = (this._multiPointsPositions.Count == 1);
				this._tmpEdge._isClosing = inClosureSnappingRange2;
				GameObject tmpEdgeGo = this.SpawnEdge(this._tmpEdge, -1);
				if (this._tmpEdgeGo)
				{
					UnityEngine.Object.Destroy(this._tmpEdgeGo);
				}
				this._logPool = this._newPool;
				this._tmpEdgeGo = tmpEdgeGo;
				this._tmpEdgeGo.name = "FoundationTempEdge";
				this._logPool = this._newPool;
			}
			else if (this._tmpEdgeGo)
			{
				UnityEngine.Object.Destroy(this._tmpEdgeGo);
				this._tmpEdgeGo = null;
				this._logPool.Clear();
			}
			else
			{
				base.GetComponent<Renderer>().enabled = true;
				base.transform.localScale = new Vector3(1f, Mathf.Abs(this.GetSegmentPointFloorPosition(base.transform.position).y - base.transform.position.y), 1f);
			}
			if (this._mode == FoundationArchitect.Modes.Manual)
			{
				Scene.HudGui.LockPositionIcon.SetActive(flag2);
				Scene.HudGui.UnlockPositionIcon.SetActive(this._multiPointsPositions.Count > 0);
			}
		}

		private void OnDestroy()
		{
			if (Scene.HudGui)
			{
				Scene.HudGui.LockPositionIcon.SetActive(false);
				Scene.HudGui.UnlockPositionIcon.SetActive(false);
			}
			this.Clear();
		}

		private void OnSerializing()
		{
			this._multiPointsPositions.Capacity = this._multiPointsPositions.Count;
			this._multiPointsPositionsCount = this._multiPointsPositions.Count;
		}

		private void OnDeserialized()
		{
			if (!this._initialized)
			{
				this._initialized = true;
				this._multiPointsPositions.RemoveRange(this._multiPointsPositionsCount, this._multiPointsPositions.Count - this._multiPointsPositionsCount);
				if (this._wasBuilt)
				{
					if (this._mode == FoundationArchitect.Modes.Auto)
					{
						this.RecalcPointsFromAutoBounds();
					}
					this.CreateStructure(false);
					FoundationArchitect.CreateWindSFX(this._foundationRoot.transform);
					this._foundationRoot.transform.parent = base.transform;
					this._logPool = null;
					this._newPool = null;
					this._edgesGo = null;
				}
				else if (this._wasPlaced || this._mode == FoundationArchitect.Modes.Auto)
				{
					if (this._mode == FoundationArchitect.Modes.Auto)
					{
						this.RecalcPointsFromAutoBounds();
					}
					this.CreateStructure(false);
					base.StartCoroutine(this.OnPlaced());
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator OnPlaced()
		{
			FoundationArchitect.<OnPlaced>c__Iterator136 <OnPlaced>c__Iterator = new FoundationArchitect.<OnPlaced>c__Iterator136();
			<OnPlaced>c__Iterator.<>f__this = this;
			return <OnPlaced>c__Iterator;
		}

		private void OnBuilt(GameObject built)
		{
			FoundationArchitect foundationArchitect = built.GetComponent<FoundationArchitect>();
			if (!foundationArchitect)
			{
				foundationArchitect = built.AddComponent<FoundationArchitect>();
				foundationArchitect.Clone(this);
				foundationArchitect._logPrefab = Prefabs.Instance.LogBuiltPrefab;
			}
			foundationArchitect._multiPointsPositions = this._multiPointsPositions;
			foundationArchitect._wasBuilt = true;
			foundationArchitect.OnSerializing();
			Craft_Structure expr_4F = this._craftStructure;
			expr_4F.OnBuilt = (Action<GameObject>)Delegate.Remove(expr_4F.OnBuilt, new Action<GameObject>(this.OnBuilt));
		}

		public void Clear()
		{
			if (this._edges != null)
			{
				this._edges.Clear();
			}
			if (this._edgesGo != null)
			{
				foreach (GameObject current in this._edgesGo)
				{
					UnityEngine.Object.Destroy(current);
				}
			}
			if (this._chunks != null)
			{
				this._chunks.Clear();
			}
			if (this._foundationRoot)
			{
				this._foundationRoot.transform.position = new Vector3(0f, 2000f, 0f);
				UnityEngine.Object.Destroy(this._foundationRoot);
			}
		}

		public void CreateStructure(bool isRepair = false)
		{
			if (this._wasBuilt && isRepair)
			{
				this.Clear();
				base.StartCoroutine(this.DelayedAwake(true));
			}
			for (int i = 1; i < this._multiPointsPositions.Count; i++)
			{
				FoundationArchitect.Edge edge = this.CalcEdge(this._multiPointsPositions[i - 1], this._multiPointsPositions[i]);
				this._edges.Add(edge);
				edge._isFirst = (i == 1);
				edge._isClosing = (i == this._multiPointsPositions.Count - 1 && Vector3.Distance(this._multiPointsPositions.First<Vector3>(), this._multiPointsPositions.Last<Vector3>()) < this._closureSnappingDistance);
			}
			this.ChunkCount = this._edges.Sum((FoundationArchitect.Edge e) => e._segments.Length);
			this.MaxHeight = this._edges.Max((FoundationArchitect.Edge e) => e._segments.Max((FoundationArchitect.HorizontalSegment s) => s._height));
			for (int j = 0; j < this._edges.Count; j++)
			{
				this._edgesGo.Add(this.SpawnEdge(this._edges[j], j));
			}
			if (this._wasBuilt && isRepair)
			{
				this._foundationRoot.transform.parent = base.transform;
			}
		}

		public static void CreateWindSFX(Transform root)
		{
			Collider[] allComponentsInChildren = root.GetAllComponentsInChildren<Collider>();
			if (allComponentsInChildren.Length > 0)
			{
				Bounds bounds = allComponentsInChildren[0].bounds;
				for (int i = 1; i < allComponentsInChildren.Length; i++)
				{
					bounds.Encapsulate(allComponentsInChildren[i].bounds);
				}
				float num = Terrain.activeTerrain.SampleHeight(bounds.center);
				float y = Terrain.activeTerrain.transform.position.y;
				float num2 = bounds.max.y - (num + y);
				if (num2 >= 2f)
				{
					FMOD_StudioEventEmitter.CreateAmbientEmitter(root, bounds.center, "event:/ambient/wind/wind_moan_structures");
				}
			}
		}

		public int GetAnchorIndex(StructureAnchor anchor)
		{
			if (this._mode == FoundationArchitect.Modes.Auto)
			{
				StructureAnchor[] componentsInChildren = base.gameObject.GetComponentsInChildren<StructureAnchor>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (object.ReferenceEquals(anchor, componentsInChildren[i]))
					{
						return i;
					}
				}
			}
			else
			{
				for (int j = 0; j < this._anchors.Count; j++)
				{
					if (object.ReferenceEquals(anchor, this._anchors[j]))
					{
						return j;
					}
				}
			}
			throw new Exception("Unknown anchor: " + anchor);
		}

		public void Clone(FoundationArchitect other)
		{
			this._mode = other._mode;
			this._logPrefab = other._logPrefab;
			this._logRenderer = other._logRenderer;
			this._maxLogScale = other._maxLogScale;
			this._maxHeightPercentWithoutDiagonal = other._maxHeightPercentWithoutDiagonal;
			this._floorLayers = other._floorLayers;
			this._craftStructure = other._craftStructure;
			this._logItemId = other._logItemId;
			this._logTextPrefab = other._logTextPrefab;
			this._logIconPrefab = other._logIconPrefab;
			this._aboveGround = other._aboveGround;
		}

		private void RecalcPointsFromAutoBounds()
		{
			this._multiPointsPositions.Clear();
			Vector3 position = this._autoModeBounds.center - this._autoModeBounds.size / 2f;
			Vector3 vector = this._autoModeBounds.center + this._autoModeBounds.size / 2f;
			Transform transform = this._autoModeBounds.transform;
			this._multiPointsPositions.Add(transform.TransformPoint(position));
			this._multiPointsPositions.Add(transform.TransformPoint(new Vector3(position.x, position.y, vector.z)));
			this._multiPointsPositions.Add(transform.TransformPoint(new Vector3(vector.x, position.y, vector.z)));
			this._multiPointsPositions.Add(transform.TransformPoint(new Vector3(vector.x, position.y, position.z)));
			this._multiPointsPositions.Add(this._multiPointsPositions[0]);
			Terrain activeTerrain = Terrain.activeTerrain;
			for (int i = 1; i < this._multiPointsPositions.Count; i++)
			{
				if (activeTerrain.SampleHeight(this._multiPointsPositions[i]) < this._multiPointsPositions[i].y)
				{
					this._aboveGround = true;
					break;
				}
			}
		}

		public float GetEdgeSegmentLength(float edgeLength)
		{
			return edgeLength / (float)Mathf.CeilToInt(edgeLength / this._maxSegmentHorizontalLength);
		}

		private float CalcDiagonalLength(float height, float length)
		{
			return Mathf.Sqrt(height * height + length * length);
		}

		private Vector3 GetCurrentEdgePoint()
		{
			Vector3 position = base.transform.position;
			if (this._multiPointsPositions.Count > 0)
			{
				position.y = this._multiPointsPositions[0].y;
			}
			else
			{
				position.y -= this._logWidth / 2f;
			}
			return position;
		}

		private Vector3 GetSegmentPointFloorPosition(Vector3 segmentPoint)
		{
			bool flag = Scene.IsInSinkhole(segmentPoint);
			RaycastHit raycastHit;
			if (Physics.Raycast(segmentPoint, Vector3.down, out raycastHit, (float)((!flag) ? 150 : 450), (!flag) ? this._floorLayers.value : Scene.ValidateFloorLayers(segmentPoint, this._floorLayers.value)))
			{
				if (this._aboveGround && !flag)
				{
					float value = Terrain.activeTerrain.SampleHeight(segmentPoint);
					if (Math.Round((decimal)value, 3) > Math.Round((decimal)raycastHit.point.y, 3))
					{
						return segmentPoint;
					}
				}
				return raycastHit.point;
			}
			segmentPoint.y -= this._logLength / 2f;
			return segmentPoint;
		}

		private void CreateEdge(Vector3 p1, Vector3 p2, bool isFirst, bool isClosing)
		{
			FoundationArchitect.Edge edge = this.CalcEdge(p1, p2);
			this._edges.Add(edge);
			edge._isFirst = isFirst;
			edge._isClosing = isClosing;
			this._edgesGo.Add(this.SpawnEdge(edge, this._edgesGo.Count));
		}

		private FoundationArchitect.Edge CalcEdge(Vector3 p1, Vector3 p2)
		{
			FoundationArchitect.Edge edge = new FoundationArchitect.Edge
			{
				_p1 = p1,
				_p2 = p2
			};
			edge._hlength = Vector3.Distance(p1, p2);
			edge._segments = new FoundationArchitect.HorizontalSegment[Mathf.CeilToInt(edge._hlength / this._maxSegmentHorizontalLength)];
			float edgeSegmentLength = this.GetEdgeSegmentLength(edge._hlength);
			edge._axis = (p2 - p1).normalized * edgeSegmentLength;
			Vector3 vector = p1;
			Vector3 b = this.GetSegmentPointFloorPosition(vector);
			for (int i = 0; i < edge._segments.Length; i++)
			{
				FoundationArchitect.HorizontalSegment horizontalSegment = new FoundationArchitect.HorizontalSegment();
				horizontalSegment._p1 = vector;
				horizontalSegment._p2 = ((i + 1 != edge._segments.Length) ? (vector + edge._axis) : p2);
				Vector3 segmentPointFloorPosition = this.GetSegmentPointFloorPosition(horizontalSegment._p2);
				horizontalSegment._height = Mathf.Max(Vector3.Distance(vector, b), Vector3.Distance(horizontalSegment._p2, segmentPointFloorPosition));
				horizontalSegment._segments = new FoundationArchitect.VerticalSegment[Mathf.CeilToInt(horizontalSegment._height / this._logLength)];
				for (int j = 0; j < horizontalSegment._segments.Length; j++)
				{
					FoundationArchitect.VerticalSegment verticalSegment = new FoundationArchitect.VerticalSegment();
					float num = (j + 1 != horizontalSegment._segments.Length) ? this._logLength : (horizontalSegment._height - (float)j * this._logLength);
					if (num >= this._maxHeightWithoutDiagonal)
					{
						verticalSegment._diag = true;
					}
					horizontalSegment._segments[j] = verticalSegment;
				}
				vector = horizontalSegment._p2;
				b = segmentPointFloorPosition;
				edge._segments[i] = horizontalSegment;
			}
			return edge;
		}

		private void StackLogsByScale(ref float currentLogStackRemainingScale, float logStackScaleRatio, float logScale, ref Transform currentLogStackTr, Transform log, Transform parent)
		{
			if (currentLogStackRemainingScale > 0f)
			{
				currentLogStackRemainingScale -= logScale;
			}
			else
			{
				currentLogStackTr = new GameObject("ls").transform;
				currentLogStackTr.position = log.position;
				currentLogStackTr.parent = parent;
				currentLogStackRemainingScale = logStackScaleRatio - logScale;
			}
			log.parent = currentLogStackTr;
		}

		private GameObject SpawnEdge(FoundationArchitect.Edge edge, int edgeNum)
		{
			GameObject gameObject = new GameObject("Edge" + edgeNum);
			Transform transform = gameObject.transform;
			gameObject.transform.position = edge._p1;
			float num = edge._hlength / (float)edge._segments.Length;
			Vector3 localScale = new Vector3(1f, 1f, num / this._logLength);
			Vector3 localScale2 = new Vector3(1f, 1f, this.CalcDiagonalLength(1f, localScale.z));
			Vector3 vector = edge._axis / 2f;
			Vector3 b = new Vector3(0f, this._logLength / 2f, 0f);
			Vector3 b2 = new Vector3(0f, this._logLength, 0f);
			Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);
			Quaternion rotation2 = Quaternion.LookRotation(edge._axis);
			Vector3 forward = vector - b;
			Quaternion rotation3 = Quaternion.LookRotation(forward);
			float logStackScaleRatio = (this._maxScaleLogCost <= 0f) ? 0f : (this._maxLogScale / this._maxScaleLogCost);
			float num2 = 0f;
			Transform transform2 = transform;
			edge._totalLogs = 0;
			int num3 = this._edges.Take(edgeNum).Sum((FoundationArchitect.Edge e) => e._segments.Length);
			string[] array = new string[]
			{
				"SLTier1",
				"SLTier2",
				"SLTier3"
			};
			float num4 = b2.y * (float)Mathf.CeilToInt(this.MaxHeight / b2.y / 3f) - 0.4f;
			for (int i = 0; i < edge._segments.Length; i++)
			{
				FoundationArchitect.HorizontalSegment horizontalSegment = edge._segments[i];
				Vector3 vector2 = Vector3.zero;
				num2 = 0f;
				Transform transform3 = new GameObject(i.ToString()).transform;
				transform3.parent = transform;
				int j = 0;
				bool flag = this._wasBuilt;
				for (int k = 0; k < horizontalSegment._segments.Length; k++)
				{
					if (this.SegmentTierValidation(num3 + i, j))
					{
						FoundationArchitect.VerticalSegment verticalSegment = horizontalSegment._segments[k];
						if (flag)
						{
							flag = false;
							transform2 = new GameObject("t").transform;
							transform2.parent = transform3;
							transform2.position = horizontalSegment._p1 - vector2 + vector - new Vector3(0f, num4 / 2f, 0f);
							transform2.rotation = rotation2;
							transform2.tag = array[j];
							BoxCollider boxCollider = transform2.gameObject.AddComponent<BoxCollider>();
							boxCollider.size = new Vector3(1.1f, num4 + 0.4f, 4.8f * localScale.z);
							FoundationChunkTier foundationChunkTier = transform2.gameObject.AddComponent<FoundationChunkTier>();
							foundationChunkTier._edgeNum = edgeNum;
							foundationChunkTier._segmentNum = i;
							foundationChunkTier._tierNum = j;
							this._chunks.Add(foundationChunkTier);
							transform2.gameObject.AddComponent<gridObjectBlocker>();
							getStructureStrength getStructureStrength = transform2.gameObject.AddComponent<getStructureStrength>();
							getStructureStrength._strength = getStructureStrength.strength.strong;
						}
						if (edge._isFirst && i == 0)
						{
							edge._totalLogs++;
							Transform transform4 = this.NewLog(edge._p1 - vector2 - b, rotation);
							if (!this._wasBuilt)
							{
								this.StackLogsByScale(ref num2, logStackScaleRatio, 1f, ref transform2, transform4, transform3);
							}
							else
							{
								transform4.parent = transform2;
							}
							transform4.localScale = Vector3.one;
							this._newPool.Push(transform4);
						}
						if (!edge._isClosing || i + 1 < edge._segments.Length)
						{
							edge._totalLogs++;
							Transform transform5 = this.NewLog(horizontalSegment._p2 - vector2 - b, rotation);
							if (!this._wasBuilt)
							{
								this.StackLogsByScale(ref num2, logStackScaleRatio, 1f, ref transform2, transform5, transform3);
							}
							else
							{
								transform5.parent = transform2;
							}
							transform5.localScale = Vector3.one;
							this._newPool.Push(transform5);
						}
						edge._totalLogs++;
						Transform transform6 = this.NewLog(horizontalSegment._p1 - vector2 + vector, rotation2);
						if (!this._wasBuilt)
						{
							this.StackLogsByScale(ref num2, logStackScaleRatio, localScale.z, ref transform2, transform6, transform3);
						}
						else
						{
							transform6.parent = transform2;
						}
						transform6.localScale = localScale;
						this._newPool.Push(transform6);
						if (verticalSegment._diag)
						{
							edge._totalLogs++;
							transform6 = this.NewLog(transform6.position - b, rotation3);
							if (!this._wasBuilt)
							{
								this.StackLogsByScale(ref num2, logStackScaleRatio, localScale.z, ref transform2, transform6, transform3);
							}
							else
							{
								transform6.parent = transform2;
							}
							transform6.localScale = localScale2;
							this._newPool.Push(transform6);
						}
					}
					vector2 += b2;
					if (this._wasBuilt && j < 2 && vector2.y > num4 * (float)(1 + j))
					{
						j++;
						flag = true;
					}
				}
				if (!flag)
				{
					j++;
				}
				while (j <= 2)
				{
					this._chunks.Add(null);
					j++;
				}
			}
			if (edge._isFirst)
			{
				this._foundationRoot.transform.position = edge._p1;
			}
			gameObject.transform.parent = this._foundationRoot.transform;
			return gameObject;
		}

		private Transform NewLog(Vector3 position, Quaternion rotation)
		{
			if (this._logPool.Count > 0)
			{
				Transform transform = this._logPool.Pop();
				transform.position = position;
				transform.rotation = rotation;
				return transform;
			}
			Transform transform2 = (Transform)UnityEngine.Object.Instantiate(this._logPrefab, position, rotation);
			if (!this._wasBuilt && !this._wasPlaced && this._mode == FoundationArchitect.Modes.Auto)
			{
				transform2.GetComponentInChildren<Renderer>().sharedMaterial = this._logMat;
			}
			return transform2;
		}

		public float GetLevel()
		{
			return this._multiPointsPositions[0].y + this.GetHeight();
		}

		public float GetHeight()
		{
			return this._logWidth * ((this._mode != FoundationArchitect.Modes.ManualSlave) ? 0.5f : 1f);
		}

		public List<Vector3> GetMultiPointsPositions()
		{
			return this._multiPointsPositions;
		}

		public int GetChunkIndex(FoundationChunkTier chunk)
		{
			for (int i = 0; i < this._chunks.Count; i++)
			{
				if (object.ReferenceEquals(this._chunks[i], chunk))
				{
					return i;
				}
			}
			return -1;
		}

		public FoundationChunkTier GetChunk(int index)
		{
			return this._chunks[index];
		}

		public StructureAnchor GetAnchor(int anchor)
		{
			if (this._mode == FoundationArchitect.Modes.Auto)
			{
				StructureAnchor[] componentsInChildren = base.gameObject.GetComponentsInChildren<StructureAnchor>();
				return componentsInChildren[anchor];
			}
			return this._anchors[anchor];
		}
	}
}
