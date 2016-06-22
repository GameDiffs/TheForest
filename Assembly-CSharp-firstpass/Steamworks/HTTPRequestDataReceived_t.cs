using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(2103)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTTPRequestDataReceived_t
	{
		public const int k_iCallback = 2103;

		public HTTPRequestHandle m_hRequest;

		public ulong m_ulContextValue;

		public uint m_cOffset;

		public uint m_cBytesReceived;
	}
}
