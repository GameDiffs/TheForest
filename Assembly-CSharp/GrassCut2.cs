using System;
using UnityEngine;

[ExecuteInEditMode]
public class GrassCut2 : MonoBehaviour
{
	private static Vector2 patch = default(Vector2);

	private static Vector3 size;

	private static Vector3 on;

	private static Vector3 inside;

	private int[,] detailLayer;

	private int detailRes;

	private int radio = 1;

	private void Start()
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		this.detailRes = activeTerrain.terrainData.detailResolution / 16;
		int num = activeTerrain.terrainData.detailPrototypes.Length;
		for (int i = 0; i < num; i++)
		{
			this.detailLayer = activeTerrain.terrainData.GetDetailLayer(0, 0, activeTerrain.terrainData.detailWidth, activeTerrain.terrainData.detailHeight, i);
		}
		GrassCut2.size = activeTerrain.terrainData.size / 16f;
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			MonoBehaviour.print("Something");
			Terrain activeTerrain = Terrain.activeTerrain;
			Vector3 a = base.transform.position - activeTerrain.transform.position;
			GrassCut2.patch.x = (float)((int)(a.x / GrassCut2.size.x));
			GrassCut2.patch.y = (float)((int)(a.z / GrassCut2.size.z));
			GrassCut2.on = GrassCut2.size;
			GrassCut2.on.x = GrassCut2.on.x * GrassCut2.patch.x;
			GrassCut2.on.z = GrassCut2.on.z * GrassCut2.patch.y;
			GrassCut2.inside = a - GrassCut2.on;
			float f = GrassCut2.inside.x * (float)this.detailRes / GrassCut2.size.x;
			float f2 = GrassCut2.inside.z * (float)this.detailRes / GrassCut2.size.z;
			int num = Mathf.RoundToInt(f);
			int num2 = Mathf.RoundToInt(f2);
			int num3 = (int)((float)this.detailRes * GrassCut2.patch.x) + num;
			int num4 = (int)((float)this.detailRes * GrassCut2.patch.y) + num2;
			int num5 = Mathf.Max(0, num3 - this.radio);
			int num6 = Mathf.Max(0, num4 - this.radio);
			int num7 = Mathf.Min(activeTerrain.terrainData.detailWidth, num3 + this.radio);
			int num8 = Mathf.Min(activeTerrain.terrainData.detailHeight, num4 + this.radio);
			Debug.Log(string.Concat(new object[]
			{
				"sr:",
				num3,
				" r:",
				num5,
				" to ",
				num7,
				" sc:",
				num4,
				" c:",
				num6,
				" to ",
				num8
			}));
			for (int i = num5; i < num7; i++)
			{
				for (int j = num6; j < num8; j++)
				{
					this.detailLayer[j, i] = 0;
				}
			}
			activeTerrain.terrainData.SetDetailLayer(0, 0, 0, this.detailLayer);
		}
	}
}
