using FMOD;
using FMOD.Studio;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class FMOD_StudioEventEmitter : MonoBehaviour
{
	public enum TriggerType
	{
		EventMaximumDistance,
		Collider
	}

	[Serializable]
	public class Parameter
	{
		public string name;

		public float value;
	}

	public static float HoursSinceMidnight;

	public static float WindIntensity;

	public static Transform LocalPlayerTransform;

	public string path = string.Empty;

	public bool startEventOnAwake = true;

	public bool startEventOnTriggerEnter;

	public FMOD_StudioEventEmitter.TriggerType triggerType;

	public bool allowFadeoutOnStop = true;

	public bool playOnceOnly;

	public Action<FMOD.Studio.EventInstance> preStartAction;

	[Tooltip("For event velocity")]
	public Rigidbody customRigidbody;

	private EventDescription eventDescription;

	private float triggerRadiusSquared;

	private bool withinTriggerRadius;

	private bool isMoving;

	private FMOD.Studio.EventInstance evt;

	private bool hasStarted;

	private int triggerEnterCount;

	private ParameterInstance timeParameter;

	private ParameterInstance windParameter;

	private Rigidbody cachedRigidBody;

	private static bool isShuttingDown;

	public void Play()
	{
		this.TryGetEventDescription();
		if (!this.IsEventInstanceValid())
		{
			this.CacheEventInstance();
		}
		if (this.evt != null)
		{
			this.ERRCHECK(this.evt.start());
			return;
		}
		UnityUtil.Log("Tried to play event without a valid instance: '" + this.path + "'");
	}

	public void Stop()
	{
		base.StopCoroutine("StartEventWhenStopped");
		if (this.IsEventInstanceValid())
		{
			this.ERRCHECK(this.evt.stop((!this.allowFadeoutOnStop) ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT));
		}
		if (!this.playOnceOnly)
		{
			this.hasStarted = false;
		}
	}

	public void SetVolume(float volume)
	{
		if (this.IsEventInstanceValid())
		{
			this.evt.setVolume(volume);
		}
	}

	public ParameterInstance getParameter(string name)
	{
		ParameterInstance result = null;
		this.ERRCHECK(this.evt.getParameter(name, out result));
		return result;
	}

	public PLAYBACK_STATE getPlaybackState()
	{
		if (!this.IsEventInstanceValid())
		{
			return PLAYBACK_STATE.STOPPED;
		}
		PLAYBACK_STATE result = PLAYBACK_STATE.STOPPED;
		if (this.ERRCHECK(this.evt.getPlaybackState(out result)) == RESULT.OK)
		{
			return result;
		}
		return PLAYBACK_STATE.STOPPED;
	}

	private void CalculateTriggerRadiusSquared()
	{
		FMOD_StudioEventEmitter.TriggerType triggerType = this.triggerType;
		if (triggerType != FMOD_StudioEventEmitter.TriggerType.EventMaximumDistance)
		{
			if (triggerType == FMOD_StudioEventEmitter.TriggerType.Collider)
			{
				this.triggerRadiusSquared = 0f;
				Collider[] components = base.gameObject.GetComponents<Collider>();
				for (int i = 0; i < components.Length; i++)
				{
					Collider collider = components[i];
					float sqrMagnitude = collider.bounds.extents.sqrMagnitude;
					this.triggerRadiusSquared = Mathf.Max(this.triggerRadiusSquared, sqrMagnitude);
				}
			}
		}
		else if (this.IsEventDescriptionValid())
		{
			UnityUtil.ERRCHECK(this.eventDescription.getMaximumDistance(out this.triggerRadiusSquared));
			this.triggerRadiusSquared *= this.triggerRadiusSquared;
		}
		else
		{
			this.triggerRadiusSquared = 0f;
		}
		UnityUtil.Log(string.Concat(new object[]
		{
			"Trigger radius squared is ",
			this.triggerRadiusSquared,
			" for '",
			this.path,
			"'"
		}));
	}

	private void Start()
	{
		this.OnStartOrEnable();
	}

	private void OnEnable()
	{
		this.OnStartOrEnable();
	}

	private void OnStartOrEnable()
	{
		if (!FMOD_Listener.HasLoadedBanks)
		{
			return;
		}
		if (FMOD_StudioSystem.instance)
		{
			this.TryGetEventDescription();
			if (this.startEventOnTriggerEnter)
			{
				this.CalculateTriggerRadiusSquared();
			}
			if (this.customRigidbody != null)
			{
				this.cachedRigidBody = this.customRigidbody;
			}
			else
			{
				this.cachedRigidBody = base.GetComponent<Rigidbody>();
			}
			if (this.startEventOnAwake)
			{
				this.StartEvent();
			}
		}
		this.isMoving = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.startEventOnTriggerEnter && this.triggerType == FMOD_StudioEventEmitter.TriggerType.Collider && other.CompareTag("Player"))
		{
			if (this.triggerEnterCount == 0)
			{
				base.StartCoroutine("StartEventWhenStopped");
			}
			this.triggerEnterCount++;
		}
	}

	[DebuggerHidden]
	private IEnumerator StartEventWhenStopped()
	{
		FMOD_StudioEventEmitter.<StartEventWhenStopped>c__Iterator0 <StartEventWhenStopped>c__Iterator = new FMOD_StudioEventEmitter.<StartEventWhenStopped>c__Iterator0();
		<StartEventWhenStopped>c__Iterator.<>f__this = this;
		return <StartEventWhenStopped>c__Iterator;
	}

	private void OnTriggerExit(Collider other)
	{
		if (this.startEventOnTriggerEnter && this.triggerType == FMOD_StudioEventEmitter.TriggerType.Collider && other.CompareTag("Player"))
		{
			this.triggerEnterCount--;
			if (this.triggerEnterCount == 0)
			{
				this.Stop();
			}
		}
	}

	private void OnDisable()
	{
		this.Stop();
		this.withinTriggerRadius = false;
		if (this.isMoving)
		{
			FMOD_StudioEventEmitterManager.Remove(this);
			this.isMoving = false;
		}
	}

	private void TryGetEventDescription()
	{
		if (!this.IsEventDescriptionValid() && FMOD_StudioSystem.instance)
		{
			if (string.IsNullOrEmpty(this.path))
			{
				UnityUtil.LogError("No path specified for Event Emitter");
				return;
			}
			if (UnityUtil.ERRCHECK(FMOD_StudioSystem.instance.System.getEvent(this.path, out this.eventDescription)))
			{
				if (this.ShouldCacheOnAwake())
				{
					UnityUtil.Log("Cache on awake: '" + this.path + "'");
					this.CacheEventInstance();
				}
			}
			else
			{
				UnityUtil.LogError("Failed to get event: '" + this.path + "'");
			}
		}
	}

	private void CacheEventInstance()
	{
		if (!this.IsEventDescriptionValid())
		{
			UnityUtil.LogWarning(string.Concat(new string[]
			{
				"Event description is ",
				(!(this.eventDescription == null)) ? "invalid" : "null",
				" for '",
				this.path,
				"'"
			}));
			return;
		}
		if (UnityUtil.ERRCHECK(this.eventDescription.createInstance(out this.evt)))
		{
			this.evt.getParameter("time", out this.timeParameter);
			this.evt.getParameter("wind", out this.windParameter);
		}
	}

	private void OnApplicationQuit()
	{
		FMOD_StudioEventEmitter.isShuttingDown = true;
	}

	private void OnDestroy()
	{
		if (FMOD_StudioEventEmitter.isShuttingDown)
		{
			return;
		}
		UnityUtil.Log("Destroy called");
		if (this.IsEventInstanceValid())
		{
			if (this.getPlaybackState() != PLAYBACK_STATE.STOPPED)
			{
				UnityUtil.Log("Release evt: '" + this.path + "'");
				this.ERRCHECK(this.evt.stop(STOP_MODE.IMMEDIATE));
			}
			this.ERRCHECK(this.evt.release());
			this.evt = null;
			this.timeParameter = null;
			this.windParameter = null;
		}
		this.preStartAction = null;
		this.eventDescription = null;
	}

	public void StartEvent()
	{
		if (!this.hasStarted)
		{
			if (!this.IsEventInstanceValid())
			{
				this.CacheEventInstance();
			}
			if (this.IsEventInstanceValid())
			{
				this.Update3DAttributes();
				if (this.timeParameter != null && this.timeParameter.isValid())
				{
					this.ERRCHECK(this.timeParameter.setValue(FMOD_StudioEventEmitter.HoursSinceMidnight));
				}
				if (this.windParameter != null && this.windParameter.isValid())
				{
					this.ERRCHECK(this.windParameter.setValue(FMOD_StudioEventEmitter.WindIntensity));
				}
				if (this.preStartAction != null)
				{
					this.preStartAction(this.evt);
				}
				this.ERRCHECK(this.evt.start());
				this.hasStarted = true;
			}
			else
			{
				UnityUtil.LogError("Event retrieval failed: '" + this.path + "'");
			}
		}
	}

	private bool IsEventDescriptionValid()
	{
		return this.eventDescription != null && this.eventDescription.isValid();
	}

	private bool IsEventInstanceValid()
	{
		return this.evt != null && this.evt.isValid();
	}

	private bool ShouldCacheOnAwake()
	{
		return this.startEventOnAwake || !this.startEventOnTriggerEnter;
	}

	private void Update()
	{
		if (!this.IsEventDescriptionValid())
		{
			return;
		}
		if (!this.ShouldCacheOnAwake() && FMOD_StudioEventEmitter.LocalPlayerTransform)
		{
			float sqrMagnitude = (FMOD_StudioEventEmitter.LocalPlayerTransform.position - base.transform.position).sqrMagnitude;
			if (this.IsEventInstanceValid())
			{
				if (sqrMagnitude > this.triggerRadiusSquared * 4f)
				{
					UnityUtil.ERRCHECK(this.evt.stop(STOP_MODE.ALLOWFADEOUT));
					UnityUtil.ERRCHECK(this.evt.release());
					this.evt = null;
				}
			}
			else if (sqrMagnitude < this.triggerRadiusSquared * 2.25f)
			{
				this.CacheEventInstance();
			}
			if (this.triggerType == FMOD_StudioEventEmitter.TriggerType.EventMaximumDistance)
			{
				if (this.withinTriggerRadius && sqrMagnitude > this.triggerRadiusSquared)
				{
					this.withinTriggerRadius = false;
					this.Stop();
				}
				else if (!this.withinTriggerRadius && sqrMagnitude <= this.triggerRadiusSquared)
				{
					this.withinTriggerRadius = true;
					base.StartCoroutine("StartEventWhenStopped");
				}
			}
		}
		if (this.IsEventInstanceValid())
		{
			if (base.transform.hasChanged)
			{
				this.Update3DAttributes();
				if (!this.isMoving)
				{
					FMOD_StudioEventEmitterManager.Add(this);
					this.isMoving = true;
				}
			}
			if (this.timeParameter != null && this.timeParameter.isValid())
			{
				this.ERRCHECK(this.timeParameter.setValue(FMOD_StudioEventEmitter.HoursSinceMidnight));
			}
			if (this.windParameter != null && this.windParameter.isValid())
			{
				this.ERRCHECK(this.windParameter.setValue(FMOD_StudioEventEmitter.WindIntensity));
			}
		}
		else
		{
			this.evt = null;
		}
	}

	public void StopMoving()
	{
		this.isMoving = false;
	}

	private void Update3DAttributes()
	{
		if (this.evt != null && this.evt.isValid())
		{
			FMOD.Studio.ATTRIBUTES_3D attributes = UnityUtil.to3DAttributes(base.gameObject, this.cachedRigidBody);
			this.ERRCHECK(this.evt.set3DAttributes(attributes));
		}
	}

	private RESULT ERRCHECK(RESULT result)
	{
		UnityUtil.ERRCHECK(result);
		return result;
	}

	public void TransplantEventInstance(Transform parent)
	{
		GameObject gameObject = new GameObject("Audio");
		gameObject.transform.SetParent(parent, false);
		gameObject.SetActive(false);
		FMOD_StudioEventEmitter fMOD_StudioEventEmitter = gameObject.AddComponent<FMOD_StudioEventEmitter>();
		fMOD_StudioEventEmitter.startEventOnAwake = false;
		fMOD_StudioEventEmitter.startEventOnTriggerEnter = false;
		fMOD_StudioEventEmitter.path = this.path;
		fMOD_StudioEventEmitter.eventDescription = this.eventDescription;
		fMOD_StudioEventEmitter.evt = this.evt;
		gameObject.SetActive(true);
		this.evt = null;
		this.Stop();
	}

	public static GameObject CreateStartOnAwakeEmitter(Transform parent, string eventPath)
	{
		return FMOD_StudioEventEmitter.CreateEmitter(parent, parent.position, eventPath, true, false);
	}

	public static GameObject CreateAmbientEmitter(Transform parent, Vector3 position, string eventPath)
	{
		return FMOD_StudioEventEmitter.CreateEmitter(parent, position, eventPath, false, true);
	}

	private static GameObject CreateEmitter(Transform parent, Vector3 position, string eventPath, bool startEventOnAwake, bool startEventOnTriggerEnter)
	{
		GameObject gameObject = new GameObject("Audio");
		gameObject.transform.parent = parent;
		gameObject.transform.position = position;
		gameObject.SetActive(false);
		FMOD_StudioEventEmitter fMOD_StudioEventEmitter = gameObject.AddComponent<FMOD_StudioEventEmitter>();
		fMOD_StudioEventEmitter.startEventOnAwake = startEventOnAwake;
		fMOD_StudioEventEmitter.startEventOnTriggerEnter = startEventOnTriggerEnter;
		fMOD_StudioEventEmitter.triggerType = FMOD_StudioEventEmitter.TriggerType.EventMaximumDistance;
		fMOD_StudioEventEmitter.path = eventPath;
		gameObject.SetActive(true);
		return gameObject;
	}
}
