using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(103)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SteamServersDisconnected_t
	{
		public const int k_iCallback = 103;

		public EResult m_eResult;
	}
}
