using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1306)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct RemoteStorageConflictResolution_t
	{
		public const int k_iCallback = 1306;

		public AppId_t m_nAppID;

		public EResult m_eResult;
	}
}
