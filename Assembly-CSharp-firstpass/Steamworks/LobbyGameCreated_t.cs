using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(509)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct LobbyGameCreated_t
	{
		public const int k_iCallback = 509;

		public ulong m_ulSteamIDLobby;

		public ulong m_ulSteamIDGameServer;

		public uint m_unIP;

		public ushort m_usPort;
	}
}
