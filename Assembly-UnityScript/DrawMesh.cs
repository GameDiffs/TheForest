using System;
using UnityEngine;

[Serializable]
public class DrawMesh : MonoBehaviour
{
	public Mesh aMesh;

	public Material aMaterial;

	public override void Update()
	{
		Graphics.DrawMesh(this.aMesh, this.transform.position, this.transform.rotation, this.aMaterial, 0);
	}

	public override void Main()
	{
	}
}
