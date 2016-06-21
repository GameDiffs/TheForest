using System;
using UnityEngine;

public class trapHit : MonoBehaviour
{
	public trapTrigger trigger;

	private int trapType;

	public bool disable;

	private Rigidbody rb;

	private void Start()
	{
		if (this.trigger.largeSpike)
		{
			this.trapType = 0;
		}
		if (this.trigger.largeDeadfall)
		{
			this.trapType = 1;
		}
		if (this.trigger.largeNoose)
		{
			this.trapType = 2;
		}
		if (this.trigger.largeSwingingRock)
		{
			this.trapType = 3;
			this.rb = base.transform.GetComponent<Rigidbody>();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("enemyCollide") || other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("animalCollide"))
		{
			if (other.gameObject.CompareTag("enemyCollide"))
			{
				mutantHitReceiver component = other.transform.GetComponent<mutantHitReceiver>();
				if (component && component.inNooseTrap)
				{
					Debug.Log("target is already in a trap");
					return;
				}
			}
			if (!this.disable)
			{
				if (this.trigger.largeSpike)
				{
					other.gameObject.SendMessageUpwards("setTrapLookat", base.transform.root.gameObject, SendMessageOptions.DontRequireReceiver);
					base.gameObject.SendMessage("addTrappedMutant", other.transform.root.gameObject, SendMessageOptions.DontRequireReceiver);
					other.gameObject.SendMessageUpwards("setCurrentTrap", this.trigger.gameObject, SendMessageOptions.DontRequireReceiver);
				}
				if (this.trigger.largeSwingingRock)
				{
					if (this.rb.velocity.magnitude > 9f)
					{
						other.gameObject.SendMessageUpwards("Explosion", 11, SendMessageOptions.DontRequireReceiver);
						other.gameObject.SendMessage("lookAtExplosion", base.transform.position, SendMessageOptions.DontRequireReceiver);
						other.gameObject.SendMessageUpwards("DieTrap", this.trapType, SendMessageOptions.DontRequireReceiver);
					}
				}
				else
				{
					other.gameObject.SendMessageUpwards("DieTrap", this.trapType, SendMessageOptions.DontRequireReceiver);
				}
				if (!this.disable && !this.trigger.largeSwingingRock)
				{
					base.Invoke("disableCollision", 0.05f);
				}
			}
		}
	}

	private void disableCollision()
	{
		this.disable = true;
	}
}
