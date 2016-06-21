using System;
using TheForest.Utils;
using UnityEngine;

public class mutantPickUp : MonoBehaviour
{
	public GameObject Sheen;

	public GameObject MyPickUp;

	public GameObject parentGo;

	public float delay = 2f;

	private void Awake()
	{
		base.enabled = false;
	}

	private void GrabEnter(GameObject grabber)
	{
		base.enabled = true;
		this.Sheen.SetActive(false);
		this.MyPickUp.SetActive(true);
	}

	private void GrabExit(GameObject grabber)
	{
		base.enabled = false;
		this.Sheen.SetActive(true);
		this.MyPickUp.SetActive(false);
	}

	private void Update()
	{
		if (TheForest.Utils.Input.GetButtonAfterDelay("Take", this.delay))
		{
			this.doPickup();
			this.Sheen.SetActive(false);
			this.MyPickUp.SetActive(false);
		}
	}

	private void doPickup()
	{
		if (!LocalPlayer.AnimControl.carry)
		{
			LocalPlayer.AnimControl.setMutantPickUp(this.parentGo);
			if (this.parentGo)
			{
				this.parentGo.SendMessage("doPickupDummy", SendMessageOptions.DontRequireReceiver);
			}
			base.enabled = false;
		}
	}
}
