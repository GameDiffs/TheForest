using System;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(AudioListener))]
public class SteamVR_Ears : MonoBehaviour
{
	public SteamVR_Camera vrcam;

	private bool usingSpeakers;

	private Quaternion offset;

	private void OnNewPosesApplied(params object[] args)
	{
		Transform origin = this.vrcam.origin;
		Quaternion lhs = (!(origin != null)) ? Quaternion.identity : origin.rotation;
		base.transform.rotation = lhs * this.offset;
	}

	private void OnEnable()
	{
		this.usingSpeakers = false;
		CVRSettings settings = OpenVR.Settings;
		if (settings != null)
		{
			EVRSettingsError eVRSettingsError = EVRSettingsError.None;
			if (settings.GetBool("steamvr", "usingSpeakers", false, ref eVRSettingsError))
			{
				this.usingSpeakers = true;
				float @float = settings.GetFloat("steamvr", "speakersForwardYawOffsetDegrees", 0f, ref eVRSettingsError);
				this.offset = Quaternion.Euler(0f, @float, 0f);
			}
		}
		if (this.usingSpeakers)
		{
			SteamVR_Utils.Event.Listen("new_poses_applied", new SteamVR_Utils.Event.Handler(this.OnNewPosesApplied));
		}
	}

	private void OnDisable()
	{
		if (this.usingSpeakers)
		{
			SteamVR_Utils.Event.Remove("new_poses_applied", new SteamVR_Utils.Event.Handler(this.OnNewPosesApplied));
		}
	}
}
