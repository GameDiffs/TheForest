using System;
using UnityEngine;

public class animalAttack : MonoBehaviour
{
	public int attackDamage = 5;

	private bool cooldown;

	public bool crocodile;

	private void OnEnable()
	{
		this.cooldown = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("playerHitDetect") && !this.cooldown)
		{
			if (this.crocodile)
			{
				other.SendMessageUpwards("HitShark", this.attackDamage, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				other.SendMessageUpwards("hitFromEnemy", this.attackDamage, SendMessageOptions.DontRequireReceiver);
			}
			this.cooldown = true;
			base.Invoke("resetCooldown", 1f);
		}
	}

	private void resetCooldown()
	{
		this.cooldown = false;
	}
}
