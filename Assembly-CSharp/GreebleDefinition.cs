using System;
using UnityEngine;

[Serializable]
public class GreebleDefinition
{
	public GameObject Prefab;

	[Range(1f, 100f)]
	public int Chance = 1;

	public float RespawnTime;

	public float MinAgeMinutes;

	public float MaxAgeMinutes;

	public LayerMask SurfaceMask = -1;

	public LayerMask KillMask = 0;

	public int[] TerrainTextureMask;

	public bool MatchSurfaceNormal = true;

	public bool RandomizeRotation = true;

	public bool AllowRotationX;

	public bool AllowRotationY = true;

	public bool AllowRotationZ;

	public bool HasCustomActiveValue;

	public bool IsAgeAppropriate(float ageInMinutes)
	{
		return this.MaxAgeMinutes == 0f || ageInMinutes <= 0f || (ageInMinutes >= this.MinAgeMinutes && (ageInMinutes < this.MaxAgeMinutes || this.MaxAgeMinutes < 0f));
	}
}
