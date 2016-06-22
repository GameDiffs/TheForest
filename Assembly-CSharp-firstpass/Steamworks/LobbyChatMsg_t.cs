using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(507)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct LobbyChatMsg_t
	{
		public const int k_iCallback = 507;

		public ulong m_ulSteamIDLobby;

		public ulong m_ulSteamIDUser;

		public byte m_eChatEntryType;

		public uint m_iChatID;
	}
}
