using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(3902)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamAppUninstalled_t
	{
		public const int k_iCallback = 3902;

		public AppId_t m_nAppID;
	}
}
