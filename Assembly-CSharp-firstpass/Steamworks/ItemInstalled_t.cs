using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(3405)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct ItemInstalled_t
	{
		public const int k_iCallback = 3405;

		public AppId_t m_unAppID;

		public PublishedFileId_t m_nPublishedFileId;
	}
}
