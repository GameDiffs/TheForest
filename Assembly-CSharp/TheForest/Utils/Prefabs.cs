using PathologicalGames;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Buildings.Creation;
using TheForest.Buildings.World;
using TheForest.Items;
using TheForest.Networking;
using UnityEngine;

namespace TheForest.Utils
{
	public class Prefabs : MonoBehaviour
	{
		[Serializable]
		public class TrophyPrefab
		{
			[ItemIdPicker]
			public int _itemId;

			public Transform _prefab;
		}

		[NameFromEnumIndex(typeof(EnemyType)), Header("AI")]
		public GameObject[] _deadMutantBodies;

		[Header("Buildings")]
		public Transform LogBuiltPrefab;

		public Transform LogBridgeBuiltPrefab;

		public Transform LogStairsBuiltPrefab;

		public Transform LogStiltStairsBuiltPrefab;

		public Transform LogStiltStairsGhostBuiltPrefab;

		public Transform LogFloorBuiltPrefab;

		public Transform LogRoofBuiltPrefab;

		public Transform LogWallExBuiltPrefab;

		public Mesh LogWallExBuiltPrefabLOD1;

		public Mesh LogWallExBuiltPrefabLOD2;

		public Transform LogWallDefensiveExBuiltPrefab;

		public Transform[] RockFenceChunksGhostPrefabs;

		public Transform[] RockFenceChunksGhostFillPrefabs;

		public Transform[] RockFenceChunksBuiltPrefabs;

		public Transform PerLogWDReinforcementBuiltPrefab;

		public WallDefensiveGateArchitect WallDefensiveGateGhostPrefab;

		public WallDefensiveChunkArchitect WallDefensiveChunkGhostPrefab;

		public WallDefensiveGate WallDefensiveGateTriggerPrefab;

		public Transform StickFenceExBuiltPrefab;

		public Transform BoneFenceExBuiltPrefab;

		public Transform RockPathExBuiltPrefab;

		public Transform DoorPrefab;

		public Transform DoorGhostPrefab;

		public Transform AnchorPrefab;

		public Transform TorsoPrefab;

		public Transform TorsoGhostPrefab;

		public GardenDirtPile[] GardenDirtPilePrefabs;

		public GardenDirtPile[] GardenDirtPileSmallPrefabs;

		public Prefabs.TrophyPrefab[] TrophyPrefabs;

		public BuildMission BuildMissionPrefab;

		public TriggerTagSensor WaterSensor;

		[Header("Buildings Mats")]
		public Material GhostClear;

		public Material WallChunkBillboardMat;

		[Header("Drawings")]
		public Material[] TimmyDrawingsMats;

		public Transform[] TimmyDrawingsPrefab;

		[Header("UI")]
		public GameObject LogTextPrefab;

		public GameObject LogIconPrefab;

		public GameObject StickTextPrefab;

		public GameObject StickIconPrefab;

		public GameObject RockTextPrefab;

		public GameObject RockIconPrefab;

		[Header("Destruction")]
		public Transform BuildingCollapsingDust;

		public Transform LogPickupPrefab;

		public BuildingRepair BuildingRepairTriggerPrefab;

		public GameObject DestroyedLeafShelter;

		[Header("Particles")]
		public ParticleSystem WoodHitPSPrefab;

		public ParticleSystem FireHitPSPrefab;

		public ParticleSystem FliesPSPrefab;

		public ParticleSystem[] BloodHitPSPrefabs;

		public ParticleSystem[] FootStepPrefabs;

		[Header("Mp")]
		public GameObject HashPositionToNamePrefab;

		public GameObject PlayerPrefab;

		public StealItemTrigger DeadBackpackPrefab;

		public GameObject DeadPlayerLootIconSheen;

		public GameObject DeadPlayerLootIconPickup;

		[Header("Environment")]
		public Material LowQualityWaterMaterial;

		public Material PlaneMossMaterial;

		[NameFromEnumIndex(typeof(TitleScreen.GameSetup.GameModes)), Header("GameModes")]
		public GameObject[] GameModePrefabs;

		private SpawnPool _psSpawnPool;

		public static Prefabs Instance;

		private void Awake()
		{
			Prefabs.Instance = this;
		}

		[DebuggerHidden]
		private IEnumerator Start()
		{
			Prefabs.<Start>c__Iterator1B8 <Start>c__Iterator1B = new Prefabs.<Start>c__Iterator1B8();
			<Start>c__Iterator1B.<>f__this = this;
			return <Start>c__Iterator1B;
		}

		private void OnDestroy()
		{
			if (Prefabs.Instance == this)
			{
				Prefabs.Instance = null;
			}
		}

		public void SpawnWoodHitPS(Vector3 pos, Quaternion rot)
		{
			if (this._psSpawnPool)
			{
				this._psSpawnPool.Spawn(this.WoodHitPSPrefab, pos, rot);
			}
		}

		public void SpawnFireHitPS(Vector3 pos, Quaternion rot)
		{
			if (this._psSpawnPool)
			{
				this._psSpawnPool.Spawn(this.FireHitPSPrefab, pos, rot);
			}
		}

		public void SpawnBloodHitPS(int index, Vector3 pos, Quaternion rot)
		{
			if (this._psSpawnPool)
			{
				this._psSpawnPool.Spawn(this.BloodHitPSPrefabs[(int)Mathf.Repeat((float)index, (float)this.BloodHitPSPrefabs.Length)], pos, rot);
			}
		}

		public void SpawnFootPS(int index, Vector3 pos, Quaternion rot)
		{
			if (this._psSpawnPool)
			{
				this._psSpawnPool.Spawn(this.FootStepPrefabs[(int)Mathf.Repeat((float)index, (float)this.FootStepPrefabs.Length)], pos, rot);
			}
		}

