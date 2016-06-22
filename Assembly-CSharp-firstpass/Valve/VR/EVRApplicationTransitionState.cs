using System;

namespace Valve.VR
{
	public enum EVRApplicationTransitionState
	{
		VRApplicationTransition_None,
		VRApplicationTransition_OldAppQuitSent = 10,
		VRApplicationTransition_WaitingForExternalLaunch,
		VRApplicationTransition_NewAppLaunched = 20
	}
}
