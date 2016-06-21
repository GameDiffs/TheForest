using System;
using UnityEngine;

public class pushRigidBody : MonoBehaviour
{
	public bool regularMutant;

	public bool dontBreakCrates;

	public float pushPower;

	public float weight;

	public float minPushDistance;

	private void Start()
	{
	}

	private void OnTriggerEnter(Collider hit)
	{
		if (this.regularMutant)
		{
			return;
		}
		bool flag = false;
		if (hit.gameObject.CompareTag("UnderfootWood"))
		{
			enemyCanPush component = hit.gameObject.GetComponent<enemyCanPush>();
			if (component)
			{
				flag = true;
			}
		}
		if (hit.gameObject.CompareTag("Float") || hit.gameObject.CompareTag("suitCase") || hit.gameObject.CompareTag("pushable") || hit.gameObject.CompareTag("BreakableWood") || hit.gameObject.CompareTag("BreakableRock") || hit.gameObject.CompareTag("hanging") || flag)
		{
			if (hit.gameObject.CompareTag("BreakableRock"))
			{
				hit.gameObject.SendMessage("Explosion", SendMessageOptions.DontRequireReceiver);
			}
			Rigidbody attachedRigidbody = hit.GetComponent<Collider>().attachedRigidbody;
			if (!this.dontBreakCrates && hit.gameObject.CompareTag("BreakableWood"))
			{
				hit.gameObject.SendMessage("Hit", 60, SendMessageOptions.DontRequireReceiver);
			}
			if (attachedRigidbody == null || attachedRigidbody.isKinematic)
			{
				return;
			}
			float explosionForce = 300f;
			if (flag)
			{
				explosionForce = 1000f;
			}
			attachedRigidbody.AddExplosionForce(explosionForce, base.transform.position, 20f);
		}
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (this.regularMutant)
		{
			bool flag = false;
			if (hit.gameObject.CompareTag("UnderfootWood"))
			{
				enemyCanPush component = hit.gameObject.GetComponent<enemyCanPush>();
				if (component)
				{
					flag = true;
				}
			}
			if (hit.gameObject.CompareTag("Float") || hit.gameObject.CompareTag("suitCase") || hit.gameObject.CompareTag("pushable") || hit.gameObject.CompareTag("BreakableWood") || hit.gameObject.CompareTag("hanging") || flag)
			{
				Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
				if (attachedRigidbody == null || attachedRigidbody.isKinematic)
				{
					return;
				}
				float explosionForce = 100f;
				if (flag)
				{
					explosionForce = 1000f;
				}
				attachedRigidbody.AddExplosionForce(explosionForce, base.transform.position, 20f);
			}
		}
	}
}
