using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4110)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct MusicPlayerWantsLooped_t
	{
		public const int k_iCallback = 4110;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bLooped;
	}
}
