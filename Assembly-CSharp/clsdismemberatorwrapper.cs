using System;
using UnityEngine;

[Serializable]
public class clsdismemberatorwrapper : MonoBehaviour
{
	public SkinnedMeshRenderer vargskm;

	public bool vargforcewrap;

	[HideInInspector]
	public Vector3[] propvertices;

	[HideInInspector]
	public Vector3[] propnormals;

	[HideInInspector]
	public Vector4[] proptangents;

	[HideInInspector]
	public Vector2[] propuvs;

	[HideInInspector]
	public BoneWeight[] propboneweights;

	[HideInInspector]
	public int[] proptriangles;

	[HideInInspector]
	public clssubmesher[] propsubmeshes;

	[HideInInspector]
	public Matrix4x4[] propbindposes;

	private Mesh varoriginalmesh;

	private void Awake()
	{
		if (this.vargskm != null)
		{
			this.varoriginalmesh = this.vargskm.sharedMesh;
			if (this.varoriginalmesh != null && (this.varoriginalmesh.triangles.Length != this.proptriangles.Length || this.vargforcewrap))
			{
				this.metrestorewrapper();
			}
		}
	}

	private void metrestorewrapper()
	{
		for (int i = 0; i < this.propsubmeshes.Length; i++)
		{
			this.vargskm.sharedMesh.SetTriangles(this.propsubmeshes[i].propsubmesh, i);
		}
	}
}
