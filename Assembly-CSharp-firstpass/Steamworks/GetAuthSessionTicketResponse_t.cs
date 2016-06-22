using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(163)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct GetAuthSessionTicketResponse_t
	{
		public const int k_iCallback = 163;

		public HAuthTicket m_hAuthTicket;

		public EResult m_eResult;
	}
}
