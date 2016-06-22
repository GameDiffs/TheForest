using System;
using System.Collections.Generic;
using UnityEngine;

public class setupCreepySkin : MonoBehaviour
{
	private MaterialPropertyBlock bloodPropertyBlock;

	public SkinnedMeshRenderer skin;

	public bool explodedPrefab;

	public List<MeshRenderer> skinParts = new List<MeshRenderer>();

	private void setSkin(Material mat)
	{
		if (this.skin)
		{
			this.skin.sharedMaterial = mat;
		}
		if (this.explodedPrefab)
		{
			for (int i = 0; i < this.skinParts.Count; i++)
			{
				if (this.skinParts[i])
				{
					this.skinParts[i].material = mat;
				}
			}
		}
	}

	private void setSkinDamageProperty(MaterialPropertyBlock block)
	{
		if (this.skin)
		{
			this.skin.SetPropertyBlock(block);
		}
	}
}
