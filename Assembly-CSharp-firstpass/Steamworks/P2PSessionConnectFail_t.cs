using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1203)]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct P2PSessionConnectFail_t
	{
		public const int k_iCallback = 1203;

		public CSteamID m_steamIDRemote;

		public byte m_eP2PSessionError;
	}
}
