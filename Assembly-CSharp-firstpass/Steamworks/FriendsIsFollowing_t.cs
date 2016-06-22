using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(345)]
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct FriendsIsFollowing_t
	{
		public const int k_iCallback = 345;

		public EResult m_eResult;

		public CSteamID m_steamID;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bIsFollowing;
	}
}
