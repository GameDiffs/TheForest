using System;

namespace Valve.VR
{
	public enum EDeviceActivityLevel
	{
		k_EDeviceActivityLevel_Unknown = -1,
		k_EDeviceActivityLevel_Idle,
		k_EDeviceActivityLevel_UserInteraction,
		k_EDeviceActivityLevel_UserInteraction_Timeout,
		k_EDeviceActivityLevel_Standby
	}
}
