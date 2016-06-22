using System;
using UnityEngine;

[Serializable]
public class SharkArea : MonoBehaviour
{
	public override void OnTriggerEnter(Collider otherObject)
	{
		if (otherObject.gameObject.CompareTag("Player"))
		{
			otherObject.SendMessage("EnteredOcean");
		}
	}

	public override void OnTriggerExit(Collider otherObject)
	{
		if (otherObject.gameObject.CompareTag("Player"))
		{
			otherObject.SendMessage("LeftOcean");
		}
	}

	public override void Main()
	{
	}
}
