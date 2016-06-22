using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1202)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct P2PSessionRequest_t
	{
		public const int k_iCallback = 1202;

		public CSteamID m_steamIDRemote;
	}
}
