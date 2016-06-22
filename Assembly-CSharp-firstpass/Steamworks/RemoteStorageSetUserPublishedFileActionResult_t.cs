using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1327)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct RemoteStorageSetUserPublishedFileActionResult_t
	{
		public const int k_iCallback = 1327;

		public EResult m_eResult;

		public PublishedFileId_t m_nPublishedFileId;

		public EWorkshopFileAction m_eAction;
	}
}
