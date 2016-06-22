using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(510)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct LobbyMatchList_t
	{
		public const int k_iCallback = 510;

		public uint m_nLobbiesMatching;
	}
}
