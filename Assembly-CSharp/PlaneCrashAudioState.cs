using FMOD.Studio;
using System;
using TheForest.Utils;
using UnityEngine;

public class PlaneCrashAudioState : MonoBehaviour
{
	private EventInstance snapshotInstance;

	private static PlaneCrashAudioState sInstance;

	public static void Spawn()
	{
		if (PlaneCrashAudioState.sInstance != null)
		{
			return;
		}
		EventInstance @event = FMOD_StudioSystem.instance.GetEvent("snapshot:/amb_off");
		if (@event == null)
		{
			return;
		}
		GameObject gameObject = new GameObject("Plane Crash Audio State");
		PlaneCrashAudioState.sInstance = gameObject.AddComponent<PlaneCrashAudioState>();
		PlaneCrashAudioState.sInstance.snapshotInstance = @event;
		UnityUtil.ERRCHECK(@event.start());
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}

	private void OnDisable()
	{
		if (this.snapshotInstance != null)
		{
			UnityUtil.ERRCHECK(this.snapshotInstance.stop(STOP_MODE.ALLOWFADEOUT));
			UnityUtil.ERRCHECK(this.snapshotInstance.release());
		}
		if (PlaneCrashAudioState.sInstance == this)
		{
			PlaneCrashAudioState.sInstance = null;
		}
	}

	public static void Disable()
	{
		if (PlaneCrashAudioState.sInstance != null)
		{
			UnityEngine.Object.Destroy(PlaneCrashAudioState.sInstance.gameObject);
		}
	}

	private void OnLevelWasLoaded()
	{
		if (Application.loadedLevelName == "TitleScene")
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if (Scene.PlaneCrashAnimGO && !Scene.PlaneCrashAnimGO.activeSelf)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
