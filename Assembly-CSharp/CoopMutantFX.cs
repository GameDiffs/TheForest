using Bolt;
using FMOD.Studio;
using System;
using UnityEngine;

public class CoopMutantFX : EntityBehaviour<IMutantState>
{
	public GameObject FX_Fire1;

	public GameObject FX_Fire2;

	public GameObject FX_Fire3;

	public GameObject FX_Fire4;

	[Header("FMOD Events (played on creepy mutants for multiplayer clients)")]
	public string OnFireEvent;

	private bool isBurning;

	private EventInstance onFireEventInstance;

	private void Awake()
	{
		if (!BoltNetwork.isRunning)
		{
			base.enabled = false;
		}
	}

	private void OnDisable()
	{
		this.StopOnFireEvent();
	}

	public override void Attached()
	{
		base.state.AddCallback("fx_mask", delegate
		{
			int fx_mask = base.state.fx_mask;
			this.FX_Fire1.SetActive((fx_mask & 1) == 1);
			this.FX_Fire1.SetActive((fx_mask & 2) == 2);
			this.FX_Fire1.SetActive((fx_mask & 4) == 4);
			this.FX_Fire1.SetActive((fx_mask & 8) == 8);
			if (!this.entity.isOwner)
			{
				bool flag = this.isBurning;
				this.isBurning = (fx_mask != 0);
				if (this.isBurning != flag)
				{
					if (this.isBurning)
					{
						this.StartOnFireEvent();
					}
					else
					{
						this.StopOnFireEvent();
					}
				}
			}
		});
	}

	private void StartOnFireEvent()
	{
		if (this.onFireEventInstance == null && !string.IsNullOrEmpty(this.OnFireEvent))
		{
			this.onFireEventInstance = FMOD_StudioSystem.instance.GetEvent(this.OnFireEvent);
			if (this.onFireEventInstance != null)
			{
				this.onFireEventInstance.setParameterValue("health", 50f);
				this.UpdateOnFireEvent();
				UnityUtil.ERRCHECK(this.onFireEventInstance.start());
			}
		}
	}

	private void StopOnFireEvent()
	{
		if (this.onFireEventInstance != null)
		{
			UnityUtil.ERRCHECK(this.onFireEventInstance.stop(STOP_MODE.ALLOWFADEOUT));
			UnityUtil.ERRCHECK(this.onFireEventInstance.release());
			this.onFireEventInstance = null;
		}
	}

	private void UpdateOnFireEvent()
	{
		if (this.onFireEventInstance != null)
		{
			UnityUtil.ERRCHECK(this.onFireEventInstance.set3DAttributes(base.transform.to3DAttributes()));
		}
	}

	public void Update()
	{
		if (BoltNetwork.isRunning && this.entity && this.entity.isAttached && this.entity.isOwner)
		{
			int num = 0;
			if (this.FX_Fire1 && this.FX_Fire1.activeInHierarchy)
			{
				num |= 1;
			}
			if (this.FX_Fire2 && this.FX_Fire2.activeInHierarchy)
			{
				num |= 2;
			}
			if (this.FX_Fire3 && this.FX_Fire3.activeInHierarchy)
			{
				num |= 4;
			}
			if (this.FX_Fire4 && this.FX_Fire4.activeInHierarchy)
			{
				num |= 8;
			}
			if (base.state.fx_mask != num)
			{
				base.state.fx_mask = num;
			}
		}
		this.UpdateOnFireEvent();
	}
}
