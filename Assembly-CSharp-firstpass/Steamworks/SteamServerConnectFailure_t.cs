using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(102)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamServerConnectFailure_t
	{
		public const int k_iCallback = 102;

		public EResult m_eResult;
	}
}
