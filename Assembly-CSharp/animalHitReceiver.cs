using System;
using UnityEngine;

public class animalHitReceiver : MonoBehaviour
{
	private animalHealth health;

	private bool burning;

	private void Awake()
	{
		this.health = base.transform.root.GetComponentInChildren<animalHealth>();
		if (!this.health)
		{
			this.health = base.transform.root.GetComponent<animalHealth>();
		}
	}

	public void Burn()
	{
		if (!this.burning && this.health)
		{
			this.health.Burn();
			this.burning = true;
			base.Invoke("resetBurning", 5f);
		}
	}

	private void resetBurning()
	{
		this.burning = false;
	}
}
