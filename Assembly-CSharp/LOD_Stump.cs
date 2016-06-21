using System;
using UnityEngine;

public class LOD_Stump : LOD_Trees
{
	public override void SetLOD(int lod)
	{
		if (this.CurrentLodTransform)
		{
			this.CurrentLodTransform.transform.parent = null;
		}
		base.SetLOD(lod);
		if (this.CurrentLodTransform)
		{
			this.CurrentLodTransform.transform.parent = base.transform;
			this.CurrentLodTransform.transform.localScale = Vector3.one;
		}
	}
}
