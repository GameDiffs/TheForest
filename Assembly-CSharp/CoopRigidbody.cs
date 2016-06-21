using Bolt;
using System;
using UnityEngine;

public class CoopRigidbody : EntityBehaviour<IRigidbodyState>
{
	[SerializeField]
	private Rigidbody targetRigidbody;

	public override void Attached()
	{
		if (!this.entity.isOwner)
		{
			Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Collider collider = componentsInChildren[i];
				collider.enabled = collider.isTrigger;
			}
			Rigidbody[] componentsInChildren2 = base.GetComponentsInChildren<Rigidbody>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				Rigidbody rigidbody = componentsInChildren2[j];
				rigidbody.useGravity = false;
				rigidbody.isKinematic = true;
			}
		}
		base.state.Transform.SetTransforms(this.targetRigidbody.transform);
	}
}
