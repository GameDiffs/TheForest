using System;
using UnityEngine;

public class WeaponInfoPlants : MonoBehaviour
{
	private bool Delay;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("SmallTree") && !this.Delay)
		{
			this.Delay = true;
			base.Invoke("ResetDelay", 1.5f);
			other.SendMessage("Hit", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void ResetDelay()
	{
		this.Delay = false;
	}
}
