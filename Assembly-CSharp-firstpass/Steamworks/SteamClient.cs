using System;

namespace Steamworks
{
	public static class SteamClient
	{
		public static HSteamPipe CreateSteamPipe()
		{
			InteropHelp.TestIfAvailableClient();
			return (HSteamPipe)NativeMethods.ISteamClient_CreateSteamPipe();
		}

		public static bool BReleaseSteamPipe(HSteamPipe hSteamPipe)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamClient_BReleaseSteamPipe(hSteamPipe);
		}

		public static HSteamUser ConnectToGlobalUser(HSteamPipe hSteamPipe)
		{
			InteropHelp.TestIfAvailableClient();
			return (HSteamUser)NativeMethods.ISteamClient_ConnectToGlobalUser(hSteamPipe);
		}

		public static HSteamUser CreateLocalUser(out HSteamPipe phSteamPipe, EAccountType eAccountType)
		{
			InteropHelp.TestIfAvailableClient();
			return (HSteamUser)NativeMethods.ISteamClient_CreateLocalUser(out phSteamPipe, eAccountType);
		}

		public static void ReleaseUser(HSteamPipe hSteamPipe, HSteamUser hUser)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamClient_ReleaseUser(hSteamPipe, hUser);
		}

		public static IntPtr GetISteamUser(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamUser(hSteamUser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamGameServer(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamGameServer(hSteamUser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static void SetLocalIPBinding(uint unIP, ushort usPort)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamClient_SetLocalIPBinding(unIP, usPort);
		}

		public static IntPtr GetISteamFriends(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamFriends(hSteamUser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamUtils(HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamUtils(hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamMatchmaking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamMatchmaking(hSteamUser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamMatchmakingServers(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamMatchmakingServers(hSteamUser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamGenericInterface(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamGenericInterface(hSteamUser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamUserStats(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamUserStats(hSteamUser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamGameServerStats(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamGameServerStats(hSteamuser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamApps(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamApps(hSteamUser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamNetworking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamNetworking(hSteamUser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamRemoteStorage(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamRemoteStorage(hSteamuser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamScreenshots(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamScreenshots(hSteamuser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static void RunFrame()
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamClient_RunFrame();
		}

		public static uint GetIPCCallCount()
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamClient_GetIPCCallCount();
		}

		public static void SetWarningMessageHook(SteamAPIWarningMessageHook_t pFunction)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamClient_SetWarningMessageHook(pFunction);
		}

		public static bool BShutdownIfAllPipesClosed()
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamClient_BShutdownIfAllPipesClosed();
		}

		public static IntPtr GetISteamHTTP(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamHTTP(hSteamuser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamUnifiedMessages(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamUnifiedMessages(hSteamuser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamController(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamController(hSteamUser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamUGC(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamUGC(hSteamUser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamAppList(HSteamUser hSteamUser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamAppList(hSteamUser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamMusic(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamMusic(hSteamuser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamMusicRemote(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamMusicRemote(hSteamuser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamHTMLSurface(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamHTMLSurface(hSteamuser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static void Set_SteamAPI_CPostAPIResultInProcess(SteamAPI_PostAPIResultInProcess_t func)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamClient_Set_SteamAPI_CPostAPIResultInProcess(func);
		}

		public static void Remove_SteamAPI_CPostAPIResultInProcess(SteamAPI_PostAPIResultInProcess_t func)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamClient_Remove_SteamAPI_CPostAPIResultInProcess(func);
		}

		public static void Set_SteamAPI_CCheckCallbackRegisteredInProcess(SteamAPI_CheckCallbackRegistered_t func)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamClient_Set_SteamAPI_CCheckCallbackRegisteredInProcess(func);
		}

		public static IntPtr GetISteamInventory(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamInventory(hSteamuser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}

		public static IntPtr GetISteamVideo(HSteamUser hSteamuser, HSteamPipe hSteamPipe, string pchVersion)
		{
			InteropHelp.TestIfAvailableClient();
			IntPtr result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchVersion))
			{
				result = NativeMethods.ISteamClient_GetISteamVideo(hSteamuser, hSteamPipe, uTF8StringHandle);
			}
			return result;
		}
	}
}
