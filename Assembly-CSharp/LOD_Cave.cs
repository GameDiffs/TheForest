using PathologicalGames;
using System;

public class LOD_Cave : LOD_Base
{
	private static SpawnPool _spawnPool;

	public override LOD_Settings LodSettings
	{
		get
		{
			return LOD_Manager.Instance.Cave;
		}
	}

	public override SpawnPool Pool
	{
		get
		{
			if (!LOD_Cave._spawnPool)
			{
				LOD_Cave._spawnPool = PoolManager.Pools["Caves"];
			}
			return LOD_Cave._spawnPool;
		}
	}

	public override void SetLOD(int lod)
	{
		base.SetLOD(lod);
		if (this.CurrentLodTransform != null)
		{
			this.CurrentLodTransform.localScale = base.transform.lossyScale;
		}
	}
}
