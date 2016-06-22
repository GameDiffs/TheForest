using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(503)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct LobbyInvite_t
	{
		public const int k_iCallback = 503;

		public ulong m_ulSteamIDUser;

		public ulong m_ulSteamIDLobby;

		public ulong m_ulGameID;
	}
}
