using System;
using UnityEngine;

public class animalSkinSetup : MonoBehaviour
{
	public SkinnedMeshRenderer skin;

	private void setSkin(Material mat)
	{
		this.skin.sharedMaterial = mat;
	}
}
