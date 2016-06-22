using System;
using UnityEngine;

[Serializable]
public class TerrainNeighbors : MonoBehaviour
{
	public Terrain left;

	public Terrain top;

	public Terrain right;

	public Terrain bottom;

	public override void Start()
	{
		Terrain terrain = (Terrain)this.GetComponent(typeof(Terrain));
		terrain.SetNeighbors(this.left, this.top, this.right, this.bottom);
	}

	public override void Main()
	{
	}
}
