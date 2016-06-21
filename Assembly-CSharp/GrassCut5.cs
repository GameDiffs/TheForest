using System;
using UnityEngine;

[ExecuteInEditMode]
public class GrassCut5 : MonoBehaviour
{
	protected class Layer
	{
		public int[,] detailLayer;
	}

	private static Vector2 patch = default(Vector2);

	private static Vector3 size;

	private static Vector3 on;

	private static Vector3 inside;

	private GrassCut5.Layer[] layers;

	private int detailRes;

	private int radio;

	private int numDetails;

	private int ammount = 1;

	private void Start()
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		this.detailRes = activeTerrain.terrainData.detailResolution / 16;
		this.numDetails = activeTerrain.terrainData.detailPrototypes.Length;
		this.layers = new GrassCut5.Layer[this.numDetails];
		for (int i = 0; i < this.numDetails; i++)
		{
			int[,] detailLayer = activeTerrain.terrainData.GetDetailLayer(0, 0, activeTerrain.terrainData.detailWidth, activeTerrain.terrainData.detailHeight, i);
			this.layers[i] = new GrassCut5.Layer();
			this.layers[i].detailLayer = detailLayer;
		}
		GrassCut5.size = activeTerrain.terrainData.size / 16f;
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			MonoBehaviour.print("TryingtoCut");
			Terrain activeTerrain = Terrain.activeTerrain;
			Vector3 a = base.transform.position - activeTerrain.transform.position;
			GrassCut5.patch.x = (float)((int)(a.x / GrassCut5.size.x));
			GrassCut5.patch.y = (float)((int)(a.z / GrassCut5.size.z));
			GrassCut5.on = GrassCut5.size;
			GrassCut5.on.x = GrassCut5.on.x * GrassCut5.patch.x;
			GrassCut5.on.z = GrassCut5.on.z * GrassCut5.patch.y;
			GrassCut5.inside = a - GrassCut5.on;
			float f = GrassCut5.inside.x * (float)this.detailRes / GrassCut5.size.x;
			float f2 = GrassCut5.inside.z * (float)this.detailRes / GrassCut5.size.z;
			int num = Mathf.RoundToInt(f);
			int num2 = Mathf.RoundToInt(f2);
			int num3 = (int)((float)this.detailRes * GrassCut5.patch.x) + num;
			int num4 = (int)((float)this.detailRes * GrassCut5.patch.y) + num2;
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
}
