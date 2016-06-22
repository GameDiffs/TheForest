using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(341)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct DownloadClanActivityCountsResult_t
	{
		public const int k_iCallback = 341;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bSuccess;
	}
}
