using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(335)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct ClanOfficerListResponse_t
	{
		public const int k_iCallback = 335;

		public CSteamID m_steamIDClan;

		public int m_cOfficers;

		public byte m_bSuccess;
	}
}
