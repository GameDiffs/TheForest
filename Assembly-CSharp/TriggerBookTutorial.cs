using System;
using UnityEngine;

public class TriggerBookTutorial : MonoBehaviour
{
	private void Awake()
	{
		base.gameObject.layer = 2;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			other.SendMessage("ShowStep1Tut");
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
