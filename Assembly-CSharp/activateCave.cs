using System;
using TheForest.Utils;
using UnityEngine;

public class activateCave : MonoBehaviour
{
	public GameObject Sheen;

	public GameObject MyPickUp;

	public GameObject enterPos;

	public GameObject exitPos;

	public bool entry;

	public bool ignoreLightingSwitch;

	private void Start()
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
		if (base.enabled)
		{
			base.enabled = false;
			this.MyPickUp.SetActive(false);
		}
	}

	private void Update()
	{
		if (!this.ignoreLightingSwitch)
		{
			if (this.entry && Clock.InCave)
			{
				return;
			}
			if (!this.entry && !Clock.InCave)
			{
				return;
			}
		}
		if (LocalPlayer.Animator.GetCurrentAnimatorStateInfo(0).IsTag("enterCave"))
		{
			return;
		}
		if (LocalPlayer.FpCharacter.PushingSled)
		{
			return;
		}
		if (TheForest.Utils.Input.GetButtonDown("Take"))
		{
			LocalPlayer.SpecialActions.SendMessage("setLightingSwitch", this.ignoreLightingSwitch);
			if (this.entry)
			{
				LocalPlayer.SpecialActions.SendMessage("enterCave", this.enterPos);
			}
			else
			{
				LocalPlayer.SpecialActions.SendMessage("exitCave", this.exitPos);
			}
			this.Sheen.SetActive(false);
			this.MyPickUp.SetActive(false);
		}
	}
}
