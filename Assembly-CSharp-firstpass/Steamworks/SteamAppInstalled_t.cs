using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(3901)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamAppInstalled_t
	{
		public const int k_iCallback = 3901;

		public AppId_t m_nAppID;
	}
}
