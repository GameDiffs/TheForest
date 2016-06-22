using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4013)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct MusicPlayerSelectsPlaylistEntry_t
	{
		public const int k_iCallback = 4013;

		public int nID;
	}
}
