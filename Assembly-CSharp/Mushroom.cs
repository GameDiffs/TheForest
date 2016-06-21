using System;
using TheForest.Utils;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
	public bool MushroomChant;

	public bool MushroomAman;

	public bool MushroomDeer;

	public bool MushroomJack;

	public bool MushroomLibertyCap;

	public bool MushroomPuffMush;

	private GameObject Player;

	private bool PlayerHere;

	public GameObject Sheen;

	public GameObject PickUp;

	private void Awake()
	{
		this.Player = GameObject.FindWithTag("Player");
	}

	private void Update()
	{
		if (this.PlayerHere && TheForest.Utils.Input.GetButtonDown("Take"))
		{
			if (this.MushroomChant)
			{
				this.Player.SendMessage("AteMushroomChant");
			}
			if (this.MushroomAman)
			{
				this.Player.SendMessage("AteMushroomAman");
			}
			if (this.MushroomDeer)
			{
				this.Player.SendMessage("AteMushroomDeer");
			}
			if (this.MushroomJack)
			{
				this.Player.SendMessage("AteMushroomJack");
			}
			if (this.MushroomLibertyCap)
			{
				this.Player.SendMessage("AteMushroomLibertyCap");
			}
			if (this.MushroomPuffMush)
			{
				this.Player.SendMessage("AteMushroomPuffMush");
			}
			UnityEngine.Object.Destroy(base.transform.parent.gameObject);
		}
	}

	private void GrabEnter()
	{
		this.PlayerHere = true;
		this.Sheen.SetActive(false);
		this.PickUp.SetActive(true);
	}

	private void GrabExit()
	{
		this.PlayerHere = false;
		this.Sheen.SetActive(true);
		this.PickUp.SetActive(false);
	}
}
