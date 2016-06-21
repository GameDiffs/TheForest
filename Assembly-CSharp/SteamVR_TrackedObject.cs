using System;
using UnityEngine;
using Valve.VR;

public class SteamVR_TrackedObject : MonoBehaviour
{
	public enum EIndex
	{
		None = -1,
		Hmd,
		Device1,
		Device2,
		Device3,
		Device4,
		Device5,
		Device6,
		Device7,
		Device8,
		Device9,
		Device10,
		Device11,
		Device12,
		Device13,
		Device14,
		Device15
	}

	public SteamVR_TrackedObject.EIndex index;

	public Transform origin;

	public bool isValid;

	private void OnNewPoses(params object[] args)
	{
		if (this.index == SteamVR_TrackedObject.EIndex.None)
		{
			return;
		}
		int num = (int)this.index;
		this.isValid = false;
		TrackedDevicePose_t[] array = (TrackedDevicePose_t[])args[0];
		if (array.Length <= num)
		{
			return;
		}
		if (!array[num].bDeviceIsConnected)
		{
			return;
		}
		if (!array[num].bPoseIsValid)
		{
			return;
		}
		this.isValid = true;
		SteamVR_Utils.RigidTransform b = new SteamVR_Utils.RigidTransform(array[num].mDeviceToAbsoluteTracking);
		if (this.origin != null)
		{
			b = new SteamVR_Utils.RigidTransform(this.origin) * b;
			b.pos.x = b.pos.x * this.origin.localScale.x;
			b.pos.y = b.pos.y * this.origin.localScale.y;
			b.pos.z = b.pos.z * this.origin.localScale.z;
			base.transform.position = b.pos;
			base.transform.rotation = b.rot;
		}
		else
		{
			base.transform.localPosition = b.pos;
			base.transform.localRotation = b.rot;
		}
	}

	private void OnEnable()
	{
		SteamVR_Render instance = SteamVR_Render.instance;
		if (instance == null)
		{
			base.enabled = false;
			return;
		}
		SteamVR_Utils.Event.Listen("new_poses", new SteamVR_Utils.Event.Handler(this.OnNewPoses));
	}

	private void OnDisable()
	{
		SteamVR_Utils.Event.Remove("new_poses", new SteamVR_Utils.Event.Handler(this.OnNewPoses));
	}

	public void SetDeviceIndex(int index)
	{
		if (Enum.IsDefined(typeof(SteamVR_TrackedObject.EIndex), index))
		{
			this.index = (SteamVR_TrackedObject.EIndex)index;
		}
	}
}
