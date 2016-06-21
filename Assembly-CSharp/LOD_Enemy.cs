using PathologicalGames;
using System;

public class LOD_Enemy : LOD_Base
{
	private static SpawnPool _spawnPool;

	public override LOD_Settings LodSettings
	{
		get
		{
			return LOD_Manager.Instance.Enemy;
		}
	}

	public override SpawnPool Pool
	{
		get
		{
			if (!LOD_Enemy._spawnPool)
			{
				LOD_Enemy._spawnPool = PoolManager.Pools["Enemies"];
			}
			return LOD_Enemy._spawnPool;
		}
	}
}
