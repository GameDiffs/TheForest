using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1104)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct LeaderboardFindResult_t
	{
		public const int k_iCallback = 1104;

		public SteamLeaderboard_t m_hSteamLeaderboard;

		public byte m_bLeaderboardFound;
	}
}
