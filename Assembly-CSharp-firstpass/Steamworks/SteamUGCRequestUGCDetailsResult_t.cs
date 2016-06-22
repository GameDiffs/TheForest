using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(3402)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamUGCRequestUGCDetailsResult_t
	{
		public const int k_iCallback = 3402;

		public SteamUGCDetails_t m_details;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bCachedData;
	}
}
