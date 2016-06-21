using System;
using TheForest.Utils;
using UnityEngine;

public class CoopWaterCollider : MonoBehaviour
{
	public Collider WaterCollider;

	private void Start()
	{
		if (!this.WaterCollider)
		{
			this.WaterCollider = base.GetComponent<Collider>();
		}
	}

	private void Update()
	{
		if (LocalPlayer.GameObject && this.WaterCollider)
		{
			CoopPlayerColliders componentInChildren = LocalPlayer.GameObject.GetComponentInChildren<CoopPlayerColliders>();
			if (componentInChildren)
			{
				Collider[] worldCollisionColiders = componentInChildren.WorldCollisionColiders;
				for (int i = 0; i < worldCollisionColiders.Length; i++)
				{
					Collider collider = worldCollisionColiders[i];
					Physics.IgnoreCollision(this.WaterCollider, collider, true);
				}
				base.enabled = false;
			}
		}
	}
}
