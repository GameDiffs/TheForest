using FMOD.Studio;
using System;
using UnityEngine;

public class objectOnWater : MonoBehaviour
{
	[Header("FMOD EVENT")]
	public string onWaterEvent;

	private EventInstance onWater;

	private void OnEnable()
	{
		this.onWater = FMOD_StudioSystem.instance.GetEvent(this.onWaterEvent);
	}

	private void Update()
	{
		PLAYBACK_STATE pLAYBACK_STATE;
		UnityUtil.ERRCHECK(this.onWater.getPlaybackState(out pLAYBACK_STATE));
		if (base.gameObject.GetComponent<Buoyancy>().inWaterCounter > 0)
		{
			if (pLAYBACK_STATE == PLAYBACK_STATE.STOPPED)
			{
				UnityUtil.ERRCHECK(this.onWater.set3DAttributes(UnityUtil.to3DAttributes(base.gameObject, null)));
				UnityUtil.ERRCHECK(this.onWater.start());
			}
			else
			{
				UnityUtil.ERRCHECK(this.onWater.set3DAttributes(UnityUtil.to3DAttributes(base.gameObject, null)));
			}
		}
		else if (pLAYBACK_STATE == PLAYBACK_STATE.PLAYING || pLAYBACK_STATE == PLAYBACK_STATE.STARTING)
		{
			UnityUtil.ERRCHECK(this.onWater.stop(STOP_MODE.ALLOWFADEOUT));
		}
	}

	private void OnDisable()
	{
		UnityUtil.ERRCHECK(this.onWater.release());
	}
}
