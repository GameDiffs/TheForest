using System;
using UnityEngine;

public class SwitchMesh : MonoBehaviour
{
	public Mesh mesh;

	private void OnMouseDown()
	{
		base.GetComponent<MeshFilter>().mesh = this.mesh;
	}
}
