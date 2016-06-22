using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4701)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamInventoryFullUpdate_t
	{
		public const int k_iCallback = 4701;

		public SteamInventoryResult_t m_handle;
	}
}
