using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(115)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct GSPolicyResponse_t
	{
		public const int k_iCallback = 115;

		public byte m_bSecure;
	}
}
