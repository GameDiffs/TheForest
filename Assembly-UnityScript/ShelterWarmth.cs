using System;
using UnityEngine;

[Serializable]
public class ShelterWarmth : MonoBehaviour
{
	private GameObject Player;

	private bool playerHere;

	public override void Awake()
	{
		this.Player = GameObject.FindWithTag("Player");
	}

	public override void OnTriggerEnter(Collider otherObject)
	{
		if (otherObject.gameObject.CompareTag("Player"))
		{
			otherObject.SendMessage("Heat");
			this.playerHere = true;
			this.Player.SendMessage("ShowCanSleep");
		}
	}

	public override void OnTriggerExit(Collider otherObject)
	{
		if (otherObject.gameObject.CompareTag("Player"))
		{
			otherObject.SendMessage("LeaveHeat");
			if (this.playerHere)
			{
				this.Player.SendMessage("CloseCanSleep");
				this.playerHere = false;
			}
		}
	}

	public override void Update()
	{
		if (this.playerHere && Input.GetButtonDown("RestKey"))
		{
			this.Player.SendMessage("GoToSleep");
			this.Player.SendMessage("CloseCanSleep");
		}
	}

	public override void Main()
	{
	}
}
