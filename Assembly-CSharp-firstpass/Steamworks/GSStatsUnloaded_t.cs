using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1108)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct GSStatsUnloaded_t
	{
		public const int k_iCallback = 1108;

		public CSteamID m_steamIDUser;
	}
}
