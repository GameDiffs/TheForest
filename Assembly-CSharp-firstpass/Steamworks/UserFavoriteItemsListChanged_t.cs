using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(3407)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct UserFavoriteItemsListChanged_t
	{
		public const int k_iCallback = 3407;

		public PublishedFileId_t m_nPublishedFileId;

		public EResult m_eResult;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bWasAddRequest;
	}
}
