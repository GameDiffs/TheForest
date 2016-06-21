using System;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(Camera))]
public class SteamVR_UpdatePoses : MonoBehaviour
{
	private void Awake()
	{
		Camera component = base.GetComponent<Camera>();
		component.clearFlags = CameraClearFlags.Nothing;
		component.useOcclusionCulling = false;
		component.cullingMask = 0;
		component.depth = -9999f;
	}

	private void OnPreCull()
	{
		CVRCompositor compositor = OpenVR.Compositor;
		if (compositor != null)
		{
			SteamVR_Render instance = SteamVR_Render.instance;
			compositor.GetLastPoses(instance.poses, instance.gamePoses);
			SteamVR_Utils.Event.Send("new_poses", new object[]
			{
				instance.poses
			});
			SteamVR_Utils.Event.Send("new_poses_applied", new object[0]);
		}
	}
}
