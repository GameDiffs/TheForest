using System;
using UnityEngine;

public class setupFishRagdoll : MonoBehaviour
{
	public int testFishInt;

	public Mesh[] mesh;

	public Material[] mat;

	public SkinnedMeshRenderer skin;

	private void Start()
	{
		if (this.testFishInt > 0)
		{
			this.doSkinSetup(this.testFishInt);
		}
	}

	private void doSkinSetup(int n)
	{
		if (this.skin)
		{
			this.skin.sharedMaterial = this.mat[n];
			this.skin.sharedMesh = this.mesh[n];
		}
	}
}
