using Bolt;
using System;
using TheForest.Utils;
using UnityEngine;

public class activateSledPush : EntityBehaviour<IMultiHolderState>
{
	public GameObject Sheen;

	public GameObject MyPickUp;

	public bool onSled;

	public lookAtDir[] allLookat;

	public LogHolder logScript;

	public GameObject logTrigger;

	private bool coolDown;

	private bool inTrigger;

	private bool distanceCheck
	{
		get
		{
			if (BoltNetwork.isRunning)
			{
				float magnitude = (base.transform.position - LocalPlayer.Transform.position).magnitude;
				return magnitude < 3.7f;
			}
			return true;
		}
	}

	private bool CanGrabThisSled()
	{
		BoltEntity componentInParent = base.GetComponentInParent<BoltEntity>();
		if (componentInParent && componentInParent.isAttached && componentInParent.StateIs<IMultiHolderState>())
		{
			IMultiHolderState state = componentInParent.GetState<IMultiHolderState>();
			return state.IsReal && !state.GrabbedBy;
		}
		Debug.LogError("Could not find entity with multiholderstate");
		return false;
	}

	private void SetEnable(bool value)
	{
		if (BoltNetwork.isRunning && this.entity && this.entity.isAttached && this.entity.isOwner && !base.state.IsReal)
		{
			Debug.LogWarning("Can't change enable state for sledpush when sled is not real");
			return;
		}
		base.enabled = value;
	}

	private void Start()
	{
		this.logScript = base.transform.parent.GetComponentInChildren<LogHolder>();
		this.allLookat = base.transform.root.GetComponentsInChildren<lookAtDir>();
		lookAtDir[] array = this.allLookat;
		for (int i = 0; i < array.Length; i++)
		{
			lookAtDir lookAtDir = array[i];
			lookAtDir.enabled = false;
		}
		if (!BoltNetwork.isRunning || (this.entity && this.entity.isAttached && base.state.IsReal))
		{
			this.SetEnable(false);
		}
	}

	private void GrabEnter()
	{
		if (!BoltNetwork.isRunning || this.CanGrabThisSled())
		{
			this.logTrigger.SetActive(false);
			this.inTrigger = true;
			this.Sheen.SetActive(false);
			this.MyPickUp.SetActive(true);
			this.SetEnable(true);
		}
	}

	private void GrabExit()
	{
		if (!BoltNetwork.isRunning || this.CanGrabThisSled())
		{
			this.inTrigger = false;
			if (base.enabled)
			{
				this.logTrigger.SetActive(true);
				this.Sheen.SetActive(true);
				this.MyPickUp.SetActive(false);
			}
			this.SetEnable(false);
		}
	}

	public void Interraction(bool allow)
	{
		this.inTrigger = allow;
		if (allow)
		{
			this.logTrigger.SetActive(false);
			this.Sheen.SetActive(false);
			this.MyPickUp.SetActive(true);
		}
		else
		{
			this.logTrigger.SetActive(false);
			this.Sheen.SetActive(false);
			this.MyPickUp.SetActive(false);
		}
		base.gameObject.SetActive(allow);
	}

	private void Update()
	{
		if (LocalPlayer.AnimControl.onRope || LocalPlayer.FpCharacter.Diving || !LocalPlayer.FpCharacter.Grounded || LocalPlayer.AnimControl.onFire)
		{
			return;
		}
		if (!this.onSled && !this.coolDown && this.inTrigger && !Clock.InCave && this.distanceCheck && TheForest.Utils.Input.GetButtonDown("Take"))
		{
			if (BoltNetwork.isRunning)
			{
				if (this.CanGrabThisSled())
				{
					this.Sheen.SetActive(false);
					this.MyPickUp.SetActive(false);
					SledGrab sledGrab = SledGrab.Create(GlobalTargets.OnlyServer);
					sledGrab.Player = LocalPlayer.Entity;
					sledGrab.Sled = base.GetComponentInParent<BoltEntity>();
					sledGrab.Send();
				}
			}
			else
			{
				this.enableSled();
			}
		}
		else if (this.onSled && !this.coolDown && this.inTrigger && TheForest.Utils.Input.GetButtonDown("Take"))
		{
			this.disableSled();
		}
	}

	public void enableSled()
	{
		LocalPlayer.SpecialActions.SendMessage("enterPushSled", base.transform);
		Grabber.Filter = base.gameObject;
		this.Sheen.SetActive(false);
		this.MyPickUp.SetActive(false);
		this.onSled = true;
		this.coolDown = true;
		base.Invoke("resetCoolDown", 1.2f);
	}

	public void disableSled()
	{
		if (BoltNetwork.isRunning)
		{
			BoltEntity componentInParent = base.GetComponentInParent<BoltEntity>();
			if (componentInParent)
			{
				BoltNetwork.Destroy(componentInParent);
			}
		}
		Grabber.Filter = null;
		this.logTrigger.SetActive(true);
		LocalPlayer.SpecialActions.SendMessage("exitPushSled");
		this.Sheen.SetActive(false);
		this.MyPickUp.SetActive(false);
		this.onSled = false;
		this.coolDown = true;
		base.Invoke("resetCoolDown", 1.2f);
	}

	public void enableSuspension()
	{
		lookAtDir[] array = this.allLookat;
		for (int i = 0; i < array.Length; i++)
		{
			lookAtDir lookAtDir = array[i];
			lookAtDir.enabled = true;
		}
	}

	public void disableSuspension()
	{
		lookAtDir[] array = this.allLookat;
		for (int i = 0; i < array.Length; i++)
		{
			lookAtDir lookAtDir = array[i];
			lookAtDir.enabled = false;
		}
	}

	public void resetTrigger()
	{
		this.onSled = false;
	}

	private void resetCoolDown()
	{
		this.coolDown = false;
	}
}
