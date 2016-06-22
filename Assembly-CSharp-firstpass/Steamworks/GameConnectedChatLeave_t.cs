using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(340)]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct GameConnectedChatLeave_t
	{
		public const int k_iCallback = 340;

		public CSteamID m_steamIDClanChat;

		public CSteamID m_steamIDUser;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bKicked;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bDropped;
	}
}
