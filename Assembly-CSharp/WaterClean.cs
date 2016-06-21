using System;
using TheForest.Buildings.Interfaces;
using UnityEngine;

public class WaterClean : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player") || other.GetComponent(typeof(IWetable)))
		{
			other.SendMessage("GotClean");
		}
	}
}
