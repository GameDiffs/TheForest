using PathologicalGames;
using System;
using UnityEngine;

public static class GreeblePlugin
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
		Transform transform = GreeblePlugin.Pool.Spawn(prefab.transform, position, rotation);
		return (!transform) ? null : transform.gameObject;
	}

	public static void Destroy(GameObject instance)
	{
		if (instance)
		{
			GreeblePlugin.Pool.Despawn(instance.transform);
		}
	}

	public static void Remove(GameObject instance)
	{
		if (instance)
		{
			GreeblePlugin.Pool.KillInstance(instance.transform);
		}
	}

	public static Vector3 GetCameraPosition()
	{
		return PlayerCamLocation.PlayerLoc;
	}
}
