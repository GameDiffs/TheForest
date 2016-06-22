using System;
using UnityEngine;

[Serializable]
public class SpawnZone : MonoBehaviour
{
	public GameObject MySpawn;

	public override void OnTriggerEnter(Collider otherObject)
	{
		if (otherObject.gameObject.CompareTag("Player"))
		{
			this.MySpawn.SendMessage("StartSpawn");
		}
	}

	public override void OnTriggerExit(Collider otherObject)
	{
		if (otherObject.gameObject.CompareTag("Player"))
		{
			this.MySpawn.SendMessage("StopSpawn");
		}
	}

	public override void Main()
	{
	}
}
