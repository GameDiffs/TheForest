using FMOD.Studio;
using PathologicalGames;
using System;
using UnityEngine;

public class LOD_Trees : LOD_Base
{
	public GameObject StumpPrefab;

	protected CustomBillboard hcb;

	protected int hBillboardId = -1;

	private static SpawnPool _spawnPool;

	public override SpawnPool Pool
	{
		get
		{
			if (!LOD_Trees._spawnPool)
			{
				LOD_Trees._spawnPool = PoolManager.Pools["Trees"];
			}
			return LOD_Trees._spawnPool;
		}
	}

	public override LOD_Settings LodSettings
	{
		get
		{
			return LOD_Manager.Instance.Trees;
		}
	}

	public TreeHealth CurrentView
	{
		get;
		set;
	}

	public GameObject OnTreeCutDownTarget
	{
		get;
		set;
	}

	public override void SetLOD(int lod)
	{
		EventInstance windEvent = TreeWindSfx.BeginTransfer(this.CurrentLodTransform);
		base.SetLOD(lod);
		TreeWindSfx.CompleteTransfer(this.CurrentLodTransform, windEvent);
	}
}
