using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(207)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct GSGameplayStats_t
	{
		public const int k_iCallback = 207;

		public EResult m_eResult;

		public int m_nRank;

		public uint m_unTotalConnects;

		public uint m_unTotalMinutesPlayed;
	}
}
