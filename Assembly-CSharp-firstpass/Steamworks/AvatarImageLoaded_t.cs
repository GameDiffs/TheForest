using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(334)]
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct AvatarImageLoaded_t
	{
		public const int k_iCallback = 334;

		public CSteamID m_steamID;

		public int m_iImage;

		public int m_iWide;

		public int m_iTall;
	}
}
