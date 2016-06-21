using System;

public class LOD_CaveEntrance : LOD_Cave
{
	public override LOD_Settings LodSettings
	{
		get
		{
			return LOD_Manager.Instance.CaveEntrance;
		}
	}
}
