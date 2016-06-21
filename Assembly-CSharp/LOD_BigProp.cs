using FMOD.Studio;
using PathologicalGames;
using System;

public class LOD_BigProp : LOD_Base
{
	public override LOD_Settings LodSettings
	{
		get
		{
			return LOD_Manager.Instance.BigProp;
		}
	}

	public override SpawnPool Pool
	{
		get
		{
			return null;
		}
	}

	public override void SetLOD(int lod)
	{
		EventInstance windEvent = TreeWindSfx.BeginTransfer(this.CurrentLodTransform);
		base.SetLOD(lod);
		TreeWindSfx.CompleteTransfer(this.CurrentLodTransform, windEvent);
	}
}
