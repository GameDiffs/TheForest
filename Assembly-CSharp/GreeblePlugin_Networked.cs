using PathologicalGames;
using System;
using UnityEngine;

public static class GreeblePlugin_Networked
{
	private static SpawnPool Pool
	{
		get
		{
			return PoolManager.Pools["Greebles"];
		}
	}

	public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		return GreeblePlugin_Networked.Pool.Spawn(prefab.transform, position, rotation).gameObject;
	}

	public static void Destroy(GameObject instance)
	{
		if (instance == null)
		{
			return;
		}
		if (instance)
		{
			GreeblePlugin_Networked.Pool.Despawn(instance.transform);
		}
		else
		{
			GreeblePlugin_Networked.Pool.KillInstance(instance.transform);
		}
	}

	public static void Remove(GameObject instance)
	{
		if (instance)
		{
			GreeblePlugin_Networked.Pool.KillInstance(instance.transform);
		}
	}

	public static Vector3 GetCameraPosition()
	{
		return PlayerCamLocation.PlayerLoc;
	}
}
