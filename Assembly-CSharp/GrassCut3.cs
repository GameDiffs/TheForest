using System;
using UnityEngine;

[ExecuteInEditMode]
public class GrassCut3 : MonoBehaviour
{
	private static Vector2 patch = default(Vector2);

	private static Vector3 size;

	private static Vector3 on;

	private static Vector3 inside;

	private int[,] detailLayer;

	private int detailRes;

	private int radio;

	private void Start()
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		this.detailRes = activeTerrain.terrainData.detailResolution / 16;
		int num = activeTerrain.terrainData.detailPrototypes.Length;
		for (int i = 0; i < num; i++)
		{
			this.detailLayer = activeTerrain.terrainData.GetDetailLayer(0, 0, activeTerrain.terrainData.detailWidth, activeTerrain.terrainData.detailHeight, i);
		}
		GrassCut3.size = activeTerrain.terrainData.size / 16f;
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Terrain activeTerrain = Terrain.activeTerrain;
			Vector3 a = base.transform.position - activeTerrain.transform.position;
			GrassCut3.patch.x = (float)((int)(a.x / GrassCut3.size.x));
			GrassCut3.patch.y = (float)((int)(a.z / GrassCut3.size.z));
			GrassCut3.on = GrassCut3.size;
			GrassCut3.on.x = GrassCut3.on.x * GrassCut3.patch.x;
			GrassCut3.on.z = GrassCut3.on.z * GrassCut3.patch.y;
			GrassCut3.inside = a - GrassCut3.on;
			float f = GrassCut3.inside.x * (float)this.detailRes / GrassCut3.size.x;
			float f2 = GrassCut3.inside.z * (float)this.detailRes / GrassCut3.size.z;
			int num = Mathf.RoundToInt(f);
			int num2 = Mathf.RoundToInt(f2);
			int num3 = (int)((float)this.detailRes * GrassCut3.patch.x) + num;
			int num4 = (int)((float)this.detailRes * GrassCut3.patch.y) + num2;
			int num5 = Mathf.Max(0, num3 - this.radio);
			int num6 = Mathf.Max(0, num4 - this.radio);
			int num7 = Mathf.Min(activeTerrain.terrainData.detailWidth, num3 + this.radio);
			int num8 = Mathf.Min(activeTerrain.terrainData.detailHeight, num4 + this.radio);
			for (int i = num5; i <= num7; i++)
			{
				for (int j = num6; j <= num8; j++)
				{
					this.detailLayer[j, i] = 0;
				}
			}
			activeTerrain.terrainData.SetDetailLayer(0, 0, 0, this.detailLayer);
		}
	}
}
