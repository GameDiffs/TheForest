using PathologicalGames;
using System;

public class LOD_Creature : LOD_Base
{
	private static SpawnPool _spawnPool;

	public override LOD_Settings LodSettings
	{
		get
		{
			return LOD_Manager.Instance.Creature;
		}
	}

	public override SpawnPool Pool
	{
		get
		{
			if (!LOD_Creature._spawnPool)
			{
				LOD_Creature._spawnPool = PoolManager.Pools["Creatures"];
			}
			return LOD_Creature._spawnPool;
		}
	}
}
