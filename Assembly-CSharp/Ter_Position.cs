using System;
using UnityEngine;

public class Ter_Position : MonoBehaviour
{
	public Terrain MyTerrain;

	public Vector3 worldPos;

	public GUIText displayPosition;

	public GUIText displayPosition2;

	public int Amount;

	public int posX;

	public int posY;

	public float posX2;

	public float posY2;

	public Vector3 terrrPos;

	private void Start()
	{
		this.worldPos = base.transform.position;
	}

	private void Update()
	{
		this.worldPos = base.transform.position;
		Vector3 vector = this.worldPos - this.MyTerrain.transform.position;
		float x = Mathf.InverseLerp(0f, this.MyTerrain.terrainData.size.x, vector.x);
		float y = Mathf.InverseLerp(0f, this.MyTerrain.terrainData.size.z, vector.z);
		Vector2 vector2 = new Vector2(x, y);
		Vector2 vector3 = this.MyTerrain.terrainData.GetInterpolatedNormal(vector2.x, vector2.y);
		this.displayPosition.text = string.Concat(new object[]
		{
			"Pezz Terrain Pos: ",
			vector3,
			" ",
			vector2.x,
			" ",
			vector2.y
		});
		float num = base.transform.position.x - this.MyTerrain.transform.position.x;
		float num2 = base.transform.position.z - this.MyTerrain.transform.position.z;
		float num3 = num2 / this.MyTerrain.terrainData.size.z;
		float num4 = num / this.MyTerrain.terrainData.size.x;
		float num5 = num4 * (float)this.MyTerrain.terrainData.heightmapWidth;
		float num6 = num3 * (float)this.MyTerrain.terrainData.heightmapHeight;
		this.displayPosition2.text = string.Concat(new object[]
		{
			"Pezz Terrain Pos2: ",
			vector3,
			" ",
			num5,
			" ",
			num6
		});
		this.posX2 = (base.transform.position.x - this.MyTerrain.transform.position.x) / this.MyTerrain.terrainData.size.x * (float)this.MyTerrain.terrainData.heightmapWidth;
		this.posY2 = (base.transform.position.z - this.MyTerrain.transform.position.z) / this.MyTerrain.terrainData.size.z * (float)this.MyTerrain.terrainData.heightmapHeight;
		this.posX = (int)this.posX2;
		this.posY = (int)this.posY2;
	}
}
