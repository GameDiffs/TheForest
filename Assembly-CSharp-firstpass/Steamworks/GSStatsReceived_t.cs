using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1800)]
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct GSStatsReceived_t
	{
		public const int k_iCallback = 1800;

		public EResult m_eResult;

		public CSteamID m_steamIDUser;
	}
}
