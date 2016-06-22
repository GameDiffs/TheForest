using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(201)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct GSClientApprove_t
	{
		public const int k_iCallback = 201;

		public CSteamID m_SteamID;

		public CSteamID m_OwnerSteamID;
	}
}
