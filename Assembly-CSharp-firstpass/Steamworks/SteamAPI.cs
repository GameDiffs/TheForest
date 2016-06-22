using System;

namespace Steamworks
{
	public static class SteamAPI
	{
		private static bool _initialized;

		public static bool RestartAppIfNecessary(AppId_t unOwnAppID)
		{
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamAPI_RestartAppIfNecessary(unOwnAppID);
		}

		public static bool InitSafe()
		{
			return SteamAPI.Init();
		}

		public static bool Init()
		{
			if (SteamAPI._initialized)
			{
				throw new Exception("Tried to Initialize Steamworks twice in one session!");
			}
			InteropHelp.TestIfPlatformSupported();
			SteamAPI._initialized = NativeMethods.SteamAPI_InitSafe();
			return SteamAPI._initialized;
		}

		public static void Shutdown()
		{
			InteropHelp.TestIfPlatformSupported();
			NativeMethods.SteamAPI_Shutdown();
		}

		public static void RunCallbacks()
		{
			InteropHelp.TestIfPlatformSupported();
			NativeMethods.SteamAPI_RunCallbacks();
		}

		public static bool IsSteamRunning()
		{
			InteropHelp.TestIfPlatformSupported();
			return NativeMethods.SteamAPI_IsSteamRunning();
		}

		public static HSteamUser GetHSteamUserCurrent()
		{
			InteropHelp.TestIfPlatformSupported();
			return (HSteamUser)NativeMethods.Steam_GetHSteamUserCurrent();
		}

		public static HSteamPipe GetHSteamPipe()
		{
			InteropHelp.TestIfPlatformSupported();
			return (HSteamPipe)NativeMethods.SteamAPI_GetHSteamPipe();
		}

		public static HSteamUser GetHSteamUser()
		{
			InteropHelp.TestIfPlatformSupported();
			return (HSteamUser)NativeMethods.SteamAPI_GetHSteamUser();
		}
	}
}
