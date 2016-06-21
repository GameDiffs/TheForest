using FMOD.Studio;
using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

public class PhysicsSfx : MonoBehaviour
{
	private const float SLIDE_STOP_DELAY = 0.05f;

	private const float SLIDE_SAFETY_TIMEOUT = 10f;

	public float impactSpeedThreshold = 2f;

	public float impactRetriggerTimeout = 0.5f;

	public float slideSpeedThreshold = 2f;

	public float slideRetriggerTimeout = 0.5f;

	public float delayAfterSpawn = 2f;

	[Header("FMOD")]
	public string impactEvent;

	public string slideEvent;

	private bool waitingForSpawnTimeout = true;

	private bool hasPreloaded;

	private float sqrImpactSpeedThreshold;

	private bool impactEnabled = true;

	private bool slideEnabled = true;

	private float sqrSlideSpeedThreshold;

	private EventInstance slideEventInstance;

	private Collider collider;

	private Rigidbody rigidbody;

	private List<GameObject> slideObjects;

	private static HashSet<string> StationaryTags = new HashSet<string>
	{
		"TerrainMain",
		"SLTier1",
		"SLTier2",
		"SLTier3",
		"DeadTree",
		"Target",
		"UnderfootRock",
		"BreakableRock",
		"Block",
		"UnderfootMetal",
		"UnderfootCarpet",
		"UnderfootDirt"
	};

	private static HashSet<string> CheckComponentTags = new HashSet<string>
	{
		"UnderfootWood",
		"BreakableWood",
		"structure",
		"jumpObject"
	};

	private bool isStoppingSlideEvent;

	private void OnEnable()
	{
		if (this.delayAfterSpawn > 0f)
		{
			this.waitingForSpawnTimeout = true;
			base.Invoke("EndSpawnTimeout", this.delayAfterSpawn);
		}
		else
		{
			this.waitingForSpawnTimeout = false;
		}
		this.sqrImpactSpeedThreshold = this.impactSpeedThreshold * this.impactSpeedThreshold;
		this.sqrSlideSpeedThreshold = this.slideSpeedThreshold * this.slideSpeedThreshold;
		this.collider = base.GetComponent<Collider>();
		this.rigidbody = base.GetComponent<Rigidbody>();
		if (!this.rigidbody)
		{
			base.enabled = false;
			return;
		}
		this.slideObjects = new List<GameObject>();
		FMODCommon.PreloadEvents(new string[]
		{
			this.impactEvent,
			this.slideEvent
		});
		this.hasPreloaded = true;
	}

	private void OnDisable()
	{
		if (this.slideEventInstance != null)
		{
			this.StopSlideEvent();
			base.CancelInvoke("StopSlideEvent");
		}
		if (this.hasPreloaded)
		{
			FMODCommon.UnloadEvents(new string[]
			{
				this.impactEvent,
				this.slideEvent
			});
			this.hasPreloaded = false;
		}
	}

	private void EndSpawnTimeout()
	{
		this.waitingForSpawnTimeout = false;
	}

	private static bool IsSlideObject(GameObject gameObject)
	{
		return PhysicsSfx.StationaryTags.Contains(gameObject.tag) || (PhysicsSfx.CheckComponentTags.Contains(gameObject.tag) && gameObject.GetComponentInParent<DisableSlideSFX>() == null);
	}

	private void OnCollisionEnter(Collision collision)
	{
		GameObject gameObject = collision.gameObject;
		if (PhysicsSfx.IsSlideObject(gameObject))
		{
			this.slideObjects.Add(gameObject);
		}
		if (this.waitingForSpawnTimeout)
		{
			return;
		}
		if (collision.contacts.Length < 1)
		{
			return;
		}
		if (gameObject.CompareTag("Player"))
		{
			return;
		}
		if (this.impactEnabled && collision.relativeVelocity.sqrMagnitude > this.sqrImpactSpeedThreshold)
		{
			FMODCommon.PlayOneshot(this.impactEvent, collision.contacts[0].point, new object[]
			{
				"speed",
				collision.relativeVelocity.magnitude
			});
			this.impactEnabled = false;
			base.Invoke("EnableImpact", this.impactRetriggerTimeout);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		GameObject gameObject = collision.gameObject;
		if (PhysicsSfx.IsSlideObject(gameObject))
		{
			this.slideObjects.Remove(gameObject);
		}
	}

	private void UpdateSlideEvent()
	{
		UnityUtil.ERRCHECK(this.slideEventInstance.setParameterValue("speed", this.rigidbody.velocity.magnitude));
		Vector3 pos = this.collider.ClosestPointOnBounds(LocalPlayer.Transform.position);
		UnityUtil.ERRCHECK(this.slideEventInstance.set3DAttributes(pos.to3DAttributes()));
	}

	private void TryStartSlideEvent()
	{
		if (this.slideEnabled && this.slideEvent != null && this.slideEvent.Length > 0 && FMOD_StudioSystem.instance)
		{
			this.slideEventInstance = FMOD_StudioSystem.instance.GetEvent(this.slideEvent);
			if (this.slideEventInstance != null)
			{
				this.UpdateSlideEvent();
				UnityUtil.ERRCHECK(this.slideEventInstance.start());
				base.Invoke("StopSlideEventWithoutReleasing", 10f);
				this.slideEnabled = false;
				base.Invoke("EnableSlide", this.slideRetriggerTimeout);
			}
		}
	}

	private void StopSlideEvent()
	{
		UnityUtil.ERRCHECK(this.slideEventInstance.setParameterValue("stop", 1f));
		UnityUtil.ERRCHECK(this.slideEventInstance.release());
		this.slideEventInstance = null;
		this.isStoppingSlideEvent = false;
		base.CancelInvoke("StopSlideEventWithoutReleasing");
	}

	private void StopSlideEventWithoutReleasing()
	{
		if (this.slideEventInstance != null)
		{
			UnityUtil.ERRCHECK(this.slideEventInstance.setParameterValue("stop", 1f));
		}
	}

	private void Update()
	{
		if (this.waitingForSpawnTimeout)
		{
			return;
		}
		if (!this.rigidbody)
		{
			base.enabled = false;
			return;
		}
		if (this.slideObjects.Count > 0 && this.rigidbody.velocity.sqrMagnitude > this.sqrSlideSpeedThreshold)
		{
			if (this.slideEventInstance == null)
			{
				this.TryStartSlideEvent();
			}
			else
			{
				if (this.isStoppingSlideEvent)
				{
					base.CancelInvoke("StopSlideEvent");
					this.isStoppingSlideEvent = false;
				}
				this.UpdateSlideEvent();
			}
		}
		else if (this.slideEventInstance != null)
		{
			this.UpdateSlideEvent();
			if (!this.isStoppingSlideEvent)
			{
				base.Invoke("StopSlideEvent", 0.05f);
				this.isStoppingSlideEvent = true;
			}
		}
	}

	private void EnableImpact()
	{
		this.impactEnabled = true;
	}

	private void EnableSlide()
	{
		this.slideEnabled = true;
	}
}
