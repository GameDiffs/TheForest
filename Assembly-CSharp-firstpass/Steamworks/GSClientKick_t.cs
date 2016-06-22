using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(203)]
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct GSClientKick_t
	{
		public const int k_iCallback = 203;

		public CSteamID m_SteamID;

		public EDenyReason m_eDenyReason;
	}
}
