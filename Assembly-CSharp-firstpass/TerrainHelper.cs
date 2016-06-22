using System;
using UnityEngine;

public static class TerrainHelper
{
	public static int GetProminantTextureIndex(Vector3 worldPosition)
	{
		return TerrainHelper.GetProminantTextureIndex(Terrain.activeTerrain, worldPosition);
	}

	public static int GetProminantTextureIndex(Terrain terrain, Vector3 worldPosition)
	{
		if (!terrain)
		{
			return -1;
		}
		Vector3 position = terrain.GetPosition();
		TerrainData terrainData = terrain.terrainData;
		int alphamapResolution = terrainData.alphamapResolution;
		int num = (int)((worldPosition.x - position.x) / terrainData.size.x * (float)alphamapResolution);
		int num2 = (int)((worldPosition.z - position.z) / terrainData.size.z * (float)alphamapResolution);
		if (num < 0 || num2 < 0 || num >= alphamapResolution || num2 >= alphamapResolution)
		{
			return -1;
		}
		float[,,] alphamaps = terrain.terrainData.GetAlphamaps(num, num2, 1, 1);
		int alphamapLayers = terrain.terrainData.alphamapLayers;
		int result = -1;
		float num3 = 0f;
		for (int i = 0; i < alphamapLayers; i++)
		{
			if (alphamaps[0, 0, i] > num3)
			{
				result = i;
				num3 = alphamaps[0, 0, i];
			}
		}
		return result;
	}
}
