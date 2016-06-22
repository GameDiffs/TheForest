using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1309)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct RemoteStoragePublishFileResult_t
	{
		public const int k_iCallback = 1309;

		public EResult m_eResult;

		public PublishedFileId_t m_nPublishedFileId;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bUserNeedsToAcceptWorkshopLegalAgreement;
	}
}
