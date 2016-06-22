using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(702)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct LowBatteryPower_t
	{
		public const int k_iCallback = 702;

		public byte m_nMinutesBatteryLeft;
	}
}
