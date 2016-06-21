using System;
using UnityEngine;

[ExecuteInEditMode]
public class GrassCut4 : MonoBehaviour
{
	protected class Layer
	{
		public int[,] detailLayer;
	}

	private static Vector2 patch = default(Vector2);

	private static Vector3 size;

	private static Vector3 on;

	private static Vector3 inside;

	private GrassCut4.Layer[] layers;

	private int detailRes;

	private int radio = 1;

	private int numDetails;

	private int ammount = 1;

	private void Start()
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		this.detailRes = activeTerrain.terrainData.detailResolution / 16;
		this.numDetails = activeTerrain.terrainData.detailPrototypes.Length;
		this.layers = new GrassCut4.Layer[this.numDetails];
		for (int i = 0; i < this.numDetails; i++)
		{
			int[,] detailLayer = activeTerrain.terrainData.GetDetailLayer(0, 0, activeTerrain.terrainData.detailWidth, activeTerrain.terrainData.detailHeight, i);
			this.layers[i] = new GrassCut4.Layer();
			this.layers[i].detailLayer = detailLayer;
		}
		GrassCut4.size = activeTerrain.terrainData.size / 16f;
	}

	public void Update()
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		Vector3 a = base.transform.position - activeTerrain.transform.position;
		GrassCut4.patch.x = (float)((int)(a.x / GrassCut4.size.x));
		GrassCut4.patch.y = (float)((int)(a.z / GrassCut4.size.z));
		GrassCut4.on = GrassCut4.size;
		GrassCut4.on.x = GrassCut4.on.x * GrassCut4.patch.x;
		GrassCut4.on.z = GrassCut4.on.z * GrassCut4.patch.y;
		GrassCut4.inside = a - GrassCut4.on;
		float f = GrassCut4.inside.x * (float)this.detailRes / GrassCut4.size.x;
		float f2 = GrassCut4.inside.z * (float)this.detailRes / GrassCut4.size.z;
		int num = Mathf.RoundToInt(f);
		int num2 = Mathf.RoundToInt(f2);
		int num3 = (int)((float)this.detailRes * GrassCut4.patch.x) + num;
		int num4 = (int)((float)this.detailRes * GrassCut4.patch.y) + num2;
		int num5 = Mathf.Max(0, num3 - this.radio);
		int num6 = Mathf.Max(0, num4 - this.radio);
		int num7 = Mathf.Min(activeTerrain.terrainData.detailWidth, num3 + this.radio);
		int num8 = Mathf.Min(activeTerrain.terrainData.detailHeight, num4 + this.radio);
		for (int i = num5; i <= num7; i++)
		{
			for (int j = num6; j <= num8; j++)
			{
				for (int k = 0; k < this.numDetails; k++)
				{
					int num9 = this.layers[k].detailLayer[j, i];
					this.layers[k].detailLayer[j, i] = Mathf.Max(0, num9 - this.ammount);
				}
			}
		}
		for (int l = 0; l < this.numDetails; l++)
		{
			activeTerrain.terrainData.SetDetailLayer(0, 0, l, this.layers[l].detailLayer);
		}
	}
}
