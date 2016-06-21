using Bolt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Items;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Dock Architect")]
	public class DockArchitect : MonoBehaviour, ICoopStructure, TriggerTagSensor.ITarget
	{
		[SerializeThis]
		public bool _wasPlaced;

		[SerializeThis]
		public bool _wasBuilt;

		public Transform _logPrefab;

		public Transform _stiltPrefab;

		public Renderer _logRenderer;

		public float _maxEdgeLength = 12f;

		public float _closureSnappingDistance = 2.5f;

		public float _logCost = 1f;

		public float _maxAngleBetweenEdges = 60f;

		public int _maxPoints = 50;

		public LayerMask _floorLayers;

		public Craft_Structure _craftStructure;

		[ItemIdPicker(Item.Types.Equipment)]
		public int _logItemId;

		private bool _initialized;

		[SerializeThis]
		private List<Vector3> _multiPointsPositions;

		[SerializeThis]
		private int _multiPointsPositionsCount;

		private Vector3 _placerOffset;

		private Transform _dockRoot;

		private float _logLength;

		private float _logWidth;

		private float _logHeight;

		private float _maxChunkLength;

		private float _minChunkLength;

		private float _waterLevel = 3.40282347E+38f;

		private Stack<Transform> _logPool;

		private Stack<Transform> _newPool;

		private Material _logMat;

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

		public IProtocolToken CustomToken
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		private void Awake()
		{
			base.StartCoroutine(this.DelayedAwake(LevelSerializer.IsDeserializing));
			base.enabled = false;
		}

		[DebuggerHidden]
		private IEnumerator DelayedAwake(bool isDeserializing)
		{
			DockArchitect.<DelayedAwake>c__Iterator131 <DelayedAwake>c__Iterator = new DockArchitect.<DelayedAwake>c__Iterator131();
			<DelayedAwake>c__Iterator.isDeserializing = isDeserializing;
			<DelayedAwake>c__Iterator.<$>isDeserializing = isDeserializing;
			<DelayedAwake>c__Iterator.<>f__this = this;
			return <DelayedAwake>c__Iterator;
		}

		private void Update()
		{
			bool flag = this._multiPointsPositions.Count > 1;
			if (LocalPlayer.Create.BuildingPlacer.Clear != flag)
			{
				Scene.HudGui.PlaceWallIcon.SetActive(flag);
				LocalPlayer.Create.BuildingPlacer.Clear = flag;
				if (base.GetComponent<Renderer>().enabled)
				{
					base.GetComponent<Renderer>().sharedMaterial = this._logMat;
				}
			}
			this._logMat.SetColor("_TintColor", (this._waterLevel != 3.40282347E+38f) ? LocalPlayer.Create.BuildingPlacer.ClearMat.GetColor("_TintColor") : LocalPlayer.Create.BuildingPlacer.RedMat.GetColor("_TintColor"));
			if (this._waterLevel == 3.40282347E+38f)
			{
				return;
			}
			if (this._multiPointsPositions.Count > 0)
			{
				Vector3 vector = base.transform.position;
				Vector3 vector2 = this._multiPointsPositions.Last<Vector3>();
				if (this._multiPointsPositions.Count == 1 && Vector3.Distance(vector2, vector) < this._logWidth)
				{
					vector.y = vector2.y;
					Vector3 vector3 = vector - vector2;
					if (vector3.sqrMagnitude == 0f)
					{
						vector3 = Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f) * Vector3.forward;
					}
					vector = vector2 + vector3.normalized * (this._logWidth * 0.75f);
				}
				vector.y = vector2.y;
				if (Vector3.Distance(vector2, vector) >= this._logWidth * 0.5f)
				{
					if (Vector3.Distance(vector2, vector) >= this._maxEdgeLength)
					{
						vector = vector2 + (vector - vector2).normalized * this._maxEdgeLength;
					}
					this._multiPointsPositions.Add(vector);
					Transform dockRoot = this.SpawnDock();
					this._multiPointsPositions.RemoveAt(this._multiPointsPositions.Count - 1);
					if (this._dockRoot)
					{
						UnityEngine.Object.Destroy(this._dockRoot.gameObject);
					}
					this._dockRoot = dockRoot;
				}
			}
			else
			{
				if (this._dockRoot)
				{
					base.GetComponent<Renderer>().enabled = true;
					UnityEngine.Object.Destroy(this._dockRoot.gameObject);
					this._dockRoot = null;
					this._logPool.Clear();
				}
				if (base.transform.parent.position.y < this._waterLevel + 2f)
				{
					Vector3 position = base.transform.parent.position;
					position.y = this._waterLevel + 2f;
					base.transform.position = position;
					base.transform.rotation = Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f);
				}
			}
			this.CheckLockPoint();
			this.CheckUnlockPoint();
		}

		private void OnDestroy()
		{
			if (this._dockRoot)
			{
				UnityEngine.Object.Destroy(this._dockRoot.gameObject);
			}
			if (Scene.HudGui)
			{
				Scene.HudGui.LockPositionIcon.SetActive(false);
				Scene.HudGui.UnlockPositionIcon.SetActive(false);
			}
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
					this.CreateStructure(false);
					this._dockRoot.transform.parent = base.transform;
					this._logPool = null;
					this._newPool = null;
				}
				else if (this._wasPlaced)
				{
					base.StartCoroutine(this.OnPlaced());
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator OnPlaced()
		{
			DockArchitect.<OnPlaced>c__Iterator132 <OnPlaced>c__Iterator = new DockArchitect.<OnPlaced>c__Iterator132();
			<OnPlaced>c__Iterator.<>f__this = this;
			return <OnPlaced>c__Iterator;
		}

		private void OnBuilt(GameObject built)
		{
			DockArchitect component = built.GetComponent<DockArchitect>();
			component._multiPointsPositions = this._multiPointsPositions;
			component._wasBuilt = true;
			component.OnSerializing();
		}

		public void CreateStructure(bool isRepair = false)
		{
			if (this._wasBuilt && isRepair)
			{
				if (this._dockRoot)
				{
					UnityEngine.Object.Destroy(this._dockRoot.gameObject);
				}
				this._dockRoot = null;
				base.StartCoroutine(this.DelayedAwake(true));
			}
			this._dockRoot = this.SpawnDock();
			if (this._wasBuilt && isRepair)
			{
				this._dockRoot.parent = base.transform;
			}
		}

		private void CheckLockPoint()
		{
			bool flag = this._multiPointsPositions.Count == 0;
			bool flag2 = false;
			if (!flag)
			{
				Vector3 vector = base.transform.position - this._multiPointsPositions.Last<Vector3>();
				Vector3 vector2 = vector;
				vector2.y = 0f;
				float magnitude = vector.magnitude;
				vector2.y = 0f;
				flag = (magnitude > this._logLength);
				if (flag && this._multiPointsPositions.Count > 1)
				{
					Vector3 from = this._multiPointsPositions.Last<Vector3>() - this._multiPointsPositions[this._multiPointsPositions.Count - 2];
					float num = Vector3.Angle(from, vector);
					flag = (num < this._maxAngleBetweenEdges);
				}
				flag2 = (magnitude >= this._maxEdgeLength);
			}
			if (flag && TheForest.Utils.Input.GetButtonDown("Fire1"))
			{
				Vector3 vector3 = base.transform.position;
				vector3.y = this._waterLevel + 2f;
				Vector3 vector4 = (this._multiPointsPositions.Count <= 0) ? vector3 : this._multiPointsPositions.Last<Vector3>();
				if (flag2)
				{
					vector3 = vector4 + (vector3 - vector4).normalized * this._maxEdgeLength;
				}
				if (base.GetComponent<Renderer>().enabled)
				{
					base.GetComponent<Renderer>().enabled = false;
				}
				Scene.HudGui.UnlockPositionIcon.SetActive(true);
				this._multiPointsPositions.Add(vector3);
			}
			Scene.HudGui.LockPositionIcon.SetActive(flag);
		}

		private void CheckUnlockPoint()
		{
			if (TheForest.Utils.Input.GetButtonDown("AltFire") && this._multiPointsPositions.Count > 0)
			{
				this._multiPointsPositions.RemoveAt(this._multiPointsPositions.Count - 1);
				if (this._multiPointsPositions.Count == 0)
				{
					base.GetComponent<Renderer>().enabled = true;
				}
				Scene.HudGui.UnlockPositionIcon.SetActive(false);
			}
		}

		private Vector3 GetPointFloorPosition(Vector3 point)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(point, Vector3.down, out raycastHit, 150f, Scene.ValidateFloorLayers(point, this._floorLayers.value)))
			{
				return raycastHit.point;
			}
			point.y -= this._logLength / 2f;
			return point;
		}

		private Transform SpawnDock()
		{
			Transform transform = new GameObject("DockRoot").transform;
			transform.position = this._multiPointsPositions.First<Vector3>();
			Stack<Transform> stack = new Stack<Transform>();
			for (int i = 1; i < this._multiPointsPositions.Count; i++)
			{
				Vector3 vector = this._multiPointsPositions[i - 1];
				Vector3 a = this._multiPointsPositions[i];
				Vector3 vector2 = vector;
				Vector3 forward = a - vector;
				Vector3 rhs = new Vector3(forward.x, 0f, forward.z);
				Vector3 forward2 = Vector3.Cross(Vector3.up, rhs);
				int num = Mathf.CeilToInt(rhs.magnitude / this._logWidth);
				Vector3 b = rhs.normalized * this._logWidth;
				Quaternion rotation = Quaternion.LookRotation(forward2);
				if (i % 2 == 1)
				{
					vector2.y += 0.2f;
				}
				Transform transform2 = new GameObject("DockEdge" + i).transform;
				transform2.parent = transform;
				transform2.rotation = Quaternion.LookRotation(forward);
				transform2.position = vector2;
				Vector3 b2 = transform2.right * (this._logLength / 2f + this._logWidth * 0.3f);
				for (int j = 0; j < num; j++)
				{
					Transform transform3 = this.NewLog(vector2, rotation);
					transform3.parent = transform2;
					stack.Push(transform3);
					bool flag = j + 1 == num;
					if (j % 4 == 0 || flag)
					{
						bool flag2 = false;
						bool flag3 = false;
						if (flag && i + 1 < this._multiPointsPositions.Count)
						{
							if (Vector3.Cross(this._multiPointsPositions[i - 1] - this._multiPointsPositions[i], this._multiPointsPositions[i + 1] - this._multiPointsPositions[i]).y > 0f)
							{
								flag2 = true;
							}
							else
							{
								flag3 = true;
							}
							Transform transform4 = UnityEngine.Object.Instantiate<Transform>(this._stiltPrefab);
							transform4.parent = transform2;
							Vector3 normalized = ((this._multiPointsPositions[i - 1] - this._multiPointsPositions[i]).normalized + (this._multiPointsPositions[i + 1] - this._multiPointsPositions[i]).normalized).normalized;
							transform4.position = vector2 + normalized * (this._logLength / 2f + this._logWidth * 0.3f) + Vector3.up;
							transform4.localScale = new Vector3(1f, (Mathf.Abs(vector2.y - this.GetPointFloorPosition(vector2).y) + 1f) / 4.5f, 1f);
							if (!this._wasBuilt && !this._wasPlaced)
							{
								transform4.GetComponentInChildren<Renderer>().sharedMaterial = this._logMat;
							}
						}
						if (i > 1 && j == 0)
						{
							if (Vector3.Cross(this._multiPointsPositions[i] - this._multiPointsPositions[i - 1], this._multiPointsPositions[i - 2] - this._multiPointsPositions[i - 1]).y < 0f)
							{
								flag2 = true;
							}
							else
							{
								flag3 = true;
							}
						}
						if (!flag3)
						{
							Transform transform4 = UnityEngine.Object.Instantiate<Transform>(this._stiltPrefab);
							transform4.parent = transform2;
							transform4.position = vector2 + b2 + Vector3.up;
							transform4.localScale = new Vector3(1f, (Mathf.Abs(vector2.y - this.GetPointFloorPosition(vector2).y) + 1f) / 4.5f, 1f);
							if (!this._wasBuilt && !this._wasPlaced)
							{
								transform4.GetComponentInChildren<Renderer>().sharedMaterial = this._logMat;
							}
						}
						if (!flag2)
						{
							Transform transform4 = UnityEngine.Object.Instantiate<Transform>(this._stiltPrefab);
							transform4.parent = transform2;
							transform4.position = vector2 - b2 + Vector3.up;
							transform4.localScale = new Vector3(1f, (Mathf.Abs(vector2.y - this.GetPointFloorPosition(vector2).y) + 1f) / 4.5f, 1f);
							if (!this._wasBuilt && !this._wasPlaced)
							{
								transform4.GetComponentInChildren<Renderer>().sharedMaterial = this._logMat;
							}
						}
					}
					vector2 += b;
				}
				if (this._wasBuilt)
				{
					BoxCollider boxCollider = transform2.gameObject.AddComponent<BoxCollider>();
					boxCollider.size = new Vector3(this._logLength, this._logHeight, forward.magnitude);
					boxCollider.center = new Vector3(0f, -0.34f, boxCollider.size.z / 2f);
					transform2.tag = "UnderfootWood";
					transform2.gameObject.layer = 21;
					transform2.gameObject.AddComponent<gridObjectBlocker>();
				}
			}
			if (!this._wasPlaced && !this._wasBuilt)
			{
				this._logPool = stack;
			}
			return transform;
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
			if (!this._wasBuilt && !this._wasPlaced)
			{
				transform2.GetComponentInChildren<Renderer>().sharedMaterial = this._logMat;
			}
			else if (this._wasBuilt)
			{
				transform2.rotation *= this.RandomLogRotation();
			}
			return transform2;
		}

		private Quaternion RandomLogRotation()
		{
			return Quaternion.Euler((float)UnityEngine.Random.Range(-3, 3), (float)UnityEngine.Random.Range(-2, 2), (float)UnityEngine.Random.Range(-5, 5));
		}

		public void OnTargetTagTrigerEnter(Collider other)
		{
			this._waterLevel = other.bounds.max.y;
			this._logMat.SetColor("_TintColor", LocalPlayer.Create.BuildingPlacer.ClearMat.GetColor("_TintColor"));
		}

		public void OnTargetTagTrigerExit(Collider other)
		{
			this._waterLevel = 3.40282347E+38f;
			this._logMat.SetColor("_TintColor", LocalPlayer.Create.BuildingPlacer.RedMat.GetColor("_TintColor"));
			Scene.HudGui.LockPositionIcon.SetActive(false);
			Scene.HudGui.UnlockPositionIcon.SetActive(false);
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
		}
	}
}
