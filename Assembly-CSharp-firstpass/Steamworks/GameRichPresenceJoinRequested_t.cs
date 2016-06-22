using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(337)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct GameRichPresenceJoinRequested_t
	{
		public const int k_iCallback = 337;

		public CSteamID m_steamIDFriend;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string m_rgchConnect;
	}
}
