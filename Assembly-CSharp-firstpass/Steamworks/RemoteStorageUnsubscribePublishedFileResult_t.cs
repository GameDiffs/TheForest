using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1315)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct RemoteStorageUnsubscribePublishedFileResult_t
	{
		public const int k_iCallback = 1315;

		public EResult m_eResult;

		public PublishedFileId_t m_nPublishedFileId;
	}
}
