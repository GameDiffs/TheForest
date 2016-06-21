using Pathfinding;
using PathologicalGames;
using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

public class AnimalSpawnController : MonoBehaviour
{
	public const float OVERALL_SPAWN_CHANCE_MODIFIER = 0.25f;

	public const float SPAWN_UPDATE_RATE = 2f;

	public const float DESPAWN_UPDATE_RATE = 2f;

	public const float PLAYER_SPAWN_DISTANCE_MIN = 30f;

	public const float PLAYER_SPAWN_DISTANCE_MAX = 60f;

	public const float PLAYER_PROXIMITY_DISTANCE_RATIO = 0.6f;

	public const float PLAYER_DESPAWN_DISTANCE = 70f;

	private sceneTracker sceneTracker;

	public static float lastUpdate;

	[HideInInspector]
	public AnimalSpawnZone[] Zones;

	private void OnEnable()
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
		this.sceneTracker = Scene.SceneTracker;
		AnimalSpawnController.lastUpdate = Time.time;
		this.Zones = base.GetComponentsInChildren<AnimalSpawnZone>();
		AnimalSpawnZone[] zones = this.Zones;
		for (int i = 0; i < zones.Length; i++)
		{
			AnimalSpawnZone animalSpawnZone = zones[i];
			animalSpawnZone.SpawnedAnimals = new List<GameObject>();
			for (int j = 0; j < animalSpawnZone.Spawns.Length; j++)
			{
				animalSpawnZone.TotalWeight += animalSpawnZone.Spawns[j].Weight;
				animalSpawnZone.Spawns[j].WeightRunningTotal = animalSpawnZone.TotalWeight;
			}
		}
	}

	private void Update()
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
		if (AnimalSpawnController.lastUpdate + 2f < Time.time)
		{
			List<Transform> list = new List<Transform>();
			foreach (GameObject current in this.sceneTracker.allPlayers)
			{
				if (current)
				{
					list.Add(current.transform);
				}
			}
			List<Transform> list2 = new List<Transform>();
			foreach (Transform current2 in list)
			{
				bool flag = false;
				foreach (Transform current3 in list2)
				{
					if ((current2.position - current3.position).magnitude < Mathf.Lerp(30f, 60f, 0.6f))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					list2.Add(current2);
				}
			}
			AnimalSpawnZone[] zones = this.Zones;
			for (int i = 0; i < zones.Length; i++)
			{
				AnimalSpawnZone animalSpawnZone = zones[i];
				if (animalSpawnZone.DelayUntil <= Time.realtimeSinceStartup)
				{
					if (list2.Count == 0)
					{
						break;
					}
					this.UpdateZone(animalSpawnZone, list2);
				}
			}
			AnimalSpawnController.lastUpdate = Time.time;
		}
	}

	private void UpdateZone(AnimalSpawnZone zone, List<Transform> positions)
	{
		try
		{
			for (int i = 0; i < positions.Count; i++)
			{
				if ((positions[i].position - zone.transform.position).sqrMagnitude < zone.ZoneRadius * zone.ZoneRadius)
				{
					this.SpawnOneAnimalForZone(zone, positions[i]);
					positions.RemoveAt(i);
					i--;
				}
			}
		}
		catch (Exception var_1_6B)
		{
		}
	}

	private void SpawnOneAnimalForZone(AnimalSpawnZone zone, Transform pos)
	{
		if (zone.SpawnedAnimals.Count >= zone.MaxAnimalsTotal)
		{
			return;
		}
		float num = Mathf.Clamp01((float)zone.SpawnedAnimals.Count / (float)zone.MaxAnimalsTotal) - 0.25f;
		if (num < UnityEngine.Random.value)
		{
			float num2 = UnityEngine.Random.Range(0f, zone.TotalWeight);
			for (int i = 0; i < zone.Spawns.Length; i++)
			{
				if (num2 <= zone.Spawns[i].WeightRunningTotal)
				{
					this.SpawnOneAnimalForZoneOfType(zone, zone.Spawns[i], pos);
					return;
				}
			}
		}
	}

	private void SpawnOneAnimalForZoneOfType(AnimalSpawnZone zone, AnimalSpawnConfig spawn, Transform basePos)
	{
		if (spawn.nocturnal && !Clock.Dark)
		{
			return;
		}
		float sendDir;
		if (spawn.largeAnimalType)
		{
			sendDir = -1f;
		}
		else
		{
			sendDir = 1f;
		}
		Vector3 pos;
		if (AnimalSpawnController.TryFindSpawnPosition(basePos, sendDir, out pos))
		{
			Transform transform = PoolManager.Pools["creatures"].Spawn(spawn.Prefab.transform, pos, zone.transform.rotation);
			if (!transform)
			{
				return;
			}
			if (transform)
			{
				transform.GetChild(0).eulerAngles = new Vector3(0f, (float)UnityEngine.Random.Range(0, 360), 0f);
			}
			if (transform)
			{
				transform.SendMessage("startUpdateSpawn");
			}
			if (transform)
			{
				transform.SendMessage("setSnowType", zone.snowType, SendMessageOptions.DontRequireReceiver);
			}
			zone.SpawnedAnimals.Add(transform.gameObject);
			AnimalSpawnController.AttachAnimalToNetwork(zone, spawn, transform.gameObject);
		}
	}

	public static void AttachAnimalToNetwork(AnimalSpawnZone zone, AnimalSpawnConfig spawn, GameObject gameObject)
	{
		AnimalDespawner animalDespawner = gameObject.GetComponent<AnimalDespawner>();
		if (!animalDespawner)
		{
			animalDespawner = gameObject.AddComponent<AnimalDespawner>();
		}
		animalDespawner.SpawnedFromZone = zone;
		animalDespawner.DespawnRadius = 70f;
		animalDespawner.UpdateRate = 1f;
		if (BoltNetwork.isServer)
		{
			BoltEntity boltEntity = gameObject.AddComponent<BoltEntity>();
			BoltEntity component = gameObject.GetComponent<CoopAnimalServer>().NetworkContainerPrefab.GetComponent<BoltEntity>();
			using (BoltEntitySettingsModifier boltEntitySettingsModifier = component.ModifySettings())
			{
				using (BoltEntitySettingsModifier boltEntitySettingsModifier2 = boltEntity.ModifySettings())
				{
					boltEntitySettingsModifier2.clientPredicted = boltEntitySettingsModifier.clientPredicted;
					boltEntitySettingsModifier2.persistThroughSceneLoads = boltEntitySettingsModifier.persistThroughSceneLoads;
					boltEntitySettingsModifier2.allowInstantiateOnClient = boltEntitySettingsModifier.allowInstantiateOnClient;
					boltEntitySettingsModifier2.prefabId = boltEntitySettingsModifier.prefabId;
					boltEntitySettingsModifier2.updateRate = boltEntitySettingsModifier.updateRate;
					boltEntitySettingsModifier2.serializerId = boltEntitySettingsModifier.serializerId;
				}
			}
			BoltNetwork.Attach(gameObject);
		}
	}

	public static Vector3 SpawnPos(Transform pos, float getDir)
	{
		Vector3 vector = Vector3.zero;
		vector = pos.forward * getDir;
		vector = Quaternion.Euler(0f, UnityEngine.Random.Range(-1f, 1f) * 100f, 0f) * vector;
		vector *= Mathf.Lerp(30f, 60f, UnityEngine.Random.value);
		return pos.position + vector;
	}

	public static bool TryFindSpawnPosition(Transform basePos, float sendDir, out Vector3 pos)
	{
		pos = Vector3.zero;
		for (int i = 0; i < 2; i++)
		{
			pos = AnimalSpawnController.SpawnPos(basePos, sendDir);
			pos.y += 100f;
			RaycastHit raycastHit;
			if (Physics.Raycast(pos, Vector3.down, out raycastHit, 300f, 67108864) && raycastHit.collider.CompareTag("TerrainMain") && AstarPath.active != null)
			{
				GraphNode node = AstarPath.active.GetNearest(raycastHit.point).node;
				if (node != null && node.Walkable)
				{
					pos = new Vector3((float)(node.position[0] / 1000), (float)(node.position[1] / 1000), (float)(node.position[2] / 1000));
					return true;
				}
			}
		}
		return false;
	}
}
