using PathologicalGames;
using System;
using UnityEngine;

public class spawnTimeout : MonoBehaviour
{
	public float despawnTime;

	public void invokeDespawn()
	{
		if (this.despawnTime > 0f)
		{
			base.Invoke("doDespawn", this.despawnTime);
		}
	}

	private void doDespawn()
	{
		if (PoolManager.Pools["misc"].IsSpawned(base.transform))
		{
			PoolManager.Pools["misc"].Despawn(base.transform);
		}
	}
}
