using System;

namespace Steamworks
{
	public static class GameServer
	{
		public static bool InitSafe(uint unIP, ushort usSteamPort, ushort usGamePort, ushort usQueryPort, EServerMode eServerMode, string pchVersionString)
		{
			InteropHelp.TestIfPlatformSupported();
			bool result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersionString))
			{
				result = NativeMethods.SteamGameServer_InitSafe(unIP, usSteamPort, usGamePort, usQueryPort, eServerMode, uTF8StringHandle);
			}
			return result;
		}

		public static bool Init(uint unIP, ushort usSteamPort, ushort usGamePort, ushort usQueryPort, EServerMode eServerMode, string pchVersionString)
		{
			InteropHelp.TestIfPlatformSupported();
			bool result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersionString))
			{
				result = NativeMethods.SteamGameServer_InitSafe(unIP, usSteamPort, usGamePort, usQueryPort, eServerMode, uTF8StringHandle);
			}
			return result;
		}

		public static void Shutdown()
		{
			InteropHelp.TestIfPlatformSupported();
			NativeMethods.SteamGameServer_Shutdown();
		}

		public static void RunCallbacks()
		{
			InteropHelp.TestIfPlatformSupported();
			NativeMethods.SteamGameServer_RunCallbacks();
		}

		public static bool BSecure()
		{
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamGameServer_BSecure();
		}

		public static CSteamID GetSteamID()
		{
			InteropHelp.TestIfPlatformSupported();
			return (CSteamID)NativeMethods.SteamGameServer_GetSteamID();
		}

		public static HSteamPipe GetHSteamPipe()
		{
			InteropHelp.TestIfPlatformSupported();
			return (HSteamPipe)NativeMethods.SteamGameServer_GetHSteamPipe();
		}

		public static HSteamUser GetHSteamUser()
		{
			InteropHelp.TestIfPlatformSupported();
			return (HSteamUser)NativeMethods.SteamGameServer_GetHSteamUser();
		}
	}
}
