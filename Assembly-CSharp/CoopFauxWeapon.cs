using System;
using TheForest.World;
using UnityEngine;

public class CoopFauxWeapon : MonoBehaviour
{
	private int fixedUpdate;

	[HideInInspector]
	public int Damage;

	private void OnTriggerEnter(Collider c)
	{
		if (c.CompareTag("SmallTree"))
		{
			c.SendMessage("Hit", this.Damage, SendMessageOptions.DontRequireReceiver);
		}
		else if (c.CompareTag("jumpObject") || c.CompareTag("UnderfootWood"))
		{
			c.SendMessage("LocalizedHit", new LocalizedHitData(base.transform.position, (float)this.Damage), SendMessageOptions.DontRequireReceiver);
		}
	}

	private void FixedUpdate()
	{
		if (++this.fixedUpdate > 10)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
