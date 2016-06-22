using Bolt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Buildings.Interfaces;
using TheForest.Buildings.Utils;
using TheForest.Buildings.World;
using TheForest.Items;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Bridge Architect")]
	public class BridgeArchitect : MonoBehaviour, IEntityReplicationFilter, IAnchorableStructure, IAnchorableStructureX2, ICoopStructure
	{
		[SerializeThis]
		public bool _wasPlaced;

		[SerializeThis]
		public bool _wasBuilt;

		public Transform _logPrefab;

		public Renderer _logRenderer;

		public float _verticalBend = 0.003f;

		public float _maxColliderLength = 10f;

		[Range(0f, 1f)]
		public float _logCost = 0.5f;

		public Craft_Structure _craftStructure;

		[ItemIdPicker(Item.Types.Equipment)]
		public int _logItemId;

		public GameObject _logTextPrefab;

		public GameObject _logIconPrefab;

		private bool _initialized;

		[SerializeThis]
		private StructureAnchor _anchor1;

		[SerializeThis]
		private StructureAnchor _anchor2;

		[SerializeThis]
		private long _anchor1Hash;

		[SerializeThis]
		private long _anchor2Hash;

		private Vector3 _placerOffset;

		private StructureAnchor _tmpAnchor;

		private Transform _bridgeRoot;

		private float _logWidth;

		private Stack<Transform> _logPool;

		private Material _logMat;

		public StructureAnchor Anchor1
		{
			get
			{
				return this._anchor1;
			}
			set
			{
				this._anchor1 = value;
			}
		}

		public StructureAnchor Anchor2
		{
			get
			{
				return this._anchor2;
			}
			set
			{
				this._anchor2 = value;
			}
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
				return 0;
			}
			set
			{
			}
		}

		public List<Vector3> MultiPointsPositions
		{
			get
			{
				return new List<Vector3>();
			}
			set
			{
			}
		}

		public IProtocolToken CustomToken
		{
			get
			{
				CoopSteamServerStarter.AttachBuildingBoltEntity(this.Anchor1.GetComponentInParent(typeof(ICoopAnchorStructure)).GetComponent<BoltEntity>());
				CoopSteamServerStarter.AttachBuildingBoltEntity(this.Anchor2.GetComponentInParent(typeof(ICoopAnchorStructure)).GetComponent<BoltEntity>());
				CoopBridgeToken coopBridgeToken = new CoopBridgeToken();
				coopBridgeToken.Anchors[0].Entity = this.Anchor1.GetComponentInParent(typeof(ICoopAnchorStructure)).GetComponent<BoltEntity>();
				coopBridgeToken.Anchors[0].Index = (this.Anchor1.GetComponentInParent(typeof(ICoopAnchorStructure)) as ICoopAnchorStructure).GetAnchorIndex(this.Anchor1);
				coopBridgeToken.Anchors[1].Entity = this.Anchor2.GetComponentInParent(typeof(ICoopAnchorStructure)).GetComponent<BoltEntity>();
				coopBridgeToken.Anchors[1].Index = (this.Anchor2.GetComponentInParent(typeof(ICoopAnchorStructure)) as ICoopAnchorStructure).GetAnchorIndex(this.Anchor2);
				return coopBridgeToken;
			}
			set
			{
				CoopBridgeToken coopBridgeToken = (CoopBridgeToken)value;
				this.Anchor1 = (coopBridgeToken.Anchors[0].Entity.GetComponent(typeof(ICoopAnchorStructure)) as ICoopAnchorStructure).GetAnchor(coopBridgeToken.Anchors[0].Index);
				this.Anchor2 = (coopBridgeToken.Anchors[1].Entity.GetComponent(typeof(ICoopAnchorStructure)) as ICoopAnchorStructure).GetAnchor(coopBridgeToken.Anchors[1].Index);
			}
		}

		void IAnchorableStructure.AnchorDestroyed(StructureAnchor anchor)
		{
			BuildingHealth component = base.GetComponent<BuildingHealth>();
			if (component)
			{
				component.Collapse(anchor.transform.position);
			}
			else if (base.gameObject)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		bool IEntityReplicationFilter.AllowReplicationTo(BoltConnection connection)
		{
			if (this.Anchor1 && this.Anchor2)
			{
				BoltEntity component = this.Anchor1.GetComponentInParent(typeof(ICoopAnchorStructure)).GetComponent<BoltEntity>();
				BoltEntity component2 = this.Anchor2.GetComponentInParent(typeof(ICoopAnchorStructure)).GetComponent<BoltEntity>();
				return component.isAttached && component2.isAttached && connection.ExistsOnRemote(component) == ExistsResult.Yes && connection.ExistsOnRemote(component2) == ExistsResult.Yes;
			}
			return false;
		}

		private void Awake()
		{
			base.StartCoroutine(this.DelayedAwake(LevelSerializer.IsDeserializing));
			base.enabled = false;
		}

		[DebuggerHidden]
		private IEnumerator DelayedAwake(bool isDeserializing)
		{
			BridgeArchitect.<DelayedAwake>c__Iterator12D <DelayedAwake>c__Iterator12D = new BridgeArchitect.<DelayedAwake>c__Iterator12D();
			<DelayedAwake>c__Iterator12D.isDeserializing = isDeserializing;
			<DelayedAwake>c__Iterator12D.<$>isDeserializing = isDeserializing;
			<DelayedAwake>c__Iterator12D.<>f__this = this;
			return <DelayedAwake>c__Iterator12D;
		}

		private void Update()
		{
			bool flag = this._anchor1 && this._anchor2;
			if (LocalPlayer.Create.BuildingPlacer.Clear != flag || Scene.HudGui.PlaceWallIcon.activeSelf != flag)
			{
				Scene.HudGui.PlaceWallIcon.SetActive(flag);
				LocalPlayer.Create.BuildingPlacer.Clear = flag;
				if (base.GetComponent<Renderer>().enabled)
				{
					base.GetComponent<Renderer>().sharedMaterial = this._logMat;
				}
			}
			this._logMat.SetColor("_TintColor", (!flag && !this._tmpAnchor) ? LocalPlayer.Create.BuildingPlacer.RedMat.GetColor("_TintColor") : LocalPlayer.Create.BuildingPlacer.ClearMat.GetColor("_TintColor"));
			if (!base.transform.parent && !this._anchor2 && Vector3.Distance(LocalPlayer.Create.BuildingPlacer.transform.position, base.transform.position) > 3.75f)
			{
				this.RevertFirstAnchorSnapping();
			}
			if (this._anchor1)
			{
				if (!this._anchor2)
				{
					Vector3 vector = (!this._tmpAnchor) ? base.transform.position : this._tmpAnchor.transform.position;
					if (Vector3.Distance(this._anchor1.transform.position, vector) > this._logWidth)
					{
						if (base.GetComponent<Renderer>().enabled)
						{
							base.GetComponent<Renderer>().enabled = false;
						}
						Transform bridgeRoot = this.CreateBridge(this._anchor1.transform.position, vector);
						if (this._bridgeRoot)
						{
							UnityEngine.Object.Destroy(this._bridgeRoot.gameObject);
						}
						this._bridgeRoot = bridgeRoot;
					}
					else if (!base.GetComponent<Renderer>().enabled)
					{
						base.GetComponent<Renderer>().enabled = true;
					}
				}
			}
			else if (this._bridgeRoot)
			{
				UnityEngine.Object.Destroy(this._bridgeRoot.gameObject);
				this._bridgeRoot = null;
				this._logPool.Clear();
			}
			this.CheckLockAnchor();
			this.CheckUnlockAnchor();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (LocalPlayer.Create.BuildingPlacer && !this._wasPlaced && !this._wasBuilt)
			{
				StructureAnchor component = other.GetComponent<StructureAnchor>();
				if (component && component != this._anchor1 && component != this._anchor2 && this._tmpAnchor != component && component._hookedInStructure == null && (!this._anchor1 || !this._anchor1.transform.root.Equals(other.transform.root)))
				{
					if (base.transform.parent == null)
					{
						this.RevertFirstAnchorSnapping();
					}
					if (base.GetComponent<Renderer>())
					{
						base.GetComponent<Renderer>().sharedMaterial = LocalPlayer.Create.BuildingPlacer.ClearMat;
					}
					this._placerOffset = base.transform.localPosition;
					base.transform.parent = null;
					base.transform.position = other.transform.position;
					base.transform.rotation = other.transform.rotation;
					Scene.HudGui.LockPositionIcon.SetActive(true);
					this._tmpAnchor = component;
					this._tmpAnchor._hookedInStructure = this;
				}
			}
		}

		private void OnDestroy()
		{
			if (this._bridgeRoot)
			{
				UnityEngine.Object.Destroy(this._bridgeRoot.gameObject);
			}
			if (!this._wasPlaced && LocalPlayer.Create)
			{
				LocalPlayer.Create.Grabber.ClosePlace();
				if (Scene.HudGui)
				{
					Scene.HudGui.LockPositionIcon.SetActive(false);
					Scene.HudGui.UnlockPositionIcon.SetActive(false);
				}
			}
			if (this._tmpAnchor && this._tmpAnchor._hookedInStructure == this)
			{
				this._tmpAnchor._hookedInStructure = null;
			}
			if (this._anchor1 && this._anchor1._hookedInStructure == this)
			{
				this._anchor1._hookedInStructure = null;
			}
			if (this._anchor2 && this._anchor2._hookedInStructure == this)
			{
				this._anchor2._hookedInStructure = null;
			}
		}

		private void OnSerializing()
		{
			if (this._anchor1Hash == 0L)
			{
				this._anchor1Hash = this._anchor1.ToHash();
			}
			if (this._anchor2Hash == 0L)
			{
				this._anchor2Hash = this._anchor2.ToHash();
			}
		}

		private void OnDeserialized()
		{
			if (!this._initialized)
			{
				this._initialized = true;
				if (!this._anchor1)
				{
					this._anchor1 = BridgeAnchorHelper.GetAnchorFromHash(this._anchor1Hash);
				}
				if (!this._anchor2)
				{
					this._anchor2 = BridgeAnchorHelper.GetAnchorFromHash(this._anchor2Hash);
				}
				if (!this._anchor1 || !this._anchor2)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
				else
				{
					this._anchor1._hookedInStructure = this;
					this._anchor2._hookedInStructure = this;
					if (this._wasBuilt)
					{
						this.CreateStructure(false, true);
						FoundationArchitect.CreateWindSFX(base.transform);
						this._logPool = null;
					}
					else if (this._wasPlaced)
					{
						this.CreateStructure(false, false);
						this._logPool = new Stack<Transform>();
						base.StartCoroutine(this.OnPlaced(true));
					}
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator OnPlaced(bool readyMp = false)
		{
			BridgeArchitect.<OnPlaced>c__Iterator12E <OnPlaced>c__Iterator12E = new BridgeArchitect.<OnPlaced>c__Iterator12E();
			<OnPlaced>c__Iterator12E.<>f__this = this;
			return <OnPlaced>c__Iterator12E;
		}

		private void OnBuilt(GameObject built)
		{
			BridgeArchitect component = built.GetComponent<BridgeArchitect>();
			component._anchor1 = this._anchor1;
			component._anchor1._hookedInStructure = component;
			component._anchor2 = this._anchor2;
			component._anchor2._hookedInStructure = component;
			component._wasBuilt = true;
		}

		public void CreateStructure(bool isRepair = false, bool addColliders = true)
		{
			if (this._wasBuilt && isRepair)
			{
				if (this._bridgeRoot)
				{
					UnityEngine.Object.Destroy(this._bridgeRoot.gameObject);
				}
				this._bridgeRoot = null;
				base.StartCoroutine(this.DelayedAwake(true));
			}
			GameObject gameObject = (!this._bridgeRoot) ? null : this._bridgeRoot.gameObject;
			this._bridgeRoot = this.CreateBridge(this._anchor1.transform.position, this._anchor2.transform.position);
			this._bridgeRoot.name = "BridgeRoot" + ((!addColliders) ? "Ghost" : "Built");
			this._bridgeRoot.parent = base.transform;
			if (gameObject)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			if (addColliders)
			{
				base.transform.position = this._anchor1.transform.position;
				this._bridgeRoot.parent = base.transform;
				Vector3 vector = this._bridgeRoot.GetChild(0).position;
				int num = Mathf.CeilToInt(Vector3.Distance(this._anchor1.transform.position, this._anchor2.transform.position) / this._maxColliderLength);
				int num2 = Mathf.CeilToInt((float)this._bridgeRoot.childCount / (float)num);
				for (int i = 1; i <= num; i++)
				{
					int num3 = num2 * i;
					if (num3 >= this._bridgeRoot.childCount)
					{
						num3 = this._bridgeRoot.childCount - 1;
					}
					Transform child = this._bridgeRoot.GetChild(num3);
					Vector3 position = child.position;
					Transform transform = new GameObject("Floor" + i).transform;
					transform.parent = base.transform;
					transform.position = vector;
					transform.LookAt(position);
					BoxCollider boxCollider = transform.gameObject.AddComponent<BoxCollider>();
					boxCollider.center = transform.InverseTransformPoint(Vector3.Lerp(vector, position, 0.5f));
					boxCollider.size = new Vector3(4.5f, this._logWidth, Vector3.Distance(vector, position));
					transform.tag = "UnderfootWood";
					vector = position;
				}
			}
			if (this._wasBuilt && isRepair)
			{
				this._bridgeRoot.parent = base.transform;
			}
		}

		private void RevertFirstAnchorSnapping()
		{
			if (LocalPlayer.Create.BuildingPlacer)
			{
				base.GetComponent<Renderer>().material = LocalPlayer.Create.BuildingPlacer.RedMat;
				base.transform.parent = LocalPlayer.Create.BuildingPlacer.transform;
				base.transform.localPosition = this._placerOffset;
				base.transform.localRotation = Quaternion.identity;
				Scene.HudGui.LockPositionIcon.SetActive(false);
				if (this._tmpAnchor)
				{
					this._tmpAnchor._hookedInStructure = null;
				}
				this._tmpAnchor = null;
			}
		}

		private void CheckLockAnchor()
		{
			if (this._tmpAnchor != null && TheForest.Utils.Input.GetButtonDown("Fire1"))
			{
				if (!this._anchor1)
				{
					this._anchor1 = this._tmpAnchor;
					this._anchor1._hookedInStructure = this;
					Scene.HudGui.UnlockPositionIcon.SetActive(true);
				}
				else if (!this._anchor2)
				{
					this._anchor2 = this._tmpAnchor;
					this._anchor2._hookedInStructure = this;
					if (BoltNetwork.isRunning)
					{
						base.transform.position = this._anchor1.transform.position;
						base.transform.LookAt(this._anchor2.transform.position);
					}
					if (this._bridgeRoot)
					{
						UnityEngine.Object.Destroy(this._bridgeRoot.gameObject);
					}
					this._bridgeRoot = this.CreateBridge(this._anchor1.transform.position, this._anchor2.transform.position);
				}
				Scene.HudGui.LockPositionIcon.SetActive(false);
				this._tmpAnchor = null;
			}
		}

		private void CheckUnlockAnchor()
		{
			if (TheForest.Utils.Input.GetButtonDown("AltFire"))
			{
				if (this._anchor2)
				{
					base.GetComponent<Collider>().enabled = false;
					base.GetComponent<Collider>().enabled = true;
					this._anchor2._hookedInStructure = null;
					this._anchor2 = null;
				}
				else if (this._anchor1)
				{
					base.GetComponent<Collider>().enabled = false;
					base.GetComponent<Collider>().enabled = true;
					base.GetComponent<Renderer>().enabled = true;
					base.GetComponent<Renderer>().material = LocalPlayer.Create.BuildingPlacer.RedMat;
					Scene.HudGui.UnlockPositionIcon.SetActive(false);
					this._anchor1._hookedInStructure = null;
					this._anchor1 = null;
				}
			}
		}

		private Transform CreateBridge(Vector3 p1, Vector3 p2)
		{
			if (p1.y > p2.y)
			{
				Vector3 vector = p1;
				p1 = p2;
				p2 = vector;
			}
			Transform transform = new GameObject("BridgeRoot").transform;
			transform.position = p1;
			Quaternion quaternion = Quaternion.LookRotation(p2 - p1);
			float num = Vector3.Distance(p1, p2) / 2f;
			Stack<Transform> stack = new Stack<Transform>();
			float num2 = this._logWidth / 3f;
			float num3 = 1f;
			Transform parent = null;
			float num4;
			while ((num4 = Vector3.Distance(p1, p2)) > num2)
			{
				float num5 = num4 * this._verticalBend;
				Transform transform2;
				if (!this._wasBuilt)
				{
					transform2 = this.NewLog(p1, quaternion);
					num3 += this._logCost;
					if (num3 >= 1f)
					{
						num3 -= 1f;
						parent = transform2;
						transform2.parent = transform;
					}
					else
					{
						transform2.parent = parent;
					}
				}
				else
				{
					transform2 = this.NewLog(p1, quaternion * Quaternion.Euler((float)UnityEngine.Random.Range(0, 359), (float)UnityEngine.Random.Range(-3, 3), (float)UnityEngine.Random.Range(-3, 3)));
					transform2.parent = transform;
				}
				stack.Push(transform2);
				p1 = Vector3.MoveTowards(p1, p2, this._logWidth - num5);
				p1.y -= num5;
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
			return transform2;
		}
	}
}
