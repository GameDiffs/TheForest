using System;
using UnityEngine;

public class StoreMesh : MonoBehaviour
{
	[HideInInspector]
	public Vector3[] vertices;

	[HideInInspector]
	public Vector3[] normals;

	[HideInInspector]
	public Vector2[] uv;

	[HideInInspector]
	public Vector2[] uv1;

	[HideInInspector]
	public Vector2[] uv2;

	[HideInInspector]
	public Color[] colors;

	[HideInInspector]
	public int[][] triangles;

	[HideInInspector]
	public Vector4[] tangents;

	[HideInInspector]
	public int subMeshCount;

	private MeshFilter filter;

	private SkinnedMeshRenderer skinnedMeshRenderer;

	private void Awake()
	{
		this.filter = base.GetComponent<MeshFilter>();
		this.skinnedMeshRenderer = base.GetComponent<SkinnedMeshRenderer>();
		if (this.filter == null && this.skinnedMeshRenderer == null)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void OnSerializing()
	{
		Mesh mesh = (!(this.filter != null)) ? this.skinnedMeshRenderer.sharedMesh : this.filter.mesh;
		this.vertices = mesh.vertices;
		this.normals = mesh.normals;
		this.uv = mesh.uv;
		this.uv1 = mesh.uv2;
		this.uv2 = mesh.uv2;
		this.colors = mesh.colors;
		this.triangles = new int[this.subMeshCount = mesh.subMeshCount][];
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			this.triangles[i] = mesh.GetTriangles(i);
		}
		this.tangents = mesh.tangents;
	}

	private void OnDeserialized()
	{
		Mesh mesh = new Mesh();
		mesh.vertices = this.vertices;
		mesh.normals = this.normals;
		mesh.uv = this.uv;
		mesh.uv2 = this.uv1;
		mesh.uv2 = this.uv2;
		mesh.colors = this.colors;
		mesh.tangents = this.tangents;
		mesh.subMeshCount = this.subMeshCount;
		for (int i = 0; i < this.subMeshCount; i++)
		{
			mesh.SetTriangles(this.triangles[i], i);
		}
		mesh.RecalculateBounds();
		if (this.filter != null)
		{
			this.filter.mesh = mesh;
		}
		else
		{
			this.skinnedMeshRenderer.sharedMesh = mesh;
		}
	}
}
