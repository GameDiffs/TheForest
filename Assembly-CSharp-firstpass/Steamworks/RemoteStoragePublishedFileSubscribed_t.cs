using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1321)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct RemoteStoragePublishedFileSubscribed_t
	{
		public const int k_iCallback = 1321;

		public PublishedFileId_t m_nPublishedFileId;

		public AppId_t m_nAppID;
	}
}
