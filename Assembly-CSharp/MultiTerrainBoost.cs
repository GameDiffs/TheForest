using System;
using UnityEngine;

public class MultiTerrainBoost : MonoBehaviour
{
	private Camera MainCamera;

	private bool[] active1;

	private Terrain[] terrains;

	private Bounds[] bounds;

	private Plane[] planes;

	private float distance;

	private int count_terrain;

	private void Start()
	{
		this.MainCamera = base.GetComponent<Camera>();
		this.terrains = (Resources.FindObjectsOfTypeAll(typeof(Terrain)) as Terrain[]);
		this.bounds = new Bounds[this.terrains.Length];
		this.active1 = new bool[this.terrains.Length];
		this.calcBounds();
	}

	private void LateUpdate()
	{
		this.calcFrustrum();
		this.count_terrain = 0;
		while (this.count_terrain < this.bounds.Length)
		{
			if (this.IsRenderedFrom(this.bounds[this.count_terrain]))
			{
				if (!this.active1[this.count_terrain])
				{
					this.terrains[this.count_terrain].enabled = true;
					this.active1[this.count_terrain] = true;
				}
			}
			else if (this.active1[this.count_terrain])
			{
				this.terrains[this.count_terrain].enabled = false;
				this.active1[this.count_terrain] = false;
			}
			this.count_terrain++;
		}
	}

	private void calcBounds()
	{
		this.count_terrain = 0;
		while (this.count_terrain < this.terrains.Length)
		{
			this.bounds[this.count_terrain].size = this.terrains[this.count_terrain].terrainData.size;
			this.bounds[this.count_terrain].center = new Vector3(this.terrains[this.count_terrain].transform.position.x + this.bounds[this.count_terrain].size.x / 2f, this.terrains[this.count_terrain].transform.position.y + this.bounds[this.count_terrain].size.y / 2f, this.terrains[this.count_terrain].transform.position.z + this.bounds[this.count_terrain].size.z / 2f);
			this.active1[this.count_terrain] = true;
			this.count_terrain++;
		}
	}

	private void calcFrustrum()
	{
		this.planes = GeometryUtility.CalculateFrustumPlanes(this.MainCamera);
	}

	private bool IsRenderedFrom(Bounds bound)
	{
		return GeometryUtility.TestPlanesAABB(this.planes, bound);
	}
}
