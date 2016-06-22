using System;

namespace Steamworks
{
	public static class SteamGameServerStats
	{
		public static SteamAPICall_t RequestUserStats(CSteamID steamIDUser)
		{
			InteropHelp.TestIfAvailableGameServer();
			return (SteamAPICall_t)NativeMethods.ISteamGameServerStats_RequestUserStats(steamIDUser);
		}

		public static bool GetUserStat(CSteamID steamIDUser, string pchName, out int pData)
		{
			InteropHelp.TestIfAvailableGameServer();
			bool result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchName))
			{
				result = NativeMethods.ISteamGameServerStats_GetUserStat(steamIDUser, uTF8StringHandle, out pData);
			}
			return result;
		}

		public static bool GetUserStat(CSteamID steamIDUser, string pchName, out float pData)
		{
			InteropHelp.TestIfAvailableGameServer();
			bool result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchName))
			{
				result = NativeMethods.ISteamGameServerStats_GetUserStat_(steamIDUser, uTF8StringHandle, out pData);
			}
			return result;
		}

		public static bool GetUserAchievement(CSteamID steamIDUser, string pchName, out bool pbAchieved)
		{
			InteropHelp.TestIfAvailableGameServer();
			bool result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchName))
			{
				result = NativeMethods.ISteamGameServerStats_GetUserAchievement(steamIDUser, uTF8StringHandle, out pbAchieved);
			}
			return result;
		}

		public static bool SetUserStat(CSteamID steamIDUser, string pchName, int nData)
		{
			InteropHelp.TestIfAvailableGameServer();
			bool result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchName))
			{
				result = NativeMethods.ISteamGameServerStats_SetUserStat(steamIDUser, uTF8StringHandle, nData);
			}
			return result;
		}

		public static bool SetUserStat(CSteamID steamIDUser, string pchName, float fData)
		{
			InteropHelp.TestIfAvailableGameServer();
			bool result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchName))
			{
				result = NativeMethods.ISteamGameServerStats_SetUserStat_(steamIDUser, uTF8StringHandle, fData);
			}
			return result;
		}

		public static bool UpdateUserAvgRateStat(CSteamID steamIDUser, string pchName, float flCountThisSession, double dSessionLength)
		{
			InteropHelp.TestIfAvailableGameServer();
			bool result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchName))
			{
				result = NativeMethods.ISteamGameServerStats_UpdateUserAvgRateStat(steamIDUser, uTF8StringHandle, flCountThisSession, dSessionLength);
			}
			return result;
		}

		public static bool SetUserAchievement(CSteamID steamIDUser, string pchName)
		{
			InteropHelp.TestIfAvailableGameServer();
			bool result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchName))
			{
				result = NativeMethods.ISteamGameServerStats_SetUserAchievement(steamIDUser, uTF8StringHandle);
			}
			return result;
		}

		public static bool ClearUserAchievement(CSteamID steamIDUser, string pchName)
		{
			InteropHelp.TestIfAvailableGameServer();
			bool result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchName))
			{
				result = NativeMethods.ISteamGameServerStats_ClearUserAchievement(steamIDUser, uTF8StringHandle);
			}
			return result;
		}

		public static SteamAPICall_t StoreUserStats(CSteamID steamIDUser)
		{
			InteropHelp.TestIfAvailableGameServer();
			return (SteamAPICall_t)NativeMethods.ISteamGameServerStats_StoreUserStats(steamIDUser);
		}
	}
}
