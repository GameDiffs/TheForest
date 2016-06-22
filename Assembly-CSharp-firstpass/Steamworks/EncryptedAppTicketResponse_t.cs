using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(154)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct EncryptedAppTicketResponse_t
	{
		public const int k_iCallback = 154;

		public EResult m_eResult;
	}
}
