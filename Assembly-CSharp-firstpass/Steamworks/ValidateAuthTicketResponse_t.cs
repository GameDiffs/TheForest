using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(143)]
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct ValidateAuthTicketResponse_t
	{
		public const int k_iCallback = 143;

		public CSteamID m_SteamID;

		public EAuthSessionResponse m_eAuthSessionResponse;

		public CSteamID m_OwnerSteamID;
	}
}
