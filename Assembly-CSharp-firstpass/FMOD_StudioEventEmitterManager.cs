using System;
using System.Collections.Generic;
using UnityEngine;

public class FMOD_StudioEventEmitterManager : MonoBehaviour
{
	private class EmitterInfo
	{
		public int framesSinceLastMovement;

		public FMOD_StudioEventEmitter emitter;

		public EmitterInfo(FMOD_StudioEventEmitter emitter)
		{
			this.framesSinceLastMovement = 0;
			this.emitter = emitter;
		}
	}

	private static FMOD_StudioEventEmitterManager Instance;

	private static List<FMOD_StudioEventEmitterManager.EmitterInfo> emitters = new List<FMOD_StudioEventEmitterManager.EmitterInfo>();

	private void Awake()
	{
		if (FMOD_StudioEventEmitterManager.Instance == null)
		{
			FMOD_StudioEventEmitterManager.Instance = this;
		}
		else if (FMOD_StudioEventEmitterManager.Instance != this)
		{
			Debug.LogWarning("There is more than one FMOD_StudioEventEmitterManager component in the scene! One has been destroyed.", this);
			UnityEngine.Object.Destroy(this);
		}
	}

	private void OnDestroy()
	{
		if (FMOD_StudioEventEmitterManager.Instance == this)
		{
			FMOD_StudioEventEmitterManager.Instance = null;
		}
	}

	public static void Add(FMOD_StudioEventEmitter emitter)
	{
		for (int i = 0; i < FMOD_StudioEventEmitterManager.emitters.Count; i++)
		{
			if (FMOD_StudioEventEmitterManager.emitters[i].emitter == emitter)
			{
				FMOD_StudioEventEmitterManager.emitters[i].framesSinceLastMovement = 0;
				return;
			}
		}
		FMOD_StudioEventEmitterManager.emitters.Add(new FMOD_StudioEventEmitterManager.EmitterInfo(emitter));
	}

	public static void Remove(FMOD_StudioEventEmitter emitter)
	{
		for (int i = 0; i < FMOD_StudioEventEmitterManager.emitters.Count; i++)
		{
			if (FMOD_StudioEventEmitterManager.emitters[i].emitter == emitter)
			{
				FMOD_StudioEventEmitterManager.emitters[i] = FMOD_StudioEventEmitterManager.emitters[FMOD_StudioEventEmitterManager.emitters.Count - 1];
				FMOD_StudioEventEmitterManager.emitters.RemoveAt(FMOD_StudioEventEmitterManager.emitters.Count - 1);
				return;
			}
		}
	}

	private void LateUpdate()
	{
		int num = 0;
		for (int i = 0; i < FMOD_StudioEventEmitterManager.emitters.Count; i++)
		{
			FMOD_StudioEventEmitterManager.EmitterInfo emitterInfo = FMOD_StudioEventEmitterManager.emitters[i];
			if (emitterInfo.emitter != null)
			{
				if (emitterInfo.emitter.transform.hasChanged)
				{
					emitterInfo.emitter.transform.hasChanged = false;
					emitterInfo.framesSinceLastMovement = 0;
				}
				else
				{
					emitterInfo.framesSinceLastMovement++;
				}
				if (emitterInfo.framesSinceLastMovement < 5)
				{
					FMOD_StudioEventEmitterManager.emitters[num] = emitterInfo;
					num++;
				}
				else
				{
					emitterInfo.emitter.StopMoving();
				}
			}
		}
		for (int j = FMOD_StudioEventEmitterManager.emitters.Count - 1; j >= num; j--)
		{
			FMOD_StudioEventEmitterManager.emitters.RemoveAt(j);
		}
	}
}
