using System;
using UnityEngine;

namespace PathologicalGames
{
	public static class InstanceManager
	{
		public static string poolName = "Projectiles";

		public static Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
		{
			return (Transform)UnityEngine.Object.Instantiate(prefab, pos, rot);
		}

		public static void Despawn(Transform instance)
		{
			UnityEngine.Object.Destroy(instance.gameObject);
		}
	}
}
