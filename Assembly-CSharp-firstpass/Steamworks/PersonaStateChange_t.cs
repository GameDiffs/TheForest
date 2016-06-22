using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(304)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct PersonaStateChange_t
	{
		public const int k_iCallback = 304;

		public ulong m_ulSteamID;

		public EPersonaChange m_nChangeFlags;
	}
}
