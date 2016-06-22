using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(2102)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTTPRequestHeadersReceived_t
	{
		public const int k_iCallback = 2102;

		public HTTPRequestHandle m_hRequest;

		public ulong m_ulContextValue;
	}
}
