using PathologicalGames;
using System;

public class LOD_PickUps : LOD_Base
{
	private static SpawnPool _spawnPool;

	public override LOD_Settings LodSettings
	{
		get
		{
			return LOD_Manager.Instance.PickUps;
		}
	}

	public override SpawnPool Pool
	{
		get
		{
			if (!LOD_PickUps._spawnPool)
			{
				LOD_PickUps._spawnPool = PoolManager.Pools["PickUps"];
			}
			return LOD_PickUps._spawnPool;
		}
	}

	public override bool DestroyInsteadOfDisable
	{
		get
		{
			return true;
		}
	}
}
