using System;
using UnityEngine;

public class rockDamage : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if ((other.gameObject.CompareTag("enemyCollide") || other.gameObject.CompareTag("lb_bird") || other.gameObject.CompareTag("animalCollide")) && base.GetComponent<Rigidbody>().velocity.magnitude > 5f)
		{
			other.gameObject.SendMessage("Hit", 2);
		}
	}
}
