using Bolt;
using System;
using UnityEngine;

public class CoopMutantClientHider : EntityBehaviour
{
	private bool _old;

	private void Update()
	{
		if (this.entity.IsAttached() && !this.entity.isOwner && this.entity.isFrozen != this._old)
		{
			Renderer[] componentsInChildren = base.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Renderer renderer = componentsInChildren[i];
				renderer.enabled = !this.entity.isFrozen;
			}
			this._old = this.entity.isFrozen;
		}
		if (!this.entity.IsAttached() || this.entity.isOwner || this.entity.isFrozen)
		{
		}
	}
}
