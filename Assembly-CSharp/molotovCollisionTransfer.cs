using Bolt;
using System;
using UnityEngine;

public class molotovCollisionTransfer : EntityBehaviour<IMultiThrowerProjectileState>
{
	private void OnCollisionEnter(Collision collision)
	{
		Molotov componentInChildren = base.transform.GetComponentInChildren<Molotov>();
		if (componentInChildren)
		{
			componentInChildren.doMolotovCollision(collision);
		}
	}
}
