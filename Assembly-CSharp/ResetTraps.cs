using Bolt;
using System;
using TheForest.Utils;
using UnityEngine;

[DoNotSerializePublic]
public class ResetTraps : EntityBehaviour<IBuildingState>
{
	public GameObject Built;

	public GameObject TrapTrigger;

	public GameObject SheenIcon;

	public GameObject PickupIcon;

	private GameObject Ghost;

	private void Awake()
	{
		this.Ghost = base.transform.parent.gameObject;
		base.enabled = false;
	}

	private void Update()
	{
		if (TheForest.Utils.Input.GetButtonAfterDelay("Take", 0.5f))
		{
			base.enabled = false;
			this.Restore();
		}
	}

	private void GrabEnter(GameObject grabber)
	{
		this.SheenIcon.SetActive(false);
		this.PickupIcon.SetActive(true);
		base.enabled = true;
	}

	private void GrabExit()
	{
		base.enabled = false;
		this.SheenIcon.SetActive(true);
		this.PickupIcon.SetActive(false);
	}

	private void Restore()
	{
		LocalPlayer.Sfx.PlayTwinkle();
		if (BoltNetwork.isRunning)
		{
			ResetTrap resetTrap = ResetTrap.Raise(GlobalTargets.OnlyServer);
			resetTrap.TargetTrap = this.entity;
			resetTrap.Send();
		}
		else
		{
			this.RestoreSafe();
		}
	}

	public void RestoreSafe()
	{
		if (this.TrapTrigger)
		{
			this.TrapTrigger.SendMessage("releaseAnimal", SendMessageOptions.DontRequireReceiver);
		}
		if (!BoltNetwork.isRunning)
		{
			UnityEngine.Object.Instantiate(this.Built, this.Ghost.transform.position, this.Ghost.transform.rotation);
			if (this.Ghost)
			{
				UnityEngine.Object.Destroy(this.Ghost);
			}
		}
	}
}
