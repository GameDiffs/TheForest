using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(339)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct GameConnectedChatJoin_t
	{
		public const int k_iCallback = 339;

		public CSteamID m_steamIDClanChat;

		public CSteamID m_steamIDUser;
	}
}
