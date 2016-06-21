using PathologicalGames;
using System;
using UnityEngine;

public class CoopAnimalContainer : CoopBase<IAnimalState>
{
	private Transform pooled;

	[SerializeField]
	private GameObject Prefab;

	private void Awake()
	{
		this.pooled = PoolManager.Pools["creatures_net"].Spawn(this.Prefab.transform);
		if (this.pooled)
		{
			this.pooled.transform.parent = base.transform;
			this.pooled.transform.localPosition = Vector3.zero;
			this.pooled.transform.localRotation = Quaternion.identity;
		}
	}

	public override void Detached()
	{
		if (BoltNetwork.isRunning && this.pooled)
		{
			PoolManager.Pools["creatures_net"].Despawn(this.pooled);
			this.pooled.transform.parent = null;
			this.pooled = null;
		}
	}

	public override void Attached()
	{
		CoopAnimal componentInChildren = base.GetComponentInChildren<CoopAnimal>();
		if (!componentInChildren)
		{
			return;
		}
		if (componentInChildren._animator)
		{
			componentInChildren._animator.applyRootMotion = false;
		}
		if (componentInChildren.rotationTransform)
		{
			base.state.TransformPosition.SetTransforms(base.transform);
			base.state.TransformRotation.SetTransforms(componentInChildren.rotationTransform);
		}
		else
		{
			base.state.TransformFull.SetTransforms(base.transform);
		}
		CoopComponentDisabler exists = base.GetComponent<CoopComponentDisabler>();
		if (!exists)
		{
			exists = base.gameObject.AddComponent<CoopComponentDisabler>();
		}
	}
}
