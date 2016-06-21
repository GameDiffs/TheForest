using FMOD.Studio;
using System;
using UnityEngine;

public class TreeWindSfx : MonoBehaviour
{
	[Tooltip("Path of FMOD event to play")]
	public string EventPath;

	public float size;

	private EventInstance WindEvent;

	private int WindParameterIndex = -1;

	private int SizeParameterIndex = -1;

	private int TimeParameterIndex = -1;

	private float Timeout;

	private bool WaitingForTimeout;

	public bool IsActive
	{
		get
		{
			return this.WindEvent != null;
		}
	}

	private void OnEnable()
	{
		TreeWindSfxManager.Add(this);
		EventDescription eventDescription = FMOD_StudioSystem.instance.GetEventDescription(this.EventPath);
		if (eventDescription != null)
		{
			this.WindParameterIndex = FMODCommon.FindParameterIndex(eventDescription, "wind");
			this.SizeParameterIndex = FMODCommon.FindParameterIndex(eventDescription, "size");
			this.TimeParameterIndex = FMODCommon.FindParameterIndex(eventDescription, "time");
		}
	}

	private void OnDisable()
	{
		TreeWindSfx.StopEvent(this.WindEvent);
		TreeWindSfxManager.Remove(this);
	}

	public static void StopEvent(EventInstance evt)
	{
		if (evt != null && evt.isValid())
		{
			UnityUtil.ERRCHECK(evt.stop(STOP_MODE.ALLOWFADEOUT));
			UnityUtil.ERRCHECK(evt.release());
		}
	}

	public void Activate()
	{
		if (FMOD_StudioSystem.instance)
		{
			if (this.WindEvent == null || !this.WindEvent.isValid())
			{
				this.WindEvent = FMOD_StudioSystem.instance.GetEvent(this.EventPath);
				UnityUtil.ERRCHECK(this.WindEvent.set3DAttributes(UnityUtil.to3DAttributes(base.gameObject, null)));
				if (this.WindParameterIndex >= 0)
				{
					UnityUtil.ERRCHECK(this.WindEvent.setParameterValueByIndex(this.WindParameterIndex, TheForestAtmosphere.Instance.WindIntensity));
				}
				if (this.SizeParameterIndex >= 0)
				{
					UnityUtil.ERRCHECK(this.WindEvent.setParameterValueByIndex(this.SizeParameterIndex, this.size));
				}
				if (this.TimeParameterIndex >= 0)
				{
					UnityUtil.ERRCHECK(this.WindEvent.setParameterValueByIndex(this.TimeParameterIndex, FMOD_StudioEventEmitter.HoursSinceMidnight));
				}
				UnityUtil.ERRCHECK(this.WindEvent.start());
			}
			else
			{
				if (this.WindParameterIndex >= 0)
				{
					UnityUtil.ERRCHECK(this.WindEvent.setParameterValueByIndex(this.WindParameterIndex, TheForestAtmosphere.Instance.WindIntensity));
				}
				if (this.TimeParameterIndex >= 0)
				{
					UnityUtil.ERRCHECK(this.WindEvent.setParameterValueByIndex(this.TimeParameterIndex, FMOD_StudioEventEmitter.HoursSinceMidnight));
				}
			}
			this.WaitingForTimeout = false;
		}
	}

	public void Deactivate(float persistTime)
	{
		if (this.WaitingForTimeout)
		{
			if (Time.time >= this.Timeout)
			{
				TreeWindSfx.StopEvent(this.WindEvent);
				this.WindEvent = null;
			}
		}
		else
		{
			this.Timeout = Time.time + persistTime;
			this.WaitingForTimeout = true;
		}
	}

	public static EventInstance BeginTransfer(Transform source)
	{
		EventInstance result = null;
		if (source != null)
		{
			TreeWindSfx componentInChildren = source.GetComponentInChildren<TreeWindSfx>();
			if (componentInChildren != null)
			{
				result = componentInChildren.WindEvent;
				componentInChildren.WindEvent = null;
			}
		}
		return result;
	}

	public static void CompleteTransfer(Transform destination, EventInstance windEvent)
	{
		if (windEvent != null)
		{
			TreeWindSfx treeWindSfx = null;
			if (destination != null)
			{
				treeWindSfx = destination.GetComponentInChildren<TreeWindSfx>();
			}
			if (treeWindSfx != null)
			{
				treeWindSfx.WindEvent = windEvent;
			}
			else
			{
				TreeWindSfx.StopEvent(windEvent);
			}
			windEvent = null;
		}
	}
}
