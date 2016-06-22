using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(331)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct GameOverlayActivated_t
	{
		public const int k_iCallback = 331;

		public byte m_bActive;
	}
}
