using System;
using System.Collections.Generic;
using UnityEngine;

public class MB3_DisableHiddenAnimations : MonoBehaviour
{
	public List<Animation> animationsToCull = new List<Animation>();

	private void Start()
	{
		if (base.GetComponent<SkinnedMeshRenderer>() == null)
		{
			Debug.LogError("The MB3_CullHiddenAnimations script was placed on and object " + base.name + " which has no SkinnedMeshRenderer attached");
		}
	}

	private void OnBecameVisible()
	{
		for (int i = 0; i < this.animationsToCull.Count; i++)
		{
			if (this.animationsToCull[i] != null)
			{
				this.animationsToCull[i].enabled = true;
			}
		}
	}

	private void OnBecameInvisible()
	{
		for (int i = 0; i < this.animationsToCull.Count; i++)
		{
			if (this.animationsToCull[i] != null)
			{
				this.animationsToCull[i].enabled = false;
			}
		}
	}
}
