using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(101)]
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 1)]
	public struct SteamServersConnected_t
	{
		public const int k_iCallback = 101;
	}
}
