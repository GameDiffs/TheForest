using FMOD.Studio;
using System;
using UnityEngine;

public class PauseMenuAudio : MonoBehaviour
{
	private EventInstance eventInstance;

	private bool destroyAfterLoad;

	private void OnEnable()
	{
		if (FMOD_StudioSystem.instance)
		{
			this.eventInstance = FMOD_StudioSystem.instance.GetEvent("event:/music/menu");
			if (this.eventInstance != null)
			{
				UnityUtil.ERRCHECK(this.eventInstance.start());
			}
		}
	}

	private void OnDisable()
	{
		if (this.eventInstance != null)
		{
			UnityUtil.ERRCHECK(this.eventInstance.setParameterValue("stop", 1f));
			UnityUtil.ERRCHECK(this.eventInstance.release());
		}
	}

	public void PrepareForLevelLoad()
	{
		base.transform.parent = null;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		this.destroyAfterLoad = true;
	}

	private void OnLevelWasLoaded(int level)
	{
		if (this.destroyAfterLoad)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
