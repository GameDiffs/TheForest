using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Buildings.World;
using TheForest.Items;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/Creation/Wall Defensive Chunk Reinforcement")]
	public class WallDefensiveChunkReinforcement : EntityBehaviour, IEntityReplicationFilter
	{
		[SerializeThis]
		public WallDefensiveChunkArchitect _chunk;

		public bool _wasPlaced;

		public bool _wasBuilt;

		public Craft_Structure _craftStructure;

		[ItemIdPicker(Item.Types.Equipment)]
		public int _stickItemId;

		public int _stickCostPerLog = 2;

		[ItemIdPicker(Item.Types.Equipment)]
		public int _rockItemId;

		public int _rockCostPerLog = 3;

		public Transform _perLogPrefab;

		public float _hpBonus = 1.45f;

		public float _stickRandomFactor = 4f;

		public float _rockRandomFactor = 20f;

		private bool _initialized;

		private int _logCount;

		private Transform _wallRoot;

		public Transform BuiltLogPrefab
		{
			get
			{
				return Prefabs.Instance.LogWallDefensiveExBuiltPrefab;
			}
		}

		bool IEntityReplicationFilter.AllowReplicationTo(BoltConnection connection)
		{
			return this.GetParentHack() && connection.ExistsOnRemote(this.GetParentHack()) == ExistsResult.Yes;
		}

		private void Awake()
		{
			base.StartCoroutine(this.DelayedAwake(LevelSerializer.IsDeserializing));
			base.enabled = false;
		}

		[DebuggerHidden]
		private IEnumerator DelayedAwake(bool isDeserializing)
		{
			WallDefensiveChunkReinforcement.<DelayedAwake>c__Iterator140 <DelayedAwake>c__Iterator = new WallDefensiveChunkReinforcement.<DelayedAwake>c__Iterator140();
			<DelayedAwake>c__Iterator.isDeserializing = isDeserializing;
			<DelayedAwake>c__Iterator.<$>isDeserializing = isDeserializing;
			<DelayedAwake>c__Iterator.<>f__this = this;
			return <DelayedAwake>c__Iterator;
		}

		private void Update()
		{
			if (this._chunk && !this._wallRoot)
			{
				LocalPlayer.Create.BuildingPlacer.ForcedParent = this._chunk.gameObject;
				this.CreateStructure(false);
			}
			if (this._wallRoot)
			{
				LocalPlayer.Create.BuildingPlacer.SetClear();
			}
			else
			{
				LocalPlayer.Create.BuildingPlacer.SetNotclear();
			}
			if (TheForest.Utils.Input.GetButtonDown("Take"))
			{
				this._wasPlaced = true;
				base.enabled = false;
			}
			Scene.HudGui.PlaceWallIcon.SetActive(this._wallRoot && !this._wasPlaced);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!this._wasPlaced)
			{
				WallDefensiveChunkArchitect componentInParent = other.GetComponentInParent<WallDefensiveChunkArchitect>();
				if (componentInParent && componentInParent.WasBuilt && !componentInParent.Reinforcement)
				{
					if (Vector3.Dot((componentInParent.transform.position - base.transform.position).normalized, componentInParent.transform.right) < 0f)
					{
						base.transform.rotation = Quaternion.LookRotation(componentInParent.P2 - componentInParent.P1);
					}
					else
					{
						base.transform.rotation = Quaternion.LookRotation(componentInParent.P1 - componentInParent.P2);
					}
					this._chunk = componentInParent;
					this.Clear();
					base.GetComponent<Renderer>().enabled = false;
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (!this._wasPlaced)
			{
				WallDefensiveChunkArchitect componentInParent = other.GetComponentInParent<WallDefensiveChunkArchitect>();
				if (componentInParent && componentInParent == this._chunk)
				{
					this._chunk = null;
					this.Clear();
					base.GetComponent<Renderer>().enabled = true;
				}
			}
		}

		protected void OnDeserialized()
		{
			if (!this._initialized && (!BoltNetwork.isRunning || (this.entity && this.entity.isAttached)))
			{
				if (!this._chunk)
				{
					this._chunk = base.GetComponentInParent<WallDefensiveChunkArchitect>();
					if (!this._chunk)
					{
						if (!BoltNetwork.isRunning)
						{
							UnityEngine.Object.Destroy(base.gameObject);
						}
						return;
					}
				}
				if (!this._chunk.Reinforcement)
				{
					this._chunk.Reinforcement = this;
				}
				if (!this._chunk.Reinforcement.Equals(this) && !BoltNetwork.isClient)
				{
					if (BoltNetwork.isRunning)
					{
						BoltNetwork.Destroy(base.gameObject);
					}
					else
					{
						UnityEngine.Object.Destroy(base.gameObject);
					}
					return;
				}
				if (BoltNetwork.isServer)
				{
					this.SetParentHack(this._chunk.GetComponent<BoltEntity>());
				}
				this._initialized = true;
				if (this._wasBuilt)
				{
					BuildingHealth component = this._chunk.GetComponent<BuildingHealth>();
					component._maxHP *= this._hpBonus;
					this.CreateStructure(false);
					this._wallRoot.transform.parent = base.transform;
				}
				else
				{
					base.transform.position = Vector3.Lerp(this._chunk.P2, this._chunk.P1, 0.5f);
					this.CreateStructure(false);
					base.StartCoroutine(this.OnPlaced());
				}
			}
		}

		private void OnDestroy()
		{
			this.Clear();
		}

		[DebuggerHidden]
		private IEnumerator OnPlaced()
		{
			WallDefensiveChunkReinforcement.<OnPlaced>c__Iterator141 <OnPlaced>c__Iterator = new WallDefensiveChunkReinforcement.<OnPlaced>c__Iterator141();
			<OnPlaced>c__Iterator.<>f__this = this;
			return <OnPlaced>c__Iterator;
		}

		private void OnBuilt(GameObject built)
		{
			WallDefensiveChunkReinforcement component = built.GetComponent<WallDefensiveChunkReinforcement>();
			component._chunk = this._chunk;
			component._chunk.Reinforcement = component;
			this._chunk.GetComponent<BuildingHealth>().Invoke("Repair", 0.2f);
		}

		public void Clear()
		{
			if (this._wallRoot)
			{
				UnityEngine.Object.Destroy(this._wallRoot.gameObject);
				this._wallRoot = null;
			}
		}

		protected void CreateStructure(bool isRepair = false)
		{
			if (isRepair)
			{
				this.Clear();
			}
			int num = LayerMask.NameToLayer("Prop");
			this._wallRoot = this.SpawnEdge();
			if (this._wasPlaced || this._wasBuilt)
			{
				this._wallRoot.parent = base.transform;
			}
			if (this._wasBuilt)
			{
				Renderer[] componentsInChildren = this._wallRoot.gameObject.GetComponentsInChildren<Renderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Renderer renderer = componentsInChildren[i];
					if (renderer.name.Equals("s"))
					{
						renderer.transform.rotation *= Quaternion.Euler(UnityEngine.Random.Range(-this._stickRandomFactor, this._stickRandomFactor), UnityEngine.Random.Range(-this._stickRandomFactor, this._stickRandomFactor), UnityEngine.Random.Range(-this._stickRandomFactor, this._stickRandomFactor));
					}
					else
					{
						renderer.transform.rotation *= Quaternion.Euler(UnityEngine.Random.Range(-this._rockRandomFactor, this._rockRandomFactor), UnityEngine.Random.Range(-this._rockRandomFactor, this._rockRandomFactor), UnityEngine.Random.Range(-this._rockRandomFactor, this._rockRandomFactor));
					}
				}
			}
		}

		protected Transform SpawnEdge()
		{
			Transform transform = new GameObject("WallChunk").transform;
			transform.transform.position = this._chunk.P1;
			Vector3 vector = this._chunk.P2 - this._chunk.P1;
			Vector3 a = Vector3.Scale(vector, new Vector3(1f, 0f, 1f));
			Vector3 normalized = a.normalized;
			float y = Mathf.Tan(Vector3.Angle(vector, normalized) * 0.0174532924f) * this._chunk.LogWidth;
			Quaternion rotation = Quaternion.LookRotation(a * (float)((Vector3.Dot(base.transform.forward, vector) <= 0f) ? -1 : 1));
			float num = Vector3.Distance(this._chunk.P1, this._chunk.P2);
			this._logCount = Mathf.RoundToInt(num / this._chunk.LogWidth);
			Vector3 b = normalized * num / (float)this._logCount;
			b.y = y;
			if (vector.y < 0f)
			{
				b.y *= -1f;
			}
			Vector3 vector2 = this._chunk.P1;
			transform.position = this._chunk.P1;
			transform.LookAt(this._chunk.P2);
			for (int i = 0; i < this._logCount; i++)
			{
				Transform transform2 = this.NewLog(vector2, rotation);
				transform2.parent = transform;
				vector2 += b;
			}
			return transform;
		}

		private Transform NewLog(Vector3 position, Quaternion rotation)
		{
			return (Transform)UnityEngine.Object.Instantiate(this._perLogPrefab, position, (!this._wasBuilt) ? rotation : this.RandomizeLogRotation(rotation));
		}

		private Quaternion RandomizeLogRotation(Quaternion logRot)
		{
			return logRot * Quaternion.Euler(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1.5f, 1.5f), UnityEngine.Random.Range(-1.5f, 1.5f));
		}

		private int GetStickCost()
		{
			return this._logCount * this._stickCostPerLog;
		}

		private int GetRockCost()
		{
			return this._logCount * this._rockCostPerLog;
		}

		public override void Attached()
		{
			if (BoltNetwork.isServer && !this.GetParentHack() && this._chunk)
			{
				this.SetParentHack(this._chunk.GetComponent<BoltEntity>());
			}
		}

		private BoltEntity GetParentHack()
		{
			if (!this.entity || !this.entity.isAttached)
			{
				return null;
			}
			if (this.entity.StateIs<IBuildingState>())
			{
				return this.entity.GetState<IBuildingState>().ParentHack;
			}
			if (this.entity.StateIs<IConstructionState>())
			{
				return this.entity.GetState<IConstructionState>().ParentHack;
			}
			throw new Exception("invalid state type");
		}

		private void SetParentHack(BoltEntity parent)
		{
			if (this.entity.StateIs<IBuildingState>())
			{
				this.entity.GetState<IBuildingState>().ParentHack = parent;
				return;
			}
			if (this.entity.StateIs<IConstructionState>())
			{
				this.entity.GetState<IConstructionState>().ParentHack = parent;
				return;
			}
			throw new Exception("invalid state type");
		}
	}
}
