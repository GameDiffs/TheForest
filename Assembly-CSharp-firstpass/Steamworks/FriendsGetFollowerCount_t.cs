using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(344)]
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct FriendsGetFollowerCount_t
	{
		public const int k_iCallback = 344;

		public EResult m_eResult;

		public CSteamID m_steamID;

		public int m_nCount;
	}
}
