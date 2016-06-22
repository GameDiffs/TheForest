using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1330)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct RemoteStoragePublishedFileUpdated_t
	{
		public const int k_iCallback = 1330;

		public PublishedFileId_t m_nPublishedFileId;

		public AppId_t m_nAppID;

		public UGCHandle_t m_hFile;
	}
}
