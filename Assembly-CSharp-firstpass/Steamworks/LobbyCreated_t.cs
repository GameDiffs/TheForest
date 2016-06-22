using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(513)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct LobbyCreated_t
	{
		public const int k_iCallback = 513;

		public EResult m_eResult;

		public ulong m_ulSteamIDLobby;
	}
}
