using System;
using UnityEngine;

public class TerrainDetailPatches : MonoBehaviour
{
	private void Start()
	{
		Terrain.activeTerrain.collectDetailPatches = false;
	}

	private void Update()
	{
	}
}
