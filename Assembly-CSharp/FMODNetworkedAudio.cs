using FMOD.Studio;
using System;
using TheForest.Utils;
using UnityEngine;

public static class FMODNetworkedAudio
{
	public static bool DefaultLocal;

	public static float DefaultDistance = 64f;

	public static void PlayOneShot(string eventName, Vector3 position)
	{
		FMODNetworkedAudio.PlayOneShot(eventName, position, FMODNetworkedAudio.DefaultLocal, FMODNetworkedAudio.DefaultDistance);
	}

	public static void PlayOneShot(string eventName, Vector3 position, bool local)
	{
		FMODNetworkedAudio.PlayOneShot(eventName, position, local, FMODNetworkedAudio.DefaultDistance);
	}

	public static void PlayOneShot(string eventName, Vector3 position, float maxDistance)
	{
		FMODNetworkedAudio.PlayOneShot(eventName, position, FMODNetworkedAudio.DefaultLocal, maxDistance);
	}

	public static void PlayOneShot(string eventName, Vector3 position, bool local, float maxDistance)
	{
		if (BoltNetwork.isRunning)
		{
			foreach (BoltEntity current in Scene.SceneTracker.allPlayerEntities)
			{
				if ((current.transform.position - position).sqrMagnitude < maxDistance * maxDistance)
				{
					FMODNetworkedAudio.SendEvent(eventName, position, current.source);
				}
			}
		}
		if (local)
		{
			FMODCommon.PlayOneshot(eventName, position, new object[0]);
		}
	}

	public static void PlayOneShotParameter(string eventName, Vector3 position, int index, float value)
	{
		FMODNetworkedAudio.PlayOneShotParameter(eventName, position, index, value, FMODNetworkedAudio.DefaultLocal, FMODNetworkedAudio.DefaultDistance);
	}

	public static void PlayOneShotParameter(string eventName, Vector3 position, int index, float value, bool local)
	{
		FMODNetworkedAudio.PlayOneShotParameter(eventName, position, index, value, local, FMODNetworkedAudio.DefaultDistance);
	}

	public static void PlayOneShotParameter(string eventName, Vector3 position, int index, float value, float maxDistance)
	{
		FMODNetworkedAudio.PlayOneShotParameter(eventName, position, index, value, FMODNetworkedAudio.DefaultLocal, maxDistance);
	}

	public static void PlayOneShotParameter(string eventName, Vector3 position, int index, float value, bool local, float maxDistance)
	{
		if (BoltNetwork.isRunning)
		{
			foreach (BoltEntity current in Scene.SceneTracker.allPlayerEntities)
			{
				if ((current.transform.position - position).sqrMagnitude < maxDistance * maxDistance)
				{
					FMODNetworkedAudio.SendEvent(eventName, position, current.source, index, value);
				}
			}
		}
		if (local)
		{
			FMOD_StudioSystem.instance.PlayOneShot(eventName, position, delegate(EventInstance eventInstance)
			{
				eventInstance.setParameterValueByIndex(index, value);
				return true;
			});
		}
	}

	private static void SendEvent(string eventName, Vector3 position, BoltConnection connection)
	{
		FmodOneShot fmodOneShot = FmodOneShot.Create(connection);
		fmodOneShot.EventPath = CoopAudioEventDb.FindId(eventName);
		fmodOneShot.Position = position;
		fmodOneShot.Send();
	}

	private static void SendEvent(string eventName, Vector3 position, BoltConnection connection, int index, float value)
	{
		FmodOneShotParameter fmodOneShotParameter = FmodOneShotParameter.Create(connection);
		fmodOneShotParameter.Index = index;
		fmodOneShotParameter.Value = value;
		fmodOneShotParameter.EventPath = CoopAudioEventDb.FindId(eventName);
		fmodOneShotParameter.Position = position;
		fmodOneShotParameter.Send();
	}
}
