using FMOD;
using FMOD.Studio;
using System;
using UnityEngine;

public class creepySfx : MonoBehaviour
{
	public string playerSightedEvent;

	public float playerSightedTimeoutMinimum = 10f;

	public float playerSightedTimeoutMaximum = 15f;

	private bool playerSightedEnabled = true;

	private EventInstance playerSightedInstance;

	private void Start()
	{
		FMODCommon.PreloadEvents(new string[]
		{
			this.playerSightedEvent
		});
	}

	private void Update()
	{
		if (this.playerSightedInstance != null)
		{
			PLAYBACK_STATE state = PLAYBACK_STATE.STOPPED;
			if (this.playerSightedInstance.getPlaybackState(out state) == RESULT.OK && state.isPlaying())
			{
				UnityUtil.ERRCHECK(this.playerSightedInstance.set3DAttributes(base.transform.to3DAttributes()));
			}
			else
			{
				this.playerSightedInstance = null;
			}
		}
	}

	private void PlayerSighted()
	{
		base.CancelInvoke("EnablePlayerSighted");
		if (this.playerSightedEnabled)
		{
			this.playerSightedEnabled = false;
			this.playerSightedInstance = FMODCommon.PlayOneshotNetworked(this.playerSightedEvent, base.transform, FMODCommon.NetworkRole.Server);
		}
	}

	private void PlayerLost()
	{
		if (!base.IsInvoking("EnablePlayerSighted"))
		{
			base.Invoke("EnablePlayerSighted", UnityEngine.Random.Range(this.playerSightedTimeoutMinimum, this.playerSightedTimeoutMaximum));
		}
	}

	private void EnablePlayerSighted()
	{
		this.playerSightedEnabled = true;
	}

	private void OnValidate()
	{
		this.playerSightedTimeoutMinimum = Mathf.Max(this.playerSightedTimeoutMinimum, 0f);
		this.playerSightedTimeoutMaximum = Mathf.Max(this.playerSightedTimeoutMinimum, this.playerSightedTimeoutMaximum);
	}
}
