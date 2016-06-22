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
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Wall Chunk Architect")]
	public class WallChunkArchitect : EntityBehaviour, IEntityReplicationFilter, IStructureSupport, ICoopStructure
	{
		public enum Additions
		{
			Destroyed = -2,
			Wall,
			Window,
			Door1,
			Door2,
			LockedDoor1,
			LockedDoor2
		}

		public Transform _logPrefab;

		public Renderer _logRenderer;

		public float _doorAdditionMaxSlope = 0.2f;

		public Craft_Structure _craftStructure;

		[ItemIdPicker(Item.Types.Equipment)]
		public int _logItemId;

		public GameObject _logTextPrefab;

		public GameObject _logIconPrefab;

		public bool _forceWasBuilt;

		public WallArchitect _architect;

		protected bool _initialized;

		[SerializeThis]
		protected int _multiPointsPositionsCount;

		[SerializeThis]
		protected List<Vector3> _multiPointsPositions;

		[SerializeThis]
		protected bool _wasBuilt;

		[SerializeThis]
		protected Vector3 _p1;

		[SerializeThis]
		protected Vector3 _p2;

		[SerializeThis]
		protected WallChunkArchitect.Additions _addition = WallChunkArchitect.Additions.Wall;

		protected Transform _wallRoot;

		protected Transform _wallCollision;

		protected WallChunkLods _lods;

		protected float _logLength;

		protected float _logWidth;

		public virtual Transform BuiltLogPrefab
		{
			get
			{
				return Prefabs.Instance.LogWallExBuiltPrefab;
			}
		}

		public WallChunkArchitect.Additions Addition
		{
			get
			{
				return this._addition;
			}
			set
			{
				this._addition = value;
			}
		}

		public List<Vector3> MultipointPositions
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

		public Vector3 P1
		{
			get
			{
				return this._p1;
			}
			set
			{
				this._p1 = value;
			}
		}

		public Vector3 P2
		{
			get
			{
				return this._p2;
			}
			set
			{
				this._p2 = value;
			}
		}

		public float LogWidth
		{
			get
			{
				return this._logWidth;
			}
		}

		[SerializeThis]
		public IStructureSupport CurrentSupport
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
			get;
			set;
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
				return new List<Vector3>
				{
					this._p1,
					this._p2
				};
			}
			set
			{
				this._p1 = value[0];
				this._p2 = value[1];
			}
		}

		public IProtocolToken CustomToken
		{
			get
			{
				if (BoltNetwork.isServer && this.entity.isAttached)
				{
					if (this.entity.StateIs<IWallChunkBuildingState>())
					{
						this.entity.GetState<IWallChunkBuildingState>().Addition = (int)this._addition;
					}
					else
					{
						this.entity.GetState<IWallChunkConstructionState>().Addition = (int)this._addition;
					}
				}
				CoopWallChunkToken coopWallChunkToken = new CoopWallChunkToken
				{
					P1 = this._p1,
					P2 = this._p2,
					Additions = this._addition
				};
				coopWallChunkToken.PointsPositions = this._multiPointsPositions.ToArray();
				if (this.CurrentSupport != null)
				{
					coopWallChunkToken.Support = (this.CurrentSupport as MonoBehaviour).GetComponent<BoltEntity>();
					CoopSteamServerStarter.AttachBuildingBoltEntity(coopWallChunkToken.Support);
				}
				return coopWallChunkToken;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		bool IEntityReplicationFilter.AllowReplicationTo(BoltConnection connection)
		{
			return this.CurrentSupport == null || connection.ExistsOnRemote((this.CurrentSupport as MonoBehaviour).GetComponent<BoltEntity>()) == ExistsResult.Yes;
		}

		protected virtual void Awake()
		{
			base.StartCoroutine(this.DelayedAwake(LevelSerializer.IsDeserializing));
			base.enabled = false;
		}

		[DebuggerHidden]
		protected IEnumerator DelayedAwake(bool isDeserializing)
		{
			WallChunkArchitect.<DelayedAwake>c__Iterator12B <DelayedAwake>c__Iterator12B = new WallChunkArchitect.<DelayedAwake>c__Iterator12B();
			<DelayedAwake>c__Iterator12B.isDeserializing = isDeserializing;
			<DelayedAwake>c__Iterator12B.<$>isDeserializing = isDeserializing;
			<DelayedAwake>c__Iterator12B.<>f__this = this;
			return <DelayedAwake>c__Iterator12B;
		}

		private void OnSerializing()
		{
			this._multiPointsPositions.Capacity = this._multiPointsPositions.Count;
			this._multiPointsPositionsCount = this._multiPointsPositions.Count;
		}

		protected virtual void OnDeserialized()
		{
			if (!this._initialized)
			{
				this._initialized = true;
				if (this._forceWasBuilt)
				{
					this._wasBuilt = true;
				}
				if (this._p1 == Vector3.zero)
				{
					this._p1 = base.transform.position;
				}
				if (this._p2 == Vector3.zero)
				{
					this._p2 = this._p1 + base.transform.forward * this._architect.MaxSegmentHorizontalLength * 0.95f;
				}
				if (this._multiPointsPositions == null)
				{
					this._multiPointsPositions = new List<Vector3>
					{
						this._p1,
						this._p2
					};
					this._multiPointsPositionsCount = 2;
				}
				else if (this._multiPointsPositionsCount == 0)
				{
					this._multiPointsPositionsCount = this._multiPointsPositions.Skip(1).IndexOf(this._multiPointsPositions[0]);
					if (this._multiPointsPositionsCount < 0)
					{
						this._multiPointsPositionsCount = this._multiPointsPositions.Count;
					}
					else
					{
						this._multiPointsPositionsCount += 2;
					}
				}
				this._multiPointsPositions.RemoveRange(this._multiPointsPositionsCount, this._multiPointsPositions.Count - this._multiPointsPositionsCount);
				if (this._wasBuilt)
				{
					this.CreateStructure(false);
					this._wallRoot.transform.parent = base.transform;
				}
				else
				{
					this.CreateStructure(false);
					base.StartCoroutine(this.OnPlaced());
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator OnPlaced()
		{
			WallChunkArchitect.<OnPlaced>c__Iterator12C <OnPlaced>c__Iterator12C = new WallChunkArchitect.<OnPlaced>c__Iterator12C();
			<OnPlaced>c__Iterator12C.<>f__this = this;
			return <OnPlaced>c__Iterator12C;
		}

		protected virtual void OnBuilt(GameObject built)
		{
			this.HideToggleAdditionIcon();
			WallChunkArchitect component = built.GetComponent<WallChunkArchitect>();
			component._multiPointsPositions = this._multiPointsPositions;
			component._p1 = this._p1;
			component._p2 = this._p2;
			component._addition = this._addition;
			component._wasBuilt = true;
			component.CurrentSupport = this.CurrentSupport;
			if (this.CurrentSupport != null)
			{
				this.CurrentSupport.Enslaved = true;
			}
		}

		protected virtual int GetLogCost()
		{
			return 5;
		}

		public virtual void ShowToggleAdditionIcon()
		{
			WallChunkArchitect.Additions additions = this.SegmentNextAddition(this._addition);
			switch (additions + 1)
			{
			case WallChunkArchitect.Additions.Window:
				Scene.HudGui.ToggleWallIcon.SetActive(true);
				break;
			case WallChunkArchitect.Additions.Door1:
				Scene.HudGui.ToggleWindowIcon.SetActive(true);
				break;
			case WallChunkArchitect.Additions.Door2:
				Scene.HudGui.ToggleDoor1Icon.SetActive(true);
				break;
			case WallChunkArchitect.Additions.LockedDoor1:
				Scene.HudGui.ToggleDoor2Icon.SetActive(true);
				break;
			}
		}

		public void HideToggleAdditionIcon()
		{
			if (Scene.HudGui)
			{
				Scene.HudGui.ToggleWallIcon.SetActive(false);
				Scene.HudGui.ToggleDoor1Icon.SetActive(false);
				Scene.HudGui.ToggleDoor2Icon.SetActive(false);
				Scene.HudGui.ToggleGate1Icon.SetActive(false);
				Scene.HudGui.ToggleGate2Icon.SetActive(false);
				Scene.HudGui.ToggleWindowIcon.SetActive(false);
			}
		}

		public void ToggleSegmentAddition()
		{
			if (BoltNetwork.isRunning)
			{
				this.HideToggleAdditionIcon();
				this.ShowToggleAdditionIcon();
				ToggleWallAddition toggleWallAddition = ToggleWallAddition.Create(GlobalTargets.OnlyServer);
				toggleWallAddition.Wall = base.GetComponent<BoltEntity>();
				toggleWallAddition.Send();
			}
			else
			{
				this.HideToggleAdditionIcon();
				this.PerformToggleAddition();
				this.ShowToggleAdditionIcon();
			}
		}

		public virtual void PerformToggleAddition()
		{
			if (BoltNetwork.isRunning)
			{
				this.entity.GetState<IWallChunkConstructionState>().Addition = (int)(this._addition = this.SegmentNextAddition(this._addition));
			}
			else
			{
				this.UpdateAddition(this.SegmentNextAddition(this._addition));
			}
		}

		public virtual void UpdateAddition(WallChunkArchitect.Additions addition)
		{
			this._addition = addition;
			UnityEngine.Object.Destroy(this._wallRoot.gameObject);
			this._wallRoot = this.SpawnEdge();
			this._wallRoot.parent = base.transform;
			if (!this._wasBuilt && this._addition >= WallChunkArchitect.Additions.Door1 && this._addition <= WallChunkArchitect.Additions.LockedDoor2)
			{
				Vector3 position = Vector3.Lerp(this._p1, this._p2, 0.5f);
				position.y -= this._logWidth / 2f;
				Vector3 worldPosition = (this._addition != WallChunkArchitect.Additions.Door1) ? this._p1 : this._p2;
				worldPosition.y = position.y;
				Transform transform = (Transform)UnityEngine.Object.Instantiate(Prefabs.Instance.DoorGhostPrefab, position, this._wallRoot.rotation);
				transform.LookAt(worldPosition);
				transform.parent = this._wallRoot;
			}
		}

		public void Clear()
		{
			if (this._wallRoot)
			{
				UnityEngine.Object.Destroy(this._wallRoot.gameObject);
			}
		}

		protected virtual void CreateStructure(bool isRepair = false)
		{
			if (isRepair)
			{
				this.Clear();
				base.StartCoroutine(this.DelayedAwake(true));
			}
			int num = LayerMask.NameToLayer("Prop");
			this._wallRoot = this.SpawnEdge();
			this._wallRoot.parent = base.transform;
			if (this._wasBuilt)
			{
				if (this._lods)
				{
					UnityEngine.Object.Destroy(this._lods.gameObject);
				}
				this._lods = new GameObject("lods").AddComponent<WallChunkLods>();
				this._lods.transform.parent = base.transform;
				this._lods.DefineChunk(this._p1, this._p2, 4.44f * this._logWidth, this._wallRoot, this.Addition);
				BuildingHealth component = base.GetComponent<BuildingHealth>();
				if (component)
				{
					component._renderersRoot = this._wallRoot.gameObject;
				}
				if (!this._wallCollision)
				{
					GameObject gameObject = new GameObject("collision");
					gameObject.transform.parent = this._wallRoot.parent;
					gameObject.transform.position = this._wallRoot.position;
					gameObject.transform.rotation = this._wallRoot.rotation;
					gameObject.tag = "structure";
					this._wallCollision = gameObject.transform;
					GameObject arg_176_0 = this._wallRoot.gameObject;
					int layer = num;
					gameObject.layer = layer;
					arg_176_0.layer = layer;
					float num2 = Vector3.Distance(this._p1, this._p2) / this._logLength;
					float num3 = 7.4f * num2;
					float num4 = 6.75f * (0.31f + (num2 - 1f) / 2f);
					Vector3 vector = Vector3.Lerp(this._p1, this._p2, 0.5f);
					vector.y += this._logWidth * 1.8f;
					vector = this._wallRoot.InverseTransformPoint(vector);
					Vector3 size = new Vector3(1.75f, 4.5f * this._logWidth, num3 * 1f);
					getStructureStrength getStructureStrength = gameObject.AddComponent<getStructureStrength>();
					getStructureStrength._strength = getStructureStrength.strength.normal;
					BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
					boxCollider.center = vector;
					boxCollider.size = size;
					boxCollider.isTrigger = true;
					WallChunkArchitect.Additions addition = this._addition;
					if (addition != WallChunkArchitect.Additions.Wall)
					{
						vector.y += this._logWidth * 2f;
						size.y = 1f * this._logWidth;
						BoxCollider boxCollider2 = gameObject.AddComponent<BoxCollider>();
						boxCollider2.center = vector;
						boxCollider2.size = size;
						FMOD_StudioEventEmitter.CreateAmbientEmitter(gameObject.transform, gameObject.transform.TransformPoint(vector), "event:/ambient/wind/wind_moan_structures");
						vector.y -= 2.25f * this._logWidth;
						vector.z = num3 - num4 / 2f;
						size.y = 3.5f * this._logWidth;
						size.z = num4;
						boxCollider2 = gameObject.AddComponent<BoxCollider>();
						boxCollider2.center = vector;
						boxCollider2.size = size;
						vector.z = num4 / 2f;
						boxCollider2 = gameObject.AddComponent<BoxCollider>();
						boxCollider2.center = vector;
						boxCollider2.size = size;
						if (this._addition == WallChunkArchitect.Additions.Window)
						{
							size.y /= 1.9f;
							size.z = num3 - num4 * 2f;
							size.x *= 1.5f;
							vector.z += num4 / 2f + size.z / 2f;
							vector.y -= 0.9f * this._logWidth;
							boxCollider2 = gameObject.AddComponent<BoxCollider>();
							boxCollider2.center = vector;
							boxCollider2.size = size;
							GameObject gameObject2 = new GameObject("PerchTarget");
							SphereCollider sphereCollider = gameObject2.AddComponent<SphereCollider>();
							sphereCollider.isTrigger = true;
							sphereCollider.radius = 0.145f;
							gameObject2.transform.parent = this._wallRoot;
							vector.y += size.y / 2f;
							gameObject2.transform.localPosition = vector;
						}
					}
					else
					{
						BoxCollider boxCollider2 = gameObject.AddComponent<BoxCollider>();
						boxCollider2.center = vector;
						boxCollider2.size = size;
					}
					gridObjectBlocker gridObjectBlocker = gameObject.AddComponent<gridObjectBlocker>();
					gridObjectBlocker.ignoreOnDisable = true;
					addToBuilt addToBuilt = gameObject.AddComponent<addToBuilt>();
					addToBuilt.addToStructures = true;
					BuildingHealthHitRelay buildingHealthHitRelay = gameObject.AddComponent<BuildingHealthHitRelay>();
				}
				if (this.Addition >= WallChunkArchitect.Additions.Door1 && this._addition <= WallChunkArchitect.Additions.LockedDoor2 && !isRepair)
				{
					Vector3 position = Vector3.Lerp(this._p1, this._p2, 0.5f);
					position.y -= this._logWidth / 2f;
					Vector3 worldPosition = (this._addition != WallChunkArchitect.Additions.Door1 && this._addition != WallChunkArchitect.Additions.LockedDoor1) ? this._p1 : this._p2;
					worldPosition.y = position.y;
					Transform transform = (Transform)UnityEngine.Object.Instantiate(Prefabs.Instance.DoorPrefab, position, this._wallRoot.rotation);
					transform.LookAt(worldPosition);
					transform.parent = base.transform;
				}
			}
		}

		protected virtual WallChunkArchitect.Additions SegmentNextAddition(WallChunkArchitect.Additions addition)
		{
			if (addition == WallChunkArchitect.Additions.Wall)
			{
				return WallChunkArchitect.Additions.Window;
			}
			if (addition == WallChunkArchitect.Additions.Window && Mathf.Abs(Vector3.Dot((this._p2 - this._p1).normalized, Vector3.up)) < this._doorAdditionMaxSlope)
			{
				return WallChunkArchitect.Additions.Door1;
			}
			if (addition == WallChunkArchitect.Additions.Door1 && Mathf.Abs(Vector3.Dot((this._p2 - this._p1).normalized, Vector3.up)) < this._doorAdditionMaxSlope)
			{
				return WallChunkArchitect.Additions.Door2;
			}
			return WallChunkArchitect.Additions.Wall;
		}

		protected virtual Transform SpawnEdge()
		{
			Transform transform = new GameObject("WallChunk").transform;
			transform.transform.position = this._p1;
			Vector3 b = new Vector3(0f, this._logWidth * 0.95f, 0f);
			Vector3 vector = this._p2 - this._p1;
			Quaternion rotation = Quaternion.LookRotation(vector);
			Vector3 vector2 = this._p1;
			transform.position = this._p1;
			transform.LookAt(this._p2);
			Vector3 localScale = new Vector3(1f, 1f, vector.magnitude / this._logLength);
			Vector3 localScale2 = new Vector3(1f, 1f, 0.31f + (localScale.z - 1f) / 2f);
			float d = 1f - localScale2.z / localScale.z;
			for (int i = 0; i < 5; i++)
			{
				Transform transform2 = this.NewLog(vector2, rotation);
				transform2.parent = transform;
				WallChunkArchitect.Additions addition = this._addition;
				switch (addition + 1)
				{
				case WallChunkArchitect.Additions.Window:
					transform2.localScale = localScale;
					break;
				case WallChunkArchitect.Additions.Door1:
					if (i == 2 || i == 3)
					{
						transform2.localScale = localScale2;
						Transform transform3 = this.NewLog(vector2 + vector * d, rotation);
						transform3.parent = transform2;
						transform3.localScale = Vector3.one;
					}
					else
					{
						transform2.localScale = localScale;
					}
					break;
				case WallChunkArchitect.Additions.Door2:
				case WallChunkArchitect.Additions.LockedDoor1:
				case WallChunkArchitect.Additions.LockedDoor2:
				case (WallChunkArchitect.Additions)5:
					if (i < 4)
					{
						transform2.localScale = localScale2;
						Transform transform4 = this.NewLog(vector2 + vector * d, rotation);
						transform4.parent = transform2;
						transform4.localScale = Vector3.one;
					}
					else
					{
						transform2.localScale = localScale;
					}
					break;
				}
				vector2 += b;
			}
			return transform;
		}

		protected virtual void InitAdditionTrigger()
		{
			base.GetComponentInChildren<Craft_Structure>().gameObject.AddComponent<WallAdditionTrigger>();
		}

		protected Transform NewLog(Vector3 position, Quaternion rotation)
		{
			if (float.IsNaN(position.x))
			{
				UnityEngine.Debug.Log(position);
			}
			return (Transform)UnityEngine.Object.Instantiate(this._logPrefab, position, (!this._wasBuilt) ? rotation : this.RandomizeLogRotation(rotation));
		}

		protected virtual Quaternion RandomizeLogRotation(Quaternion logRot)
		{
			return logRot * Quaternion.Euler(0f, UnityEngine.Random.Range(-1.5f, 1.5f), (float)UnityEngine.Random.Range(0, 359));
		}

		protected virtual List<GameObject> GetBuiltRenderers(Transform wallRoot)
		{
			List<GameObject> list = new List<GameObject>(5);
			foreach (Transform transform in wallRoot)
			{
				list.Add(transform.gameObject);
				transform.gameObject.SetActive(false);
			}
			return list;
		}

		public virtual float GetLevel()
		{
			return this._p1.y + this.GetHeight();
		}

		public virtual float GetHeight()
		{
			return 4.9f * this._logWidth;
		}

		public virtual List<Vector3> GetMultiPointsPositions()
		{
			return ((this._multiPointsPositions != null && this._multiPointsPositions.Count > 2) || this.CurrentSupport == null) ? this._multiPointsPositions : this.CurrentSupport.GetMultiPointsPositions();
		}

		public override void Detached()
		{
			this.HideToggleAdditionIcon();
		}
	}
}
