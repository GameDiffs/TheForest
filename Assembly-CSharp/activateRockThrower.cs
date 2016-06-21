using System;
using TheForest.Utils;
using UnityEngine;

public class activateRockThrower : MonoBehaviour
{
	public GameObject Sheen;

	public GameObject MyPickUp;

	public GameObject enterPos;

	public GameObject thrower;

	public GameObject lever;

	public bool activated;

	private void Start()
	{
		base.enabled = false;
	}

	private void OnDisable()
	{
		base.enabled = false;
		this.MyPickUp.SetActive(false);
	}

	private void GrabEnter(GameObject grabber)
	{
		base.enabled = true;
		this.Sheen.SetActive(false);
		this.MyPickUp.SetActive(true);
	}

	private void GrabExit(GameObject grabber)
	{
		if (base.enabled)
		{
			base.enabled = false;
			this.MyPickUp.SetActive(false);
		}
	}

	private void Update()
	{
		if (this.activated || !base.gameObject.activeSelf)
		{
			return;
		}
		if (LocalPlayer.FpCharacter.PushingSled || LocalPlayer.AnimControl.onRockThrower)
		{
			return;
		}
		if (TheForest.Utils.Input.GetButtonDown("Take"))
		{
			LocalPlayer.SpecialActions.SendMessage("setCurrentTrigger", base.gameObject);
			LocalPlayer.SpecialActions.SendMessage("setCurrentThrower", this.thrower);
			LocalPlayer.SpecialActions.SendMessage("setCurrentLever", this.lever);
			LocalPlayer.SpecialActions.SendMessage("enterRockThrower", this.enterPos);
			this.Sheen.SetActive(false);
			this.MyPickUp.SetActive(false);
		}
	}
}
