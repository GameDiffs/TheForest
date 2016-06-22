using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(117)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct IPCFailure_t
	{
		public const int k_iCallback = 117;

		public byte m_eFailureType;
	}
}
