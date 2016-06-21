using System;
using UnityEngine;

public class TerrainRenderQueue : MonoBehaviour
{
	public int queue = 1900;

	private void Start()
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain)
		{
			activeTerrain.materialTemplate.renderQueue = this.queue;
		}
	}
}
