using System;
using UnityEngine;

public class thrownRockDamage : MonoBehaviour
{
	public Rigidbody rbVelocity;

	public GameObject pickupTrigger;

	public float checkVel;

	private float disableTime;

	private float pickupTime;

	private Vector3 lastPos;

	private Vector3 currPos;

	private void Start()
	{
		this.disableTime = Time.time + 8f;
		base.Invoke("enablePickup", 6f);
	}

	private void FixedUpdate()
	{
		if (Time.time > this.disableTime)
		{
			base.enabled = false;
		}
		this.currPos = base.transform.position;
		this.checkVel = (this.currPos - this.lastPos).magnitude * 100f;
		this.lastPos = this.currPos;
	}

	private void enablePickup()
	{
		if (this.pickupTrigger)
		{
			this.pickupTrigger.SetActive(true);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		GameObject gameObject = other.gameObject;
		Vector3 position = base.transform.position;
		if (this.checkVel < 12f)
		{
			return;
		}
		if (gameObject.CompareTag("playerHitDetect"))
		{
			if (BoltNetwork.isRunning && BoltNetwork.isServer)
			{
				Debug.Log("doing hit on player");
				CoopPlayerRemoteSetup componentInChildren = gameObject.transform.root.GetComponentInChildren<CoopPlayerRemoteSetup>();
				if (componentInChildren)
				{
					Debug.Log("found cprs");
					CreepHitPlayer creepHitPlayer = CreepHitPlayer.Create(componentInChildren.entity.source);
					creepHitPlayer.damage = 15;
					creepHitPlayer.Send();
					return;
				}
			}
			float num = Vector3.Distance(base.transform.position, gameObject.transform.position);
			gameObject.SendMessageUpwards("Explosion", 8, SendMessageOptions.DontRequireReceiver);
			gameObject.SendMessage("lookAtExplosion", base.transform.position, SendMessageOptions.DontRequireReceiver);
			if (!gameObject || gameObject.GetComponent<Rigidbody>())
			{
			}
		}
		else if (gameObject.CompareTag("animalCollide") || gameObject.CompareTag("enemyCollide") || gameObject.CompareTag("SmallTree") || gameObject.CompareTag("BreakableWood"))
		{
			float num2 = Vector3.Distance(base.transform.position, gameObject.transform.position);
			gameObject.gameObject.SendMessageUpwards("Explosion", 11, SendMessageOptions.DontRequireReceiver);
			gameObject.gameObject.SendMessage("lookAtExplosion", base.transform.position, SendMessageOptions.DontRequireReceiver);
		}
	}
}
