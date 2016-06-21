using System;
using TheForest.Tools;
using UnityEngine;

public class AnimalTypeTrigger : MonoBehaviour
{
	public AnimalType _type;

	private float _inspectStartTime;

	private float _inspectLostStartTime = -1f;

	private void Awake()
	{
		base.enabled = false;
	}

	private void Update()
	{
		if (this._inspectLostStartTime > 0f)
		{
			if (Time.realtimeSinceStartup - this._inspectLostStartTime > 1f)
			{
				base.enabled = false;
			}
		}
		else if (Time.realtimeSinceStartup - this._inspectStartTime > 4f)
		{
			base.enabled = false;
			base.GetComponent<Collider>().enabled = false;
			EventRegistry.Player.Publish(TfEvent.InspectedAnimal, this._type);
		}
	}

	private void GrabEnter()
	{
		this._inspectLostStartTime = -1f;
		if (!base.enabled)
		{
			this._inspectStartTime = Time.realtimeSinceStartup;
			base.enabled = true;
		}
	}

	private void GrabExit()
	{
		this._inspectLostStartTime = Time.realtimeSinceStartup;
	}
}
