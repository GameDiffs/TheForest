using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4103)]
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 1)]
	public struct MusicPlayerRemoteToFront_t
	{
		public const int k_iCallback = 4103;
	}
}
