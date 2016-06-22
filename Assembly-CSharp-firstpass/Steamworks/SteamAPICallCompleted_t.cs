using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(703)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamAPICallCompleted_t
	{
		public const int k_iCallback = 703;

		public SteamAPICall_t m_hAsyncCall;
	}
}