		public void SpawnNextFrame(Transform prefab, Vector3 pos, Quaternion rot, Transform parent = null)
		{
			base.StartCoroutine(this.DelayedSpawn(prefab, pos, rot, parent));
		}

		public void SpawnNextFrameMP(Transform prefab, Vector3 pos, Quaternion rot, Transform parent = null)
		{
			base.StartCoroutine(this.DelayedSpawnMP(prefab, pos, rot, parent));
		}

		public void SpawnNextFrameFromPool(string pool, Transform prefab, Vector3 pos, Quaternion rot)
		{
			base.StartCoroutine(this.DelayedPoolSpawn(pool, prefab, pos, rot));
		}

		public void SpawnNextFrameFromPoolMP(string pool, Transform prefab, Vector3 pos, Quaternion rot)
		{
			base.StartCoroutine(this.DelayedPoolSpawnMP(pool, prefab, pos, rot));
		}

		public void SpawnFromPool(string pool, Transform prefab, Vector3 pos, Quaternion rot)
		{
			PoolManager.Pools[pool].Spawn(prefab, pos, rot);
		}

		public void SpawnFromPoolMP(string pool, Transform prefab, Vector3 pos, Quaternion rot)
		{
			Transform transform = PoolManager.Pools[pool].Spawn(prefab, pos, rot);
			transform.parent = null;
			BoltNetwork.Attach(transform.gameObject);
		}

		[DebuggerHidden]
		private IEnumerator DelayedSpawn(Transform prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			Prefabs.<DelayedSpawn>c__Iterator1B9 <DelayedSpawn>c__Iterator1B = new Prefabs.<DelayedSpawn>c__Iterator1B9();
			<DelayedSpawn>c__Iterator1B.prefab = prefab;
			<DelayedSpawn>c__Iterator1B.pos = pos;
			<DelayedSpawn>c__Iterator1B.rot = rot;
			<DelayedSpawn>c__Iterator1B.parent = parent;
			<DelayedSpawn>c__Iterator1B.<$>prefab = prefab;
			<DelayedSpawn>c__Iterator1B.<$>pos = pos;
			<DelayedSpawn>c__Iterator1B.<$>rot = rot;
			<DelayedSpawn>c__Iterator1B.<$>parent = parent;
			return <DelayedSpawn>c__Iterator1B;
		}

		[DebuggerHidden]
		private IEnumerator DelayedSpawnMP(Transform prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			Prefabs.<DelayedSpawnMP>c__Iterator1BA <DelayedSpawnMP>c__Iterator1BA = new Prefabs.<DelayedSpawnMP>c__Iterator1BA();
			<DelayedSpawnMP>c__Iterator1BA.prefab = prefab;
			<DelayedSpawnMP>c__Iterator1BA.pos = pos;
			<DelayedSpawnMP>c__Iterator1BA.rot = rot;
			<DelayedSpawnMP>c__Iterator1BA.parent = parent;
			<DelayedSpawnMP>c__Iterator1BA.<$>prefab = prefab;
			<DelayedSpawnMP>c__Iterator1BA.<$>pos = pos;
			<DelayedSpawnMP>c__Iterator1BA.<$>rot = rot;
			<DelayedSpawnMP>c__Iterator1BA.<$>parent = parent;
			return <DelayedSpawnMP>c__Iterator1BA;
		}

		[DebuggerHidden]
		private IEnumerator DelayedPoolSpawn(string pool, Transform prefab, Vector3 pos, Quaternion rot)
		{
			Prefabs.<DelayedPoolSpawn>c__Iterator1BB <DelayedPoolSpawn>c__Iterator1BB = new Prefabs.<DelayedPoolSpawn>c__Iterator1BB();
			<DelayedPoolSpawn>c__Iterator1BB.pool = pool;
			<DelayedPoolSpawn>c__Iterator1BB.prefab = prefab;
			<DelayedPoolSpawn>c__Iterator1BB.pos = pos;
			<DelayedPoolSpawn>c__Iterator1BB.rot = rot;
			<DelayedPoolSpawn>c__Iterator1BB.<$>pool = pool;
			<DelayedPoolSpawn>c__Iterator1BB.<$>prefab = prefab;
			<DelayedPoolSpawn>c__Iterator1BB.<$>pos = pos;
			<DelayedPoolSpawn>c__Iterator1BB.<$>rot = rot;
			return <DelayedPoolSpawn>c__Iterator1BB;
		}

		[DebuggerHidden]
		private IEnumerator DelayedPoolSpawnMP(string pool, Transform prefab, Vector3 pos, Quaternion rot)
		{
			Prefabs.<DelayedPoolSpawnMP>c__Iterator1BC <DelayedPoolSpawnMP>c__Iterator1BC = new Prefabs.<DelayedPoolSpawnMP>c__Iterator1BC();
			<DelayedPoolSpawnMP>c__Iterator1BC.pool = pool;
			<DelayedPoolSpawnMP>c__Iterator1BC.prefab = prefab;
			<DelayedPoolSpawnMP>c__Iterator1BC.pos = pos;
			<DelayedPoolSpawnMP>c__Iterator1BC.rot = rot;
			<DelayedPoolSpawnMP>c__Iterator1BC.<$>pool = pool;
			<DelayedPoolSpawnMP>c__Iterator1BC.<$>prefab = prefab;
			<DelayedPoolSpawnMP>c__Iterator1BC.<$>pos = pos;
			<DelayedPoolSpawnMP>c__Iterator1BC.<$>rot = rot;
			return <DelayedPoolSpawnMP>c__Iterator1BC;
		}
	}
}
