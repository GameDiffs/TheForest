using System;
using UnityEngine;

public class fatCreepyCharger : MonoBehaviour
{
	private mutantScriptSetup setup;

	public float radius = 15f;

	public float power = 500f;

	public Transform thisHitBox;

	private void Start()
	{
		this.setup = base.transform.root.GetComponentInChildren<mutantScriptSetup>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
		GameObject gameObject = other.gameObject;
		Vector3 position = base.transform.position;
		if (gameObject.CompareTag("playerHitDetect"))
		{
			if (BoltNetwork.isRunning && BoltNetwork.isServer)
			{
				CoopPlayerRemoteSetup componentInChildren = gameObject.transform.root.GetComponentInChildren<CoopPlayerRemoteSetup>();
				if (componentInChildren)
				{
					CreepHitPlayer creepHitPlayer = CreepHitPlayer.Create(componentInChildren.entity.source);
					creepHitPlayer.damage = 8;
					creepHitPlayer.Send();
					return;
				}
			}
			float num = Vector3.Distance(base.transform.position, gameObject.transform.position);
			gameObject.SendMessageUpwards("Explosion", 8, SendMessageOptions.DontRequireReceiver);
			gameObject.SendMessage("lookAtExplosion", base.transform.position, SendMessageOptions.DontRequireReceiver);
			this.setup.pmCombat.SendEvent("toWander");
			if (!gameObject || gameObject.GetComponent<Rigidbody>())
			{
			}
		}
		else
		{
			if (gameObject.CompareTag("animalCollide") || gameObject.CompareTag("enemyCollide") || gameObject.CompareTag("structure") || gameObject.CompareTag("SLTier1") || gameObject.CompareTag("SLTier2") || gameObject.CompareTag("SLTier3") || gameObject.CompareTag("SmallTree") || gameObject.CompareTag("BreakableWood") || gameObject.CompareTag("BreakableRock"))
			{
				float num2 = Vector3.Distance(base.transform.position, gameObject.transform.position);
				if (gameObject.CompareTag("BreakableRock"))
				{
					gameObject.gameObject.SendMessage("Explosion", SendMessageOptions.DontRequireReceiver);
					return;
				}
				if (gameObject.gameObject.CompareTag("enemyCollide"))
				{
					if (gameObject.gameObject.GetInstanceID() != this.thisHitBox.gameObject.GetInstanceID())
					{
						mutantTargetSwitching component = gameObject.gameObject.GetComponent<mutantTargetSwitching>();
						if (component && component.typeFatCreepy)
						{
							return;
						}
						gameObject.gameObject.SendMessageUpwards("Explosion", 11, SendMessageOptions.DontRequireReceiver);
					}
				}
				else
				{
					gameObject.gameObject.SendMessageUpwards("Explosion", num2, SendMessageOptions.DontRequireReceiver);
					gameObject.gameObject.SendMessage("lookAtExplosion", base.transform.position, SendMessageOptions.DontRequireReceiver);
				}
			}
			if (gameObject && gameObject.GetComponent<Rigidbody>())
			{
				if (gameObject.gameObject.CompareTag("Tree"))
				{
					gameObject.GetComponent<Rigidbody>().AddExplosionForce(this.power / 10f, position, this.radius, 3f);
				}
				else
				{
					gameObject.GetComponent<Rigidbody>().AddExplosionForce(this.power, position, this.radius, 3f);
				}
			}
		}
	}
}
