using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(152)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct MicroTxnAuthorizationResponse_t
	{
		public const int k_iCallback = 152;

		public uint m_unAppID;

		public ulong m_ulOrderID;

		public byte m_bAuthorized;
	}
}
