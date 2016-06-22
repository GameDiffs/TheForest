using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4107)]
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 1)]
	public struct MusicPlayerWantsPlayPrevious_t
	{
		public const int k_iCallback = 4107;
	}
}
