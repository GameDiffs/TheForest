using Bolt;
using System;
using System.Collections.Generic;
using UnityEngine;

internal class CoopComponentDisabler : EntityBehaviour
{
	public List<Renderer> Renderers = new List<Renderer>();

	public List<Collider> Colliders = new List<Collider>();

	public Animator animator;

	public void EnableComponents()
	{
		this.Apply(true);
	}

	public void DisableComponents()
	{
		this.Apply(false);
	}

	private void Apply(bool enabled)
	{
		for (int i = 0; i < this.Renderers.Count; i++)
		{
			if (this.Renderers[i])
			{
				this.Renderers[i].enabled = enabled;
			}
		}
		for (int j = 0; j < this.Colliders.Count; j++)
		{
			if (this.Colliders[j])
			{
				this.Colliders[j].enabled = enabled;
			}
		}
		if (this.animator)
		{
			this.animator.enabled = enabled;
		}
	}

	private void Awake()
	{
		if (this.entity && this.entity.isAttached)
		{
			Renderer[] componentsInChildren = this.entity.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Renderer item = componentsInChildren[i];
				this.Renderers.Add(item);
			}
			Collider[] componentsInChildren2 = this.entity.GetComponentsInChildren<Collider>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				Collider item2 = componentsInChildren2[j];
				this.Colliders.Add(item2);
			}
			this.animator = this.entity.GetComponentInChildren<Animator>();
			if (this.entity.isFrozen)
			{
				this.DisableComponents();
			}
			else
			{
				this.EnableComponents();
			}
		}
	}
}
