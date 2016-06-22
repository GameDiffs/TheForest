using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4011)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct MusicPlayerWantsVolume_t
	{
		public const int k_iCallback = 4011;

		public float m_flNewVolume;
	}
}
