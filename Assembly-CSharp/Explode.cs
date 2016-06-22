using Bolt;
using System;
using TheForest.Utils;
using UnityEngine;

public class Explode : EntityBehaviour
{
	public struct Data
	{
		public float distance;

		public Explode explode;
	}

	public float radius = 15f;

	public float power = 10000f;

	public float damage = 400f;

	public static float overridePowerFloat;

	private void Start()
	{
		base.Invoke("RunExplode", 0.1f);
		base.Invoke("CleanUp", 7f);
	}

	public void setRadius(float val)
	{
		this.radius = val;
	}

	private void RunExplode()
	{
		this.radius = this.radius;
		Vector3 position = base.transform.position;
		Collider[] array = Physics.OverlapSphere(position, this.radius);
		for (int i = 0; i < array.Length; i++)
		{
			Collider collider = array[i];
			if (BoltNetwork.isClient)
			{
				if (collider.CompareTag("playerHitDetect"))
				{
					float num = Vector3.Distance(base.transform.position, collider.transform.position);
					collider.SendMessageUpwards("Explosion", num, SendMessageOptions.DontRequireReceiver);
					collider.SendMessage("lookAtExplosion", base.transform.position, SendMessageOptions.DontRequireReceiver);
					if (collider && collider.GetComponent<Rigidbody>())
					{
						collider.GetComponent<Rigidbody>().AddExplosionForce(this.power, position, this.radius, 3f);
					}
				}
				if (collider.CompareTag("SmallTree") || collider.CompareTag("Tree") || collider.CompareTag("MidTree") || collider.CompareTag("BreakableWood") || collider.CompareTag("BreakableRock") || collider.CompareTag("Fish"))
				{
					float num2 = Vector3.Distance(base.transform.position, collider.transform.position);
					if (collider.CompareTag("lb_bird") || collider.CompareTag("Fish"))
					{
						collider.gameObject.SendMessage("Explosion", num2, SendMessageOptions.DontRequireReceiver);
					}
					else
					{
						collider.gameObject.SendMessageUpwards("Explosion", num2, SendMessageOptions.DontRequireReceiver);
					}
					collider.gameObject.SendMessage("lookAtExplosion", base.transform.position, SendMessageOptions.DontRequireReceiver);
				}
			}
			else
			{
				if (collider.CompareTag("enemyCollide") || collider.CompareTag("animalCollide") || collider.CompareTag("lb_bird") || collider.CompareTag("playerHitDetect") || collider.CompareTag("structure") || collider.CompareTag("SLTier1") || collider.CompareTag("SLTier2") || collider.CompareTag("SLTier3") || collider.CompareTag("Tree") || collider.CompareTag("SmallTree") || collider.CompareTag("BreakableWood") || collider.CompareTag("MidTree") || collider.CompareTag("BreakableRock") || collider.CompareTag("Fish") || collider.CompareTag("jumpObject") || collider.CompareTag("UnderfootWood") || collider.CompareTag("UnderfootRock") || collider.CompareTag("Target"))
				{
					float num3 = Vector3.Distance(base.transform.position, collider.transform.position);
					if (collider.CompareTag("lb_bird") || collider.CompareTag("Fish"))
					{
						collider.gameObject.SendMessage("Explosion", num3, SendMessageOptions.DontRequireReceiver);
					}
					else
					{
						collider.gameObject.SendMessageUpwards("Explosion", num3, SendMessageOptions.DontRequireReceiver);
					}
					collider.gameObject.SendMessage("lookAtExplosion", base.transform.position, SendMessageOptions.DontRequireReceiver);
					if (num3 < this.radius)
					{
						collider.gameObject.SendMessage("OnExplode", new Explode.Data
						{
							distance = num3,
							explode = this
						}, SendMessageOptions.DontRequireReceiver);
					}
				}
				else if (collider.CompareTag("TripWireTrigger"))
				{
					collider.SendMessage("OnTripped", SendMessageOptions.DontRequireReceiver);
				}
				if (collider && collider.GetComponent<Rigidbody>())
				{
					if (!collider.gameObject.CompareTag("Tree"))
					{
						float num4 = 10000f;
						if (collider.GetComponent<logChecker>())
						{
							num4 *= 5.5f;
						}
						collider.GetComponent<Rigidbody>().AddExplosionForce(num4, position, this.radius, 3f, ForceMode.Force);
					}
				}
			}
		}
		if (LocalPlayer.GameObject)
		{
			float num5 = Vector3.Distance(LocalPlayer.Transform.position, base.transform.position);
			LocalPlayer.GameObject.SendMessage("enableExplodeShake", num5);
		}
	}

	private void CleanUp()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
