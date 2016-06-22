using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1329)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct RemoteStoragePublishFileProgress_t
	{
		public const int k_iCallback = 1329;

		public double m_dPercentFile;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bPreview;
	}
}
