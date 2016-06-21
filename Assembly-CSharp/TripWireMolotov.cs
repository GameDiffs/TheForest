using System;
using UnityEngine;

public class TripWireMolotov : MonoBehaviour
{
	public GameObject MolotovReal;

	public GameObject MolotovFake;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("enemyRoot") || other.gameObject.CompareTag("enemyRoot"))
		{
			this.MolotovReal.SetActive(true);
			this.MolotovFake.SetActive(false);
		}
	}
}
