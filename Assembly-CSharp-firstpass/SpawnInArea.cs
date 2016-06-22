using System;
using UnityEngine;

public class SpawnInArea : MonoBehaviour
{
	public Texture2D SpawnMap;

	private float Offset = 10f;

	private float AboveGround = 1f;

	private bool TerrainOnly = true;

	private void RandomPositionOnTerrain(GameObject obj)
	{
		Vector3 size = Terrain.activeTerrain.terrainData.size;
		Vector3 vector = default(Vector3);
		bool flag = false;
		while (!flag)
		{
			vector = Terrain.activeTerrain.transform.position;
			float num = UnityEngine.Random.Range(0f, size.x);
			float num2 = UnityEngine.Random.Range(0f, size.z);
			vector.x += num;
			vector.y += size.y + this.Offset;
			vector.z += num2;
			if (this.SpawnMap)
			{
				int x = Mathf.RoundToInt((float)this.SpawnMap.width * num / size.x);
				int y = Mathf.RoundToInt((float)this.SpawnMap.height * num2 / size.z);
				float grayscale = this.SpawnMap.GetPixel(x, y).grayscale;
				flag = (grayscale > 0f && UnityEngine.Random.Range(0f, 1f) < grayscale);
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				RaycastHit raycastHit;
				if (Physics.Raycast(vector, -Vector3.up, out raycastHit))
				{
					float distance = raycastHit.distance;
					if (raycastHit.transform.name != "Terrain" && this.TerrainOnly)
					{
						flag = false;
					}
					vector.y -= distance - this.AboveGround;
				}
				else
				{
					flag = false;
				}
			}
		}
		obj.transform.position = vector;
		base.transform.Rotate(Vector3.up * (float)UnityEngine.Random.Range(0, 360), Space.World);
	}
}
