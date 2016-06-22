using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SteamControllerState_t
	{
		public uint unPacketNum;

		public ulong ulButtons;

		public short sLeftPadX;

		public short sLeftPadY;

		public short sRightPadX;

		public short sRightPadY;
	}
}
