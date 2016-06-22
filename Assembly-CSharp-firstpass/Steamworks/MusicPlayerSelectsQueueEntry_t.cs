using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4012)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct MusicPlayerSelectsQueueEntry_t
	{
		public const int k_iCallback = 4012;

		public int nID;
	}
}
