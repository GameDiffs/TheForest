using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1106)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct LeaderboardScoreUploaded_t
	{
		public const int k_iCallback = 1106;

		public byte m_bSuccess;

		public SteamLeaderboard_t m_hSteamLeaderboard;

		public int m_nScore;

		public byte m_bScoreChanged;

		public int m_nGlobalRankNew;

		public int m_nGlobalRankPrevious;
	}
}
