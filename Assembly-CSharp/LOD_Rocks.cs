using PathologicalGames;
using System;
using UnityEngine;

public class LOD_Rocks : LOD_Base
{
	private static SpawnPool _spawnPool;

	public override LOD_Settings LodSettings
	{
		get
		{
			return LOD_Manager.Instance.Rocks;
		}
	}

	public override SpawnPool Pool
	{
		get
		{
			if (!LOD_Rocks._spawnPool)
			{
				LOD_Rocks._spawnPool = PoolManager.Pools["Rocks"];
			}
			return LOD_Rocks._spawnPool;
		}
	}

	public override void SetLOD(int lod)
	{
		bool flag = lod < this.currentLOD;
		base.SetLOD(lod);
		if (this.CurrentLodTransform)
		{
			this.CurrentLodTransform.SendMessage((!flag) ? "SkipStippling" : "ResetStippling", SendMessageOptions.DontRequireReceiver);
		}
	}
}
