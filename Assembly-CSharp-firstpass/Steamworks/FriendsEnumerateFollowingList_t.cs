using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(346)]
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct FriendsEnumerateFollowingList_t
	{
		public const int k_iCallback = 346;

		public EResult m_eResult;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
		public CSteamID[] m_rgSteamID;

		public int m_nResultsReturned;

		public int m_nTotalResultCount;
	}
}
