using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(208)]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct GSClientGroupStatus_t
	{
		public const int k_iCallback = 208;

		public CSteamID m_SteamIDUser;

		public CSteamID m_SteamIDGroup;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bMember;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bOfficer;
	}
}
