using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(343)]
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct GameConnectedFriendChatMsg_t
	{
		public const int k_iCallback = 343;

		public CSteamID m_steamIDUser;

		public int m_iMessageID;
	}
}
