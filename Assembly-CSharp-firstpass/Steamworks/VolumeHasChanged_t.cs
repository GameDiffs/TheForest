using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4002)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct VolumeHasChanged_t
	{
		public const int k_iCallback = 4002;

		public float m_flNewVolume;
	}
}
