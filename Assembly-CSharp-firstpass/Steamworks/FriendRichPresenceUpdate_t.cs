using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(336)]
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct FriendRichPresenceUpdate_t
	{
		public const int k_iCallback = 336;

		public CSteamID m_steamIDFriend;

		public AppId_t m_nAppID;
	}
}
