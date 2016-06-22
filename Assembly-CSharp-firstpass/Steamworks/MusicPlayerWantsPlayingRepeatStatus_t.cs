using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4114)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct MusicPlayerWantsPlayingRepeatStatus_t
	{
		public const int k_iCallback = 4114;

		public int m_nPlayingRepeatStatus;
	}
}
