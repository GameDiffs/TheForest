using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1311)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct RemoteStorageDeletePublishedFileResult_t
	{
		public const int k_iCallback = 1311;

		public EResult m_eResult;

		public PublishedFileId_t m_nPublishedFileId;
	}
}
