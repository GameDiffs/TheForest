using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

public class ShelterTrigger : MonoBehaviour
{
	private const float MpSleepDelayDuration = 1f;

	public GameObject SleepIcon;

	public GameObject SaveIcon;

	public bool BreakAfterSleep;

	private int _previousPlayersReady;

	private float _mpSleepDelay = -3.40282347E+38f;

	private bool CanSleep
	{
		get
		{
			return Scene.Clock.ElapsedGameTime > LocalPlayer.Stats.NextSleepTime;
		}
	}

	private void Awake()
	{
		if (this.SleepIcon)
		{
			this.SleepIcon.SetActive(false);
		}
		if (this.SaveIcon)
		{
			this.SaveIcon.SetActive(false);
		}
		base.enabled = false;
	}

	private void Update()
	{
		if (LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.World)
		{
			if (this.CanSleep)
			{
				if (!this.SleepIcon.activeSelf)
				{
					this.SleepIcon.SetActive(true);
					Scene.HudGui.SleepDelayIcon.gameObject.SetActive(false);
				}
				if (TheForest.Utils.Input.GetButtonAfterDelay("RestKey", 0.5f))
				{
					if (!BoltNetwork.isRunning)
					{
						if (LocalPlayer.Stats.GoToSleep() && this.BreakAfterSleep)
						{
							base.StartCoroutine(this.DelayedCollapse());
						}
					}
					else
					{
						this.ResetMpSleepDelay();
						this._previousPlayersReady = -1;
						this.SleepIcon.SetActive(false);
						Scene.HudGui.SleepDelayIcon.gameObject.SetActive(false);
						Scene.HudGui.MpSleepLabel.gameObject.SetActive(true);
						LocalPlayer.Inventory.CurrentView = PlayerInventory.PlayerViews.Sleep;
					}
				}
			}
			else if (this.SleepIcon.activeSelf || !Scene.HudGui.SleepDelayIcon.gameObject.activeSelf)
			{
				this.SleepIcon.SetActive(false);
				Scene.HudGui.SleepDelayIcon._target = this.SleepIcon.transform;
				Scene.HudGui.SleepDelayIcon.gameObject.SetActive(true);
			}
		}
		if (LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.Sleep)
		{
			if (TheForest.Utils.Input.GetButtonDown("Esc"))
			{
				LocalPlayer.Inventory.CurrentView = PlayerInventory.PlayerViews.World;
				Scene.HudGui.MpSleepLabel.gameObject.SetActive(false);
			}
			else
			{
				int num = 0;
				for (int i = 0; i < Scene.SceneTracker.allPlayerEntities.Count; i++)
				{
					if (Scene.SceneTracker.allPlayerEntities[i].GetState<IPlayerState>().CurrentView == 9)
					{
						num++;
					}
				}
				if (this._previousPlayersReady != num)
				{
					this.ResetMpSleepDelay();
					this._previousPlayersReady = num;
					Scene.HudGui.MpSleepLabel.text = num + 1 + "/" + (Scene.SceneTracker.allPlayerEntities.Count + 1);
				}
				if (BoltNetwork.isServer && num == Scene.SceneTracker.allPlayerEntities.Count)
				{
					if (this._mpSleepDelay > 0f)
					{
						this._mpSleepDelay -= Time.deltaTime;
					}
					else
					{
						this.ResetMpSleepDelay();
						Sleep sleep = Sleep.Create(GlobalTargets.AllClients);
						if (LocalPlayer.Stats.GoToSleep())
						{
							if (this.BreakAfterSleep)
							{
								base.StartCoroutine(this.DelayedCollapse());
							}
						}
						else
						{
							sleep.Aborted = true;
						}
						sleep.Send();
						LocalPlayer.Inventory.CurrentView = PlayerInventory.PlayerViews.World;
						Scene.HudGui.MpSleepLabel.gameObject.SetActive(false);
					}
				}
			}
		}
		if (TheForest.Utils.Input.GetButtonAfterDelay("Save", 0.5f))
		{
			LocalPlayer.Stats.JustSave();
		}
	}

	private void ResetMpSleepDelay()
	{
		this._mpSleepDelay = 1f;
	}

	private void GrabExit()
	{
		if (this.SleepIcon)
		{
			this.SleepIcon.SetActive(false);
			Scene.HudGui.SleepDelayIcon.gameObject.SetActive(false);
		}
		if (this.SaveIcon)
		{
			this.SaveIcon.SetActive(false);
		}
		base.enabled = false;
		if (LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.Sleep)
		{
			LocalPlayer.Inventory.CurrentView = PlayerInventory.PlayerViews.World;
			Scene.HudGui.MpSleepLabel.gameObject.SetActive(false);
		}
	}

	private void GrabEnter()
	{
		if (this.SleepIcon && !BoltNetwork.isRunning)
		{
			if (this.CanSleep)
			{
				this.SleepIcon.SetActive(true);
			}
			else
			{
				Scene.HudGui.SleepDelayIcon.gameObject.SetActive(true);
				Scene.HudGui.SleepDelayIcon._target = this.SleepIcon.transform;
			}
		}
		if (this.SaveIcon)
		{
			this.SaveIcon.SetActive(true);
		}
		base.enabled = true;
	}

	private void OnDestroy()
	{
		if (Scene.HudGui && this.SleepIcon && Scene.HudGui.SleepDelayIcon._target == this.SleepIcon.transform)
		{
			Scene.HudGui.SleepDelayIcon.gameObject.SetActive(false);
		}
	}

	[DebuggerHidden]
	public IEnumerator DelayedCollapse()
	{
		ShelterTrigger.<DelayedCollapse>c__Iterator1B3 <DelayedCollapse>c__Iterator1B = new ShelterTrigger.<DelayedCollapse>c__Iterator1B3();
		<DelayedCollapse>c__Iterator1B.<>f__this = this;
		return <DelayedCollapse>c__Iterator1B;
	}
}
