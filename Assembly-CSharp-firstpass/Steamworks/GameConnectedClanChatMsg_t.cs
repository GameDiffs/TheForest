using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(338)]
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct GameConnectedClanChatMsg_t
	{
		public const int k_iCallback = 338;

		public CSteamID m_steamIDClanChat;

		public CSteamID m_steamIDUser;

		public int m_iMessageID;
	}
}
