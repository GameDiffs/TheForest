using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(2501)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamUnifiedMessagesSendMethodResult_t
	{
		public const int k_iCallback = 2501;

		public ClientUnifiedMessageHandle m_hHandle;

		public ulong m_unContext;

		public EResult m_eResult;

		public uint m_unResponseSize;
	}
}
