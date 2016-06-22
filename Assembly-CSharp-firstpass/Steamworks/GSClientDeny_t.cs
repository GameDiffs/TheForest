using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(202)]
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct GSClientDeny_t
	{
		public const int k_iCallback = 202;

		public CSteamID m_SteamID;

		public EDenyReason m_eDenyReason;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
		public string m_rgchOptionalText;
	}
}
