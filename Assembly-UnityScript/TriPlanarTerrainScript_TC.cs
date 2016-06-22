using System;
using UnityEngine;

[Serializable]
public class TriPlanarTerrainScript_TC : MonoBehaviour
{
	public bool TriPlanar;

	public Texture2D[] bumpTextures;

	public Texture2D[] specTextures;

	public float[] tilesPerMeter;

	public float[] tilesPerMeterNormal;

	public float[] Strength;

	public float[] StrengthSplat;

	public TerrainData terDat;

	public override void setTerrainValues()
	{
	}

	public override void Main()
	{
	}
}
