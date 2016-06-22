using System;

namespace Steamworks
{
	public static class SteamController
	{
		public static bool Init(string pchAbsolutePathToControllerConfigVDF)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchAbsolutePathToControllerConfigVDF))
			{
				result = NativeMethods.ISteamController_Init(uTF8StringHandle);
			}
			return result;
		}

		public static bool Shutdown()
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamController_Shutdown();
		}

		public static void RunFrame()
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamController_RunFrame();
		}

		public static bool GetControllerState(uint unControllerIndex, out SteamControllerState_t pState)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamController_GetControllerState(unControllerIndex, out pState);
		}

		public static void TriggerHapticPulse(uint unControllerIndex, ESteamControllerPad eTargetPad, ushort usDurationMicroSec)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamController_TriggerHapticPulse(unControllerIndex, eTargetPad, usDurationMicroSec);
		}

		public static void SetOverrideMode(string pchMode)
		{
			InteropHelp.TestIfAvailableClient();
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchMode))
			{
				NativeMethods.ISteamController_SetOverrideMode(uTF8StringHandle);
			}
		}
	}
}
