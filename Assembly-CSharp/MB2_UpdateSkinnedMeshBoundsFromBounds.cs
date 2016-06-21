using DigitalOpus.MB.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MB2_UpdateSkinnedMeshBoundsFromBounds : MonoBehaviour
{
	public List<GameObject> objects;

	private SkinnedMeshRenderer smr;

	private void Start()
	{
		this.smr = base.GetComponent<SkinnedMeshRenderer>();
		if (this.smr == null)
		{
			Debug.LogError("Need to attach MB2_UpdateSkinnedMeshBoundsFromBounds script to an object with a SkinnedMeshRenderer component attached.");
			return;
		}
		if (this.objects == null || this.objects.Count == 0)
		{
			Debug.LogWarning("The MB2_UpdateSkinnedMeshBoundsFromBounds had no Game Objects. It should have the same list of game objects that the MeshBaker does.");
			this.smr = null;
			return;
		}
		for (int i = 0; i < this.objects.Count; i++)
		{
			if (this.objects[i] == null || this.objects[i].GetComponent<Renderer>() == null)
			{
				Debug.LogError("The list of objects had nulls or game objects without a renderer attached at position " + i);
				this.smr = null;
				return;
			}
		}
		this.smr.updateWhenOffscreen = true;
		this.smr.updateWhenOffscreen = false;
	}

	private void Update()
	{
		if (this.smr != null && this.objects != null)
		{
			MB3_MeshCombiner.UpdateSkinnedMeshApproximateBoundsFromBoundsStatic(this.objects, this.smr);
		}
	}
}
