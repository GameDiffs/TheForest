using FMOD;
using FMOD.Studio;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class FMOD_StudioSystem : MonoBehaviour
{
	public delegate bool ProcessEventInstanceDelegate(EventInstance instance);

	private const float DISTANCE_CULL_BUFFER = 20f;

	private FMOD.Studio.System system;

	private Dictionary<string, EventDescription> eventDescriptions = new Dictionary<string, EventDescription>();

	private bool isInitialized;

	private static FMOD_StudioSystem sInstance;

	public bool LiveUpdateEnabled
	{
		get;
		private set;
	}

	public static FMOD_StudioSystem instance
	{
		get
		{
			if (FMOD_StudioSystem.sInstance == null)
			{
				GameObject gameObject = new GameObject("FMOD_StudioSystem");
				FMOD_StudioSystem.sInstance = gameObject.AddComponent<FMOD_StudioSystem>();
				if (!UnityUtil.ForceLoadLowLevelBinary())
				{
					UnityUtil.LogError("Unable to load low level binary!");
					return FMOD_StudioSystem.sInstance;
				}
				FMOD_StudioSystem.sInstance.Init();
			}
			return FMOD_StudioSystem.sInstance;
		}
	}

	public FMOD.Studio.System System
	{
		get
		{
			return this.system;
		}
	}

	public EventDescription GetEventDescription(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			UnityUtil.LogError("Empty event path!");
		}
		if (this.eventDescriptions.ContainsKey(path))
		{
			return this.eventDescriptions[path];
		}
		EventDescription eventDescription = null;
		RESULT @event = this.system.getEvent(path, out eventDescription);
		if (@event == RESULT.OK)
		{
			this.eventDescriptions.Add(path, eventDescription);
			return eventDescription;
		}
		FMOD_StudioSystem.ERRCHECK(@event);
		UnityUtil.LogWarning("Error getting event with path \"" + path + "\"");
		return null;
	}

	public static bool PreloadEvent(string path)
	{
		return FMOD_StudioSystem.PreloadEvent(path, 1);
	}

	public static bool PreloadEvent(string path, int preloadCount)
	{
		if (FMOD_StudioSystem.sInstance == null)
		{
			return false;
		}
		if (path.Length == 0)
		{
			return true;
		}
		EventDescription eventDescription = FMOD_StudioSystem.sInstance.GetEventDescription(path);
		if (eventDescription == null)
		{
			UnityUtil.LogWarning("Preload failed for \"" + path + "\"");
			return false;
		}
		for (int i = 0; i < preloadCount; i++)
		{
			FMOD_StudioSystem.ERRCHECK(eventDescription.loadSampleData());
		}
		return true;
	}

	public static void UnPreloadEvent(string path)
	{
		FMOD_StudioSystem.UnPreloadEvent(path, 1);
	}

	public static void UnPreloadEvent(string path, int unloadCount)
	{
		if (FMOD_StudioSystem.sInstance == null)
		{
			return;
		}
		if (path.Length == 0)
		{
			return;
		}
		EventDescription eventDescription = FMOD_StudioSystem.sInstance.GetEventDescription(path);
		if (eventDescription == null)
		{
			UnityUtil.LogWarning("UnPreload failed for \"" + path + "\"");
			return;
		}
		for (int i = 0; i < unloadCount; i++)
		{
			FMOD_StudioSystem.ERRCHECK(eventDescription.unloadSampleData());
		}
	}

	public EventInstance GetEvent(string path)
	{
		EventInstance eventInstance = null;
		if (string.IsNullOrEmpty(path))
		{
			UnityUtil.LogError("Empty event path!");
			return null;
		}
		if (this.eventDescriptions.ContainsKey(path))
		{
			FMOD_StudioSystem.ERRCHECK(this.eventDescriptions[path].createInstance(out eventInstance));
		}
		else
		{
			EventDescription eventDescription = null;
			FMOD_StudioSystem.ERRCHECK(this.system.getEvent(path, out eventDescription));
			if (eventDescription != null && eventDescription.isValid())
			{
				this.eventDescriptions.Add(path, eventDescription);
				FMOD_StudioSystem.ERRCHECK(eventDescription.createInstance(out eventInstance));
			}
		}
		if (eventInstance == null)
		{
			UnityUtil.Log("GetEvent FAILED: \"" + path + "\"");
		}
		return eventInstance;
	}

	public EventInstance PlayOneShot(string path, Vector3 position, FMOD_StudioSystem.ProcessEventInstanceDelegate eventSetup = null)
	{
		return this.PlayOneShot(path, position, 1f, eventSetup);
	}

	private EventInstance PlayOneShot(string path, Vector3 position, float volume, FMOD_StudioSystem.ProcessEventInstanceDelegate eventSetup = null)
	{
		if (this.ShouldBeCulled(path, position))
		{
			return null;
		}
		EventInstance @event = this.GetEvent(path);
		FMOD.Studio.ATTRIBUTES_3D attributes = position.to3DAttributes();
		FMOD_StudioSystem.ERRCHECK(@event.set3DAttributes(attributes));
		if (eventSetup == null || eventSetup(@event))
		{
			FMOD_StudioSystem.ERRCHECK(@event.start());
		}
		FMOD_StudioSystem.ERRCHECK(@event.release());
		return @event;
	}

	public bool ShouldBeCulled(string path, Vector3 position)
	{
		EventDescription eventDescription = this.GetEventDescription(path);
		if (eventDescription == null)
		{
			return true;
		}
		bool flag = false;
		FMOD_StudioSystem.ERRCHECK(eventDescription.is3D(out flag));
		if (flag)
		{
			float num = 0f;
			FMOD_StudioSystem.ERRCHECK(eventDescription.getMaximumDistance(out num));
			num += 20f;
			FMOD.Studio.ATTRIBUTES_3D aTTRIBUTES_3D;
			FMOD_StudioSystem.ERRCHECK(this.System.getListenerAttributes(0, out aTTRIBUTES_3D));
			float sqrMagnitude = (position - aTTRIBUTES_3D.position.toUnityVector()).sqrMagnitude;
			float num2 = num * num;
			return sqrMagnitude > num2;
		}
		return false;
	}

	private void Init()
	{
		UnityUtil.Log("FMOD_StudioSystem: Initialize");
		if (this.isInitialized)
		{
			return;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		this.LiveUpdateEnabled = false;
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			string a = commandLineArgs[i];
			if (a == "-FMODLiveUpdate")
			{
				this.LiveUpdateEnabled = true;
				break;
			}
		}
		UnityUtil.Log("FMOD_StudioSystem: System_Create");
		FMOD_StudioSystem.ERRCHECK(FMOD.Studio.System.create(out this.system));
		FMOD.Studio.INITFLAGS iNITFLAGS = FMOD.Studio.INITFLAGS.NORMAL;
		FMOD.ADVANCEDSETTINGS aDVANCEDSETTINGS = default(FMOD.ADVANCEDSETTINGS);
		if (this.LiveUpdateEnabled)
		{
			iNITFLAGS |= FMOD.Studio.INITFLAGS.LIVEUPDATE;
			if (Application.unityVersion.StartsWith("5"))
			{
				UnityUtil.LogWarning("FMOD_StudioSystem: detected Unity 5, running on port 9265");
				aDVANCEDSETTINGS.profilePort = 9265;
			}
		}
		int @int = PlayerPrefs.GetInt("VoiceCount", 128);
		FMOD.System system;
		FMOD_StudioSystem.ERRCHECK(this.system.getLowLevelSystem(out system));
		FMOD_StudioSystem.ERRCHECK(system.setSoftwareChannels(@int));
		aDVANCEDSETTINGS.maxVorbisCodecs = @int;
		FMOD_StudioSystem.ERRCHECK(system.setAdvancedSettings(ref aDVANCEDSETTINGS));
		int num = 48000;
		StringBuilder name = new StringBuilder();
		Guid guid;
		SPEAKERMODE sPEAKERMODE;
		int num2;
		FMOD_StudioSystem.ERRCHECK(system.getDriverInfo(0, name, 0, out guid, out num, out sPEAKERMODE, out num2));
		if (num > 48000)
		{
			uint bufferlength = 0u;
			int num3 = 0;
			FMOD_StudioSystem.ERRCHECK(system.getDSPBufferSize(out bufferlength, out num3));
			FMOD_StudioSystem.ERRCHECK(system.setDSPBufferSize(bufferlength, num3 * 2));
		}
		num = Math.Min(num, 48000);
		FMOD_StudioSystem.ERRCHECK(system.setSoftwareFormat(num, SPEAKERMODE._5POINT1, 0));
		UnityUtil.Log("FMOD_StudioSystem: system.init");
		RESULT rESULT = this.system.initialize(1024, iNITFLAGS, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);
		if (rESULT == RESULT.ERR_NET_SOCKET_ERROR)
		{
			UnityUtil.LogError("Unable to initalize with LiveUpdate: socket is already in use");
		}
		else if (rESULT == RESULT.ERR_HEADER_MISMATCH)
		{
			UnityUtil.LogError("Version mismatch between C# script and FMOD binary, restart Unity and reimport the integration package to resolve this issue.");
		}
		else
		{
			FMOD_StudioSystem.ERRCHECK(rESULT);
		}
		this.isInitialized = true;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (this.system != null)
		{
			FMOD.System system;
			FMOD_StudioSystem.ERRCHECK(this.system.getLowLevelSystem(out system));
			UnityUtil.Log("Pause state changed to: " + pauseStatus);
			if (pauseStatus)
			{
				FMOD_StudioSystem.ERRCHECK(system.mixerSuspend());
			}
			else
			{
				FMOD_StudioSystem.ERRCHECK(system.mixerResume());
			}
		}
	}

	private void Update()
	{
		if (this.isInitialized)
		{
			FMOD_StudioSystem.ERRCHECK(this.system.update());
		}
	}

	private void OnDisable()
	{
		if (this.isInitialized)
		{
			UnityUtil.Log("__ SHUT DOWN FMOD SYSTEM __");
			FMOD_StudioSystem.ERRCHECK(this.system.release());
		}
	}

	private void OnDestroy()
	{
		FMOD_StudioSystem.sInstance = null;
	}

	private static bool ERRCHECK(RESULT result)
	{
		return UnityUtil.ERRCHECK(result);
	}
}
