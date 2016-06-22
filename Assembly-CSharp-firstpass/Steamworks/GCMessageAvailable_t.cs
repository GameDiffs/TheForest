using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1701)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct GCMessageAvailable_t
	{
		public const int k_iCallback = 1701;

		public uint m_nMessageSize;
	}
}
