using System;
using TheForest.Utils;
using UnityEngine;

public class activateTimmyPickup : MonoBehaviour
{
	public GameObject Sheen;

	public GameObject MyPickUp;

	public bool playerHere;

	private void Start()
	{
	}

	private void GrabEnter(GameObject grabber)
	{
		this.playerHere = true;
		if (this.Sheen)
		{
			this.Sheen.SetActive(false);
		}
		if (this.MyPickUp)
		{
			this.MyPickUp.SetActive(true);
		}
	}

	private void GrabExit(GameObject grabber)
	{
		if (this.Sheen)
		{
			this.Sheen.SetActive(true);
		}
		if (this.MyPickUp)
		{
			this.MyPickUp.SetActive(false);
		}
		this.playerHere = false;
	}

	private void Update()
	{
		if (TheForest.Utils.Input.GetButtonDown("leanForward") && this.playerHere && !LocalPlayer.AnimControl.carry)
		{
			LocalPlayer.AnimControl.setTimmyPickup(base.gameObject);
			this.playerHere = false;
			if (this.Sheen)
			{
				this.Sheen.SetActive(false);
			}
			if (this.MyPickUp)
			{
				this.MyPickUp.SetActive(false);
			}
		}
	}
}
