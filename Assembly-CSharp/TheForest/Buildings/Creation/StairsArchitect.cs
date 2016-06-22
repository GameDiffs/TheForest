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
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Stairs Architect")]
	public class StairsArchitect : MonoBehaviour, ICoopStructure
	{
		[SerializeThis]
		public bool _wasPlaced;

		[SerializeThis]
		public bool _wasBuilt;

		public Transform[] _logPrefabs;

		public Transform _stiltPrefab;

		public Renderer _logRenderer;

		public float _maxEdgeLength = 12f;

		public float _closureSnappingDistance = 2.5f;

		public float _logCost = 1f;

		public float _minAngleBetweenEdges = 90f;

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

		private Transform _stairsRoot;

		private float _logLength;

		private float _logWidth;

		private float _logHeight;

		private float _maxChunkLength;

		private float _minChunkLength;

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
			StairsArchitect.<DelayedAwake>c__Iterator142 <DelayedAwake>c__Iterator = new StairsArchitect.<DelayedAwake>c__Iterator142();
			<DelayedAwake>c__Iterator.isDeserializing = isDeserializing;
			<DelayedAwake>c__Iterator.<$>isDeserializing = isDeserializing;
			<DelayedAwake>c__Iterator.<>f__this = this;
			return <DelayedAwake>c__Iterator;
		}

		private void Update()
		{
			bool flag = this._multiPointsPositions.Count > 1;
			if (LocalPlayer.Create.BuildingPlacer.Clear != flag || Scene.HudGui.PlaceWallIcon.activeSelf != flag)
			{
				Scene.HudGui.PlaceWallIcon.SetActive(flag);
				LocalPlayer.Create.BuildingPlacer.Clear = flag;
				if (base.GetComponent<Renderer>().enabled)
				{
					base.GetComponent<Renderer>().sharedMaterial = this._logMat;
				}
			}
			if (this._multiPointsPositions.Count > 0)
			{
				Vector3 vector = base.transform.position;
				Vector3 vector2 = this._multiPointsPositions.Last<Vector3>();
				if (this._multiPointsPositions.Count > 1)
				{
					if (this._multiPointsPositions[0].y < this._multiPointsPositions[1].y)
					{
						if (vector.y < vector2.y)
						{
							vector.y = vector2.y;
						}
					}
					else if (vector.y > vector2.y)
					{
						vector.y = vector2.y;
					}
				}
				if (Vector3.Distance(vector2, vector) > this._logWidth)
				{
					if (Vector3.Distance(vector2, vector) >= this._maxEdgeLength)
					{
						vector = vector2 + (vector - vector2).normalized * this._maxEdgeLength;
					}
					this._multiPointsPositions.Add(vector);
					Transform stairsRoot = this.SpawnStairs();
					this._multiPointsPositions.RemoveAt(this._multiPointsPositions.Count - 1);
					if (this._stairsRoot)
					{
						UnityEngine.Object.Destroy(this._stairsRoot.gameObject);
					}
					this._stairsRoot = stairsRoot;
				}
			}
			else if (this._stairsRoot)
			{
				base.GetComponent<Renderer>().enabled = true;
				UnityEngine.Object.Destroy(this._stairsRoot.gameObject);
				this._stairsRoot = null;
				this._logPool.Clear();
			}
			this.CheckLockPoint();
			this.CheckUnlockPoint();
		}

		private void OnDestroy()
		{
			if (this._stairsRoot)
			{
				UnityEngine.Object.Destroy(this._stairsRoot.gameObject);
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
					this._stairsRoot.transform.parent = base.transform;
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
			StairsArchitect.<OnPlaced>c__Iterator143 <OnPlaced>c__Iterator = new StairsArchitect.<OnPlaced>c__Iterator143();
			<OnPlaced>c__Iterator.<>f__this = this;
			return <OnPlaced>c__Iterator;
		}

		private void OnBuilt(GameObject built)
		{
			StairsArchitect component = built.GetComponent<StairsArchitect>();
			component._multiPointsPositions = this._multiPointsPositions;
			component._wasBuilt = true;
			component.OnSerializing();
		}

		public void CreateStructure(bool isRepair = false)
		{
			if (this._wasBuilt && isRepair)
			{
				if (this._stairsRoot)
				{
					UnityEngine.Object.Destroy(this._stairsRoot.gameObject);
				}
				this._stairsRoot = null;
				base.StartCoroutine(this.DelayedAwake(true));
			}
			this._stairsRoot = this.SpawnStairs();
			if (this._wasBuilt && isRepair)
			{
				this._stairsRoot.parent = base.transform;
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
				float num = vector2.magnitude - this._logWidth * 3f;
				vector2.y = 0f;
				flag = (magnitude > this._logLength && num * 1.1f >= Mathf.Abs(vector.y));
				flag2 = (magnitude >= this._maxEdgeLength);
			}
			if (flag && TheForest.Utils.Input.GetButtonDown("Fire1"))
			{
				Vector3 vector3 = base.transform.position;
				Vector3 vector4 = (this._multiPointsPositions.Count <= 0) ? vector3 : this._multiPointsPositions.Last<Vector3>();
				if (this._multiPointsPositions.Count > 1)
				{
					if (this._multiPointsPositions[0].y < this._multiPointsPositions[1].y)
					{
						if (vector3.y < vector4.y)
						{
							vector3.y = vector4.y;
						}
					}
					else if (vector3.y > vector4.y)
					{
						vector3.y = vector4.y;
					}
				}
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

		private Transform SpawnStairs()
		{
			Transform transform = new GameObject("StairsRoot").transform;
			transform.position = this._multiPointsPositions.First<Vector3>();
			Stack<Transform> stack = new Stack<Transform>();
			int num;
			int num2;
			if (this._multiPointsPositions[0].y < this._multiPointsPositions[1].y)
			{
				num = this._multiPointsPositions.Count - 2;
				num2 = -1;
			}
			else
			{
				num = 1;
				num2 = 1;
			}
			while (num >= 0 && num < this._multiPointsPositions.Count)
			{
				Vector3 vector = this._multiPointsPositions[num - num2];
				Vector3 a = this._multiPointsPositions[num];
				Vector3 vector2 = vector;
				Vector3 forward = a - vector;
				Vector3 rhs = new Vector3(forward.x, 0f, forward.z);
				Vector3 vector3 = Vector3.Cross(Vector3.up, rhs);
				int num3 = Mathf.CeilToInt(rhs.magnitude / this._logWidth);
				Vector3 vector4 = rhs.normalized * this._logWidth;
				Quaternion rotation = Quaternion.LookRotation(vector3);
				Transform transform2 = new GameObject("StairEdge" + num).transform;
				transform2.parent = transform;
				forward = a - (vector2 + vector4 * 2.5f);
				transform2.rotation = Quaternion.LookRotation(forward);
				transform2.position = vector2 + vector4 * 2.5f;
				Vector3 pointFloorPosition = this.GetPointFloorPosition(vector2 + Vector3.down);
				Transform transform3 = (Transform)UnityEngine.Object.Instantiate(this._stiltPrefab, vector2, Quaternion.identity);
				transform3.parent = transform2;
				transform3.localScale = new Vector3(1f, Mathf.Abs(vector2.y - pointFloorPosition.y) / 4.5f, 1f);
				if (!this._wasBuilt && !this._wasPlaced)
				{
					transform3.GetComponentInChildren<Renderer>().sharedMaterial = this._logMat;
				}
				vector2 -= vector4 * 2.5f;
				for (int i = 0; i < 5; i++)
				{
					UnityEngine.Debug.DrawLine(vector2 - vector3, vector2 + vector3, Color.cyan);
					Transform transform4 = this.NewLog(vector2, rotation);
					transform4.parent = transform2;
					stack.Push(transform4);
					vector2 += vector4;
					if (this._wasBuilt && i == 2)
					{
						transform4.rotation = rotation;
						transform4.tag = "UnderfootWood";
						BoxCollider boxCollider = transform4.gameObject.AddComponent<BoxCollider>();
						boxCollider.size = new Vector3(this._logWidth * 5.5f, this._logHeight, this._logLength);
						boxCollider.center = new Vector3(-0.42f, -0.34f, 0f);
						transform4.gameObject.AddComponent<gridObjectBlocker>();
					}
				}
				num3 -= 3;
				Transform transform5 = UnityEngine.Object.Instantiate<Transform>(this._stiltPrefab);
				transform5.parent = transform2;
				transform5.localPosition = new Vector3(0f, -0.75f, 0f);
				transform5.localEulerAngles = new Vector3(-90f, 0f, 0f);
				transform5.localScale = new Vector3(1f, Mathf.Abs(forward.magnitude) / 4.68f, 1f);
				if (!this._wasBuilt && !this._wasPlaced)
				{
					transform5.GetComponentInChildren<Renderer>().sharedMaterial = this._logMat;
				}
				Vector3 b = new Vector3(0f, forward.y / (float)num3, 0f);
				for (int j = 0; j < num3; j++)
				{
					UnityEngine.Debug.DrawLine(vector2 - vector3, vector2 + vector3, Color.cyan);
					Transform transform6 = this.NewLog(vector2, rotation);
					transform6.parent = transform2;
					stack.Push(transform6);
					vector2 += vector4;
					vector2 += b;
				}
				if (this._wasBuilt)
				{
					BoxCollider boxCollider2 = transform2.gameObject.AddComponent<BoxCollider>();
					boxCollider2.size = new Vector3(this._logLength, this._logHeight, forward.magnitude);
					boxCollider2.center = new Vector3(0f, -0.34f, boxCollider2.size.z / 2f);
					transform2.tag = "UnderfootWood";
					transform2.gameObject.layer = 21;
					transform2.gameObject.AddComponent<gridObjectBlocker>();
					FoundationArchitect.CreateWindSFX(transform5);
				}
				num += num2;
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
			Transform transform2 = (Transform)UnityEngine.Object.Instantiate(this._logPrefabs[UnityEngine.Random.Range(0, this._logPrefabs.Length)], position, rotation);
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
	}
}
