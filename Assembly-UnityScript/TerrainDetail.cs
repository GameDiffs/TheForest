using System;
using UnityEngine;

[Serializable]
public class TerrainDetail : MonoBehaviour
{
	public int heightmapMaximumLOD;

	public float heightmapPixelError;

	public float basemapDistance;

	public bool castShadows;

	public bool draw;

	public float treeDistance;

	public float detailObjectDistance;

	public float detailObjectDensity;

	public float treeBillboardDistance;

	public float treeCrossFadeLength;

	public int treeMaximumFullLODCount;

	public TerrainDetail()
	{
		this.heightmapPixelError = (float)5;
		this.basemapDistance = (float)5000;
		this.draw = true;
		this.treeDistance = (float)20000;
		this.detailObjectDistance = (float)250;
		this.detailObjectDensity = (float)1;
		this.treeBillboardDistance = (float)200;
		this.treeCrossFadeLength = (float)50;
		this.treeMaximumFullLODCount = 50;
	}

	public override void Start()
	{
		Terrain terrain = (Terrain)this.GetComponent(typeof(Terrain));
		terrain.heightmapPixelError = this.heightmapPixelError;
		terrain.heightmapMaximumLOD = this.heightmapMaximumLOD;
		terrain.basemapDistance = this.basemapDistance;
		terrain.castShadows = this.castShadows;
		if (this.draw)
		{
			terrain.treeDistance = this.treeDistance;
			terrain.detailObjectDistance = this.detailObjectDistance;
		}
		else
		{
			terrain.treeDistance = (float)0;
			terrain.detailObjectDistance = (float)0;
		}
		terrain.detailObjectDensity = this.detailObjectDensity;
		terrain.treeMaximumFullLODCount = this.treeMaximumFullLODCount;
		terrain.treeBillboardDistance = this.treeBillboardDistance;
		terrain.treeCrossFadeLength = this.treeCrossFadeLength;
		terrain.treeMaximumFullLODCount = this.treeMaximumFullLODCount;
	}

	public override void Main()
	{
	}
}
