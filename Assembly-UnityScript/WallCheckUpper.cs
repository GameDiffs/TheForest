using System;
using UnityEngine;

[Serializable]
public class WallCheckUpper : MonoBehaviour
{
	public GameObject SideWalls;

	public override void Start()
	{
		if (this.transform.position.y > Terrain.activeTerrain.SampleHeight(this.transform.position) + (float)3)
		{
			UnityEngine.Object.Destroy(this.SideWalls);
		}
	}

	public override void Main()
	{
	}
}
