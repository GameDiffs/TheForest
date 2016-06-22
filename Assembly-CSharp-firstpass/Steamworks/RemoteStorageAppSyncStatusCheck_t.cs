using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1305)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct RemoteStorageAppSyncStatusCheck_t
	{
		public const int k_iCallback = 1305;

		public AppId_t m_nAppID;

		public EResult m_eResult;
	}
}
