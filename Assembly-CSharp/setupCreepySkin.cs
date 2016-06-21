using System;
using UnityEngine;

public class setupCreepySkin : MonoBehaviour
{
	private MaterialPropertyBlock bloodPropertyBlock;

	public SkinnedMeshRenderer skin;

	private void setSkin(Material mat)
	{
		if (this.skin)
		{
			this.skin.sharedMaterial = mat;
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
