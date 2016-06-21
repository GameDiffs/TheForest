using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SteamVR_GazeTracker : MonoBehaviour
{
	public bool isInGaze;

	public float gazeInCutoff = 0.15f;

	public float gazeOutCutoff = 0.4f;

	private Transform hmdTrackedObject;

	public event GazeEventHandler GazeOn
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.GazeOn = (GazeEventHandler)Delegate.Combine(this.GazeOn, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.GazeOn = (GazeEventHandler)Delegate.Remove(this.GazeOn, value);
		}
	}

	public event GazeEventHandler GazeOff
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.GazeOff = (GazeEventHandler)Delegate.Combine(this.GazeOff, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.GazeOff = (GazeEventHandler)Delegate.Remove(this.GazeOff, value);
		}
	}

	private void Start()
	{
	}

	public virtual void OnGazeOn(GazeEventArgs e)
	{
		if (this.GazeOn != null)
		{
			this.GazeOn(this, e);
		}
	}

	public virtual void OnGazeOff(GazeEventArgs e)
	{
		if (this.GazeOff != null)
		{
			this.GazeOff(this, e);
		}
	}

	private void Update()
	{
		if (this.hmdTrackedObject == null)
		{
			SteamVR_TrackedObject[] array = UnityEngine.Object.FindObjectsOfType<SteamVR_TrackedObject>();
			SteamVR_TrackedObject[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				SteamVR_TrackedObject steamVR_TrackedObject = array2[i];
				if (steamVR_TrackedObject.index == SteamVR_TrackedObject.EIndex.Hmd)
				{
					this.hmdTrackedObject = steamVR_TrackedObject.transform;
					break;
				}
			}
		}
		if (this.hmdTrackedObject)
		{
			Ray ray = new Ray(this.hmdTrackedObject.position, this.hmdTrackedObject.forward);
			Plane plane = new Plane(this.hmdTrackedObject.forward, base.transform.position);
			float d = 0f;
			if (plane.Raycast(ray, out d))
			{
				Vector3 a = this.hmdTrackedObject.position + this.hmdTrackedObject.forward * d;
				float num = Vector3.Distance(a, base.transform.position);
				if (num < this.gazeInCutoff && !this.isInGaze)
				{
					this.isInGaze = true;
					GazeEventArgs e;
					e.distance = num;
					this.OnGazeOn(e);
				}
				else if (num >= this.gazeOutCutoff && this.isInGaze)
				{
					this.isInGaze = false;
					GazeEventArgs e2;
					e2.distance = num;
					this.OnGazeOff(e2);
				}
			}
		}
	}
}
