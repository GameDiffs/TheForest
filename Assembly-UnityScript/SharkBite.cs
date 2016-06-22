using System;
using UnityEngine;

[Serializable]
public class SharkBite : MonoBehaviour
{
	public override void OnTriggerEnter(Collider otherObject)
	{
		if (otherObject.gameObject.CompareTag("Player"))
		{
			otherObject.SendMessage("KilledShark");
		}
		if (otherObject.gameObject.CompareTag("enemy"))
		{
			otherObject.SendMessage("BitShark");
		}
	}

	public override void Main()
	{
	}
}
