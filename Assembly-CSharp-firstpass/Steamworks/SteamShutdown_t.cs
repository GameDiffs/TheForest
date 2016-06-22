using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(704)]
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 1)]
	public struct SteamShutdown_t
	{
		public const int k_iCallback = 704;
	}
}
