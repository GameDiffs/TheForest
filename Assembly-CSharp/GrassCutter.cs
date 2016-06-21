using System;
using UnityEngine;

[ExecuteInEditMode]
public class GrassCutter : MonoBehaviour
{
	private int detailRes;

	private int radio = 5;

	public void Update()
	{
		Terrain activeTerrain = Terrain.activeTerrain;
		this.detailRes = activeTerrain.terrainData.detailResolution / 16;
		int num = activeTerrain.terrainData.detailPrototypes.Length;
		int[,] array = null;
		for (int i = 0; i < num; i++)
		{
			array = activeTerrain.terrainData.GetDetailLayer(0, 0, activeTerrain.terrainData.detailWidth, activeTerrain.terrainData.detailHeight, i);
		}
		Vector3 vector = activeTerrain.terrainData.size / 16f;
		Vector3 a = base.transform.position - activeTerrain.transform.position;
		Vector2 vector2 = new Vector2((float)((int)(a.x / vector.x)), (float)((int)a.z) / vector.z);
		Vector3 b = vector;
		b.x *= vector2.x;
		b.z *= vector2.y;
		Vector3 vector3 = a - b;
		float f = vector3.x * (float)this.detailRes / vector.x;
		float f2 = vector3.z * (float)this.detailRes / vector.z;
		int num2 = Mathf.RoundToInt(f);
		int num3 = Mathf.RoundToInt(f2);
		int num4 = (int)((float)this.detailRes * vector2.x) + num2;
		int num5 = (int)((float)this.detailRes * vector2.y) + num3;
		int num6 = Mathf.Max(0, num4 - this.radio);
		int num7 = Mathf.Max(0, num5 - this.radio);
		int num8 = Mathf.Min(activeTerrain.terrainData.detailWidth, num4 + this.radio);
		int num9 = Mathf.Min(activeTerrain.terrainData.detailHeight, num5 + this.radio);
		if (Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log(string.Concat(new object[]
			{
				"sr:",
				num4,
				" r:",
				num6,
				" to ",
				num8,
				" sc:",
				num5,
				" c:",
				num7,
				" to ",
				num9
			}));
			for (int j = num6; j < num8; j++)
			{
				for (int k = num7; k < num9; k++)
				{
					array[k, j] = 0;
				}
			}
			activeTerrain.terrainData.SetDetailLayer(0, 0, 0, array);
		}
	}
}
