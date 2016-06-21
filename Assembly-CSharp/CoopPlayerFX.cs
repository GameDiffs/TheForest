using Bolt;
using FMOD.Studio;
using System;
using UnityEngine;

public class CoopPlayerFX : EntityBehaviour<IPlayerState>
{
	public GameObject FX_Fire;

	public GameObject FX_MolotovFire;

	public GameObject FX_DynamiteFire;

	private bool wasBurning;

	private EventInstance BurningEventInstance;

	private ParameterInstance BurningHealthParameter;

	private CueInstance BurningEventKeyoff;

	private PlayerStats playerStats;

	public override void Attached()
	{
		base.state.AddCallback("FX_Fire", new PropertyCallbackSimple(this.OnFxFire));
		base.state.AddCallback("MolotovFire", new PropertyCallbackSimple(this.OnMolotovFire));
		base.state.AddCallback("DynamiteFire", new PropertyCallbackSimple(this.OnDynamiteFire));
	}

	private void OnFxFire()
	{
		this.FX_Fire.SetActive(base.state.FX_Fire);
	}

	private void OnMolotovFire()
	{
		this.FX_MolotovFire.SetActive(base.state.MolotovFire);
	}

	private void OnDynamiteFire()
	{
		this.FX_DynamiteFire.SetActive(base.state.DynamiteFire);
	}

	private void Start()
	{
		if (FMOD_StudioSystem.instance)
		{
			this.BurningEventInstance = FMOD_StudioSystem.instance.GetEvent("event:/player/player_vox/player_onfire");
			if (this.BurningEventInstance != null)
			{
				UnityUtil.ERRCHECK(this.BurningEventInstance.getParameter("health", out this.BurningHealthParameter));
				UnityUtil.ERRCHECK(this.BurningEventInstance.getCue("KeyOff", out this.BurningEventKeyoff));
			}
			this.playerStats = base.GetComponent<PlayerStats>();
		}
	}

	public void Update()
	{
		if (BoltNetwork.isRunning && this.entity && this.entity.isAttached && this.entity.isOwner)
		{
			base.state.FX_Fire = this.FX_Fire.activeInHierarchy;
			base.state.MolotovFire = this.FX_MolotovFire.activeInHierarchy;
			base.state.DynamiteFire = this.FX_DynamiteFire.activeInHierarchy;
		}
		this.UpdateBurningEvent();
	}

	private void UpdateBurningEvent()
	{
		if (this.BurningEventInstance != null && this.BurningEventInstance.isValid())
		{
			UnityUtil.ERRCHECK(this.BurningEventInstance.set3DAttributes(UnityUtil.to3DAttributes(base.gameObject, null)));
			bool activeInHierarchy = this.FX_Fire.activeInHierarchy;
			if (activeInHierarchy && this.BurningHealthParameter != null && this.BurningHealthParameter.isValid())
			{
				if (this.playerStats != null)
				{
					UnityUtil.ERRCHECK(this.BurningHealthParameter.setValue(this.playerStats.Health));
				}
				else
				{
					UnityUtil.ERRCHECK(this.BurningHealthParameter.setValue(50f));
				}
			}
			if (activeInHierarchy != this.wasBurning)
			{
				this.wasBurning = activeInHierarchy;
				if (activeInHierarchy)
				{
					UnityUtil.ERRCHECK(this.BurningEventInstance.start());
				}
				else if (this.BurningEventKeyoff != null && this.BurningEventKeyoff.isValid())
				{
					UnityUtil.ERRCHECK(this.BurningEventKeyoff.trigger());
				}
			}
		}
	}

	private void OnDestroy()
	{
		FMODCommon.ReleaseIfValid(this.BurningEventInstance, STOP_MODE.IMMEDIATE);
	}
}
