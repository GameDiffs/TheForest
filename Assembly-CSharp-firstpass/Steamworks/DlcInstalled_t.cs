using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1005)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct DlcInstalled_t
	{
		public const int k_iCallback = 1005;

		public AppId_t m_nAppID;
	}
}
