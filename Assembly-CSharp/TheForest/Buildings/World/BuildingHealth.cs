using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Buildings.Creation;
using TheForest.Buildings.Interfaces;
using TheForest.Utils;
using TheForest.Utils.Enums;
using TheForest.World;
using UniLinq;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic]
	public class BuildingHealth : EntityEventListener<IBuildingDestructibleState>, IRepairableStructure
	{
		public enum RepairModes
		{
			RenderersOnly,
			FullReplace
		}

		public int _detachedLayer;

		public float _maxHP = 100f;

		public bool _canBeRepaired = true;

		public BuildingHealth.RepairModes _repairMode;

		public Vector3 _repairTriggerOffset;

		public float _destructionForceMultiplier = 1f;

		public GameObject _renderersRoot;

		public GameObject _destroyTarget;

		public GameObject _dustPrefab;

		public Create.BuildingTypes _type;

		public CapsuleDirections _capsuleDirection = CapsuleDirections.Z;

		public BuildingHealthChunk[] _chunks = new BuildingHealthChunk[0];

		[SerializeThis]
		private float _hp;

		[SerializeThis]
		private int _collapsedLogs;

		[SerializeThis]
		private int _repairMaterial;

		[SerializeThis]
		private int _repairLogs;

		private float _lastHit;

		private BuildingRepair _repairTrigger;

		private Collider[] _colliders = new Collider[0];

		private int _distorts;

		private bool _collapsing;

		public float Hp
		{
			get
			{
				return this.HpActual;
			}
			set
			{
				this.HpActual = value;
			}
		}

		public int RepairMaterial
		{
			get
			{
				return this.RepairMaterialActual;
			}
		}

		public int RepairLogs
		{
			get
			{
				return this._repairLogsActual;
			}
		}

		public int CollapsedLogs
		{
			get
			{
				return this._collapsedLogs / 3;
			}
		}

		private float HpActual
		{
			get
			{
				if (BoltNetwork.isRunning && this.entity && this.entity.isAttached)
				{
					return base.state.hp;
				}
				return this._hp;
			}
			set
			{
				if (BoltNetwork.isRunning && this.entity && this.entity.isAttached && this.entity.isOwner)
				{
					base.state.hp = value;
				}
				this._hp = value;
			}
		}

		private int RepairMaterialActual
		{
			get
			{
				if (BoltNetwork.isRunning && this.entity && this.entity.isAttached)
				{
					return base.state.repairMaterial;
				}
				return this._repairMaterial;
			}
			set
			{
				if (BoltNetwork.isRunning && this.entity && this.entity.isAttached && this.entity.isOwner)
				{
					base.state.repairMaterial = value;
				}
				this._repairMaterial = value;
			}
		}

		private int _repairLogsActual
		{
			get
			{
				if (BoltNetwork.isRunning && this.entity && this.entity.isAttached)
				{
					return base.state.repairLogs;
				}
				return this._repairLogs;
			}
			set
			{
				if (BoltNetwork.isRunning && this.entity && this.entity.isAttached && this.entity.isOwner)
				{
					base.state.repairLogs = value;
				}
				this._repairLogs = value;
			}
		}

		private void Awake()
		{
			if (BoltNetwork.isRunning)
			{
				this._colliders = (from x in base.GetComponentsInChildren<Collider>()
				where x.CompareTag("structure")
				select x).ToArray<Collider>();
			}
			if (!LevelSerializer.IsDeserializing || this.HpActual == 0f)
			{
				this.HpActual = this._maxHP;
			}
		}

		private void OnDestroy()
		{
			if (this._repairTrigger)
			{
				UnityEngine.Object.Destroy(this._repairTrigger.gameObject);
			}
		}

		private void OnDeserialized()
		{
			if (this.HpActual == 0f)
			{
				this.HpActual = this._maxHP;
			}
			if (this.HpActual < this._maxHP && !BoltNetwork.isClient)
			{
				this.SpawnRepairTrigger();
			}
		}

		private void OnWillDestroy()
		{
			if (!BoltNetwork.isRunning || BoltNetwork.isServer)
			{
				this.LocalizedHit(new LocalizedHitData(base.transform.position, this.HpActual));
			}
		}

		public void LocalizedHit(LocalizedHitData data)
		{
			if (this._lastHit + 0.5f < Time.realtimeSinceStartup)
			{
				this._lastHit = Time.realtimeSinceStartup;
				if (BoltNetwork.isRunning)
				{
					LocalizedHit localizedHit = global::LocalizedHit.Create(GlobalTargets.OnlyServer);
					localizedHit.Building = this.entity;
					localizedHit.Damage = data._damage;
					localizedHit.Position = data._position;
					localizedHit.Chunk = -1;
					localizedHit.Send();
					if (BoltNetwork.isClient)
					{
						Prefabs.Instance.SpawnWoodHitPS(data._position, Quaternion.LookRotation(base.transform.right));
						this.DistortReal(data);
					}
				}
				else
				{
					this.LocalizedHitReal(data);
				}
			}
		}

		public int CalcMissingRepairMaterial()
		{
			return Mathf.Max(this.CalcTotalRepairMaterial() - this.RepairMaterialActual, 0);
		}

		public int CalcTotalRepairMaterial()
		{
			int num = 3;
			int num2 = 3;
			return num + Mathf.RoundToInt((this._maxHP - this.HpActual) / 500f * (float)num2);
		}

		public int CalcMissingRepairLogs()
		{
			return Mathf.Max(this._collapsedLogs / 3 - this._repairLogs, 0);
		}

		public void AddRepairMaterial(bool isLog)
		{
			if (BoltNetwork.isRunning)
			{
				AddRepairMaterial addRepairMaterial = global::AddRepairMaterial.Create(GlobalTargets.OnlyServer);
				addRepairMaterial.Building = this.entity;
				addRepairMaterial.IsLog = isLog;
				addRepairMaterial.Send();
			}
			else
			{
				this.AddRepairMaterialReal(isLog);
			}
		}

		public void AddRepairMaterialReal(bool isLog)
		{
			if (isLog)
			{
				this._repairLogs++;
			}
			else
			{
				this.RepairMaterialActual++;
			}
			if (this.CalcMissingRepairMaterial() == 0 && this.CalcMissingRepairLogs() == 0)
			{
				this.Repair();
			}
		}

		public void DamageOnly(LocalizedHitData data, int collapsedLogs = 0)
		{
			if (this.HpActual > 0f)
			{
				this.HpActual -= data._damage;
				if (collapsedLogs > 0)
				{
					LocalPlayer.Sfx.PlayStructureBreak(base.gameObject, this.HpActual / this._maxHP);
				}
				this._collapsedLogs += collapsedLogs;
				if (this.HpActual <= 0f)
				{
					this.Collapse(data._position);
				}
				else if (!this._repairTrigger)
				{
					this.SpawnRepairTrigger();
				}
			}
		}

		public void LocalizedHitReal(LocalizedHitData data)
		{
			if (!Cheats.NoDestruction && this.HpActual > 0f)
			{
				Prefabs.Instance.SpawnWoodHitPS(data._position, Quaternion.LookRotation(data._position - base.transform.position));
				this.DamageOnly(data, 0);
				if (this.HpActual > 0f)
				{
					this.Distort(data);
				}
			}
		}

		private void DistortReal(LocalizedHitData data)
		{
			if (data._damage > 0f)
			{
				Renderer[] array = (!this._renderersRoot) ? base.transform.GetComponentsInChildren<Renderer>() : this._renderersRoot.GetComponentsInChildren<Renderer>();
				float num = Mathf.Clamp(data._damage * data._distortRatio * 10f / this._maxHP, 1f, 10f);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].enabled)
					{
						Transform transform = array[i].transform;
						GameObject gameObject = transform.gameObject;
						float num2 = Vector3.Distance(array[i].bounds.center, data._position);
						if (num2 < 12f)
						{
							float num3 = (1f - num2 / 12f) * num;
							transform.localRotation *= Quaternion.Euler((float)UnityEngine.Random.Range(-1, 1) * num3, (float)UnityEngine.Random.Range(-1, 1) * num3, (float)UnityEngine.Random.Range(-1, 1) * num3);
						}
					}
				}
			}
		}

		private void Distort(LocalizedHitData data)
		{
			if (BoltNetwork.isRunning)
			{
				base.state.BuildingHits++;
			}
			else
			{
				this.DistortReal(data);
			}
		}

		[DebuggerHidden]
		private IEnumerator CollapseRoutine(Vector3 origin)
		{
			BuildingHealth.<CollapseRoutine>c__Iterator14B <CollapseRoutine>c__Iterator14B = new BuildingHealth.<CollapseRoutine>c__Iterator14B();
			<CollapseRoutine>c__Iterator14B.origin = origin;
			<CollapseRoutine>c__Iterator14B.<$>origin = origin;
			<CollapseRoutine>c__Iterator14B.<>f__this = this;
			return <CollapseRoutine>c__Iterator14B;
		}

		public void Collapse(Vector3 origin)
		{
			if (BoltNetwork.isRunning)
			{
				if (this.entity.isAttached && base.state != null)
				{
					base.state.BuildingCollapsePoint = origin;
				}
			}
			else if (base.gameObject.activeInHierarchy)
			{
				base.StartCoroutine(this.CollapseRoutine(origin));
			}
		}

		private void SpawnRepairTrigger()
		{
			if (this._repairTrigger || !this._canBeRepaired)
			{
				return;
			}
			if (BoltNetwork.isRunning && this.entity && this.entity.isAttached && this.entity.isOwner)
			{
				base.state.repairTrigger = true;
			}
			this._repairTrigger = UnityEngine.Object.Instantiate<BuildingRepair>(Prefabs.Instance.BuildingRepairTriggerPrefab);
			this._repairTrigger._target = this;
			IStructureSupport structureSupport = (IStructureSupport)base.GetComponent(typeof(IStructureSupport));
			if (structureSupport != null)
			{
				if (structureSupport is WallChunkArchitect)
				{
					this._repairTrigger.transform.position = Vector3.Lerp(((WallChunkArchitect)structureSupport).P1, ((WallChunkArchitect)structureSupport).P2, 0.5f) + base.transform.right + this._repairTriggerOffset;
					this._repairTrigger.transform.rotation = Quaternion.LookRotation(base.transform.right);
					this._repairTrigger._iconSheen2.SetActive(true);
					this._repairTrigger._iconPickUp2.SetActive(true);
				}
				else
				{
					this._repairTrigger.transform.position = MultipointUtils.CenterOf(structureSupport.GetMultiPointsPositions()) + this._repairTriggerOffset;
				}
			}
			else
			{
				this._repairTrigger.transform.position = base.transform.position + this._repairTriggerOffset;
			}
		}

		private void Repair()
		{
			this.RepairMaterialActual = 0;
			this._repairLogsActual = 0;
			this._collapsedLogs = 0;
			if (this._repairTrigger)
			{
				UnityEngine.Object.Destroy(this._repairTrigger.gameObject);
				this._repairTrigger = null;
			}
			this.HpActual = this._maxHP;
			if (BoltNetwork.isServer)
			{
				PerformRepairBuilding performRepairBuilding = PerformRepairBuilding.Create(GlobalTargets.AllClients);
				performRepairBuilding.Building = this.entity;
				performRepairBuilding.Send();
			}
			this.RespawnBuilding();
		}

		public void RespawnBuilding()
		{
			if (BoltNetwork.isServer)
			{
				base.state.repairTrigger = false;
			}
			GameObject gameObject2;
			if (this._type != Create.BuildingTypes.None)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(LocalPlayer.Create._blueprints.Find((Create.BuildingBlueprint bp) => bp._type == this._type)._builtPrefab);
				gameObject.transform.position = base.transform.position;
				gameObject.transform.rotation = base.transform.rotation;
				if (this._repairMode == BuildingHealth.RepairModes.RenderersOnly)
				{
					Transform transform = (!this._renderersRoot) ? base.transform : this._renderersRoot.transform;
					for (int i = transform.childCount - 1; i >= 0; i--)
					{
						Transform child = transform.GetChild(i);
						if ((!child.GetComponent<StoreInformation>() || child.GetComponent<ForceRepairRespawn>()) && !child.GetComponent<SkipRepairRespawn>() && !child.name.Contains("Anchor"))
						{
							UnityEngine.Object.Destroy(child.gameObject);
						}
					}
					Transform transform2 = (!this._renderersRoot) ? gameObject.transform : gameObject.GetComponent<BuildingHealth>()._renderersRoot.transform;
					for (int j = transform2.childCount - 1; j >= 0; j--)
					{
						Transform child2 = transform2.GetChild(j);
						if ((!child2.GetComponent<StoreInformation>() || child2.GetComponent<ForceRepairRespawn>()) && !child2.GetComponent<SkipRepairRespawn>() && !child2.name.Contains("Anchor"))
						{
							child2.parent = transform;
						}
					}
					UnityEngine.Object.Destroy(gameObject);
					gameObject2 = base.gameObject;
				}
				else
				{
					UnityEngine.Object.Destroy(base.gameObject);
					gameObject2 = gameObject;
					if (BoltNetwork.isServer)
					{
						BoltNetwork.Attach(gameObject);
					}
				}
			}
			else
			{
				gameObject2 = base.gameObject;
			}
			gameObject2.SendMessage("ResetHP", SendMessageOptions.DontRequireReceiver);
			gameObject2.SendMessage("CreateStructure", true, SendMessageOptions.DontRequireReceiver);
			LocalPlayer.Sfx.PlayBuildingRepair(gameObject2);
		}

		public int GetChunkIndex(BuildingHealthChunk buildingHealthChunk)
		{
			for (int i = 0; i < this._chunks.Length; i++)
			{
				if (object.ReferenceEquals(this._chunks[i], buildingHealthChunk))
				{
					return i;
				}
			}
			return -1;
		}

		public override void OnEvent(LocalizedHit evnt)
		{
		}

		public BuildingHealthChunk GetChunk(int index)
		{
			return this._chunks[index];
		}

		private void CheckRepairTriggerState()
		{
			if (base.state.repairTrigger)
			{
				this.SpawnRepairTrigger();
			}
			else if (this._repairTrigger)
			{
				UnityEngine.Object.Destroy(this._repairTrigger.gameObject);
				this._repairTrigger = null;
			}
		}

		private void CheckBuildingHits()
		{
			while (this._distorts < base.state.BuildingHits)
			{
				this.DistortRandom();
				this._distorts++;
			}
		}

		private void CheckBuildingCollapsePoint()
		{
			base.StartCoroutine(this.CollapseRoutine(base.state.BuildingCollapsePoint));
		}

		public override void Attached()
		{
			if (this.entity.StateIs<IBuildingDestructibleState>())
			{
				if (this.entity.isOwner)
				{
					base.state.hp = this._hp;
					if (this._repairTrigger)
					{
						base.state.repairTrigger = true;
					}
				}
				if (!this.entity.isOwner)
				{
					base.state.AddCallback("repairTrigger", new PropertyCallbackSimple(this.CheckRepairTriggerState));
					this.CheckRepairTriggerState();
				}
				base.state.AddCallback("BuildingHits", new PropertyCallbackSimple(this.CheckBuildingHits));
				this.CheckBuildingHits();
				base.state.AddCallback("BuildingCollapsePoint", new PropertyCallbackSimple(this.CheckBuildingCollapsePoint));
			}
			else
			{
				UnityEngine.Debug.Log("Wrong bolt state on: " + base.gameObject.name);
			}
		}

		public override void Detached()
		{
			for (int i = base.transform.childCount - 1; i >= 0; i--)
			{
				Transform child = base.transform.GetChild(i);
				if (child.GetComponent<BoltEntity>())
				{
					child.parent = null;
				}
			}
		}

		public static Vector3 GetPointInCollider(BoxCollider c)
		{
			Bounds bounds = c.bounds;
			Vector3 center = bounds.center;
			float y = UnityEngine.Random.Range(center.y - bounds.extents.y, center.y + bounds.extents.z);
			float x = UnityEngine.Random.Range(center.x - bounds.extents.x, center.x + bounds.extents.x);
			float z = UnityEngine.Random.Range(center.z - bounds.extents.z, center.z + bounds.extents.z);
			return new Vector3(x, y, z);
		}

		public static Vector3 GetPointInCollider(SphereCollider c)
		{
			Vector3 center = c.bounds.center;
			float y = UnityEngine.Random.Range(center.y - c.radius, center.y + c.radius);
			float x = UnityEngine.Random.Range(center.x - c.radius, center.x + c.radius);
			float z = UnityEngine.Random.Range(center.z - c.radius, center.z + c.radius);
			return new Vector3(x, y, z);
		}

		private void DistortRandom()
		{
			if (this._chunks.Length <= 0)
			{
				if (this._colliders.Length > 0)
				{
					Collider collider = this._colliders[UnityEngine.Random.Range(0, this._colliders.Length)];
					Vector3 vector = default(Vector3);
					if (collider is BoxCollider)
					{
						vector = BuildingHealth.GetPointInCollider(collider as BoxCollider);
					}
					if (collider is SphereCollider)
					{
						vector = BuildingHealth.GetPointInCollider(collider as SphereCollider);
					}
					if (vector != Vector3.zero)
					{
						LocalizedHitData data = default(LocalizedHitData);
						float num = (1f - this.Hp / this._maxHP) / (float)base.state.BuildingHits;
						data._damage = num * this._maxHP;
						data._position = vector;
						data._distortRatio = num * 10f;
						this.DistortReal(data);
					}
				}
			}
		}
	}
}
