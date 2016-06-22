using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1112)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct GlobalStatsReceived_t
	{
		public const int k_iCallback = 1112;

		public ulong m_nGameID;

		public EResult m_eResult;
	}
}
