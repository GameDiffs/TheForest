using System;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

public class activateBench : MonoBehaviour
{
	public GameObject Sheen;

	public GameObject MyPickUp;

	private bool Sitting;

	private PlayerInventory Player;

	private void Start()
	{
		base.enabled = false;
	}

	private void GrabEnter(GameObject grabber)
	{
		if (!LocalPlayer.AnimControl.onRope)
		{
			base.enabled = true;
			this.Player = grabber.transform.root.GetComponent<PlayerInventory>();
			this.Sheen.SetActive(false);
			this.MyPickUp.SetActive(true);
		}
	}

	private void GrabExit(GameObject grabber)
	{
		if (!this.Sitting && this.Player && this.Player.transform == grabber.transform.root)
		{
			base.enabled = false;
			this.Sheen.SetActive(true);
			this.MyPickUp.SetActive(false);
		}
	}

	private void Update()
	{
		if (LocalPlayer.AnimControl.onRope)
		{
			this.Sheen.SetActive(true);
			this.MyPickUp.SetActive(false);
			base.enabled = false;
			return;
		}
		if (!this.Sitting)
		{
			if (TheForest.Utils.Input.GetButtonDown("Take"))
			{
				this.Sitting = true;
				this.Player.SpecialActions.SendMessage("SitOnBench", base.transform);
				this.Sheen.SetActive(false);
				this.MyPickUp.SetActive(false);
			}
		}
		else if (TheForest.Utils.Input.GetButtonDown("Take") || TheForest.Utils.Input.GetButtonDown("Crouch") || TheForest.Utils.Input.GetButtonDown("Jump") || TheForest.Utils.Input.GetButtonDown("Vertical"))
		{
			this.Player.SpecialActions.SendMessage("UpFromBench");
			this.Sitting = false;
			this.Player = null;
			base.enabled = false;
		}
	}
}
