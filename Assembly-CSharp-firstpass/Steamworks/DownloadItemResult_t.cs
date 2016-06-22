using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(3406)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct DownloadItemResult_t
	{
		public const int k_iCallback = 3406;

		public AppId_t m_unAppID;

		public PublishedFileId_t m_nPublishedFileId;

		public EResult m_eResult;
	}
}
