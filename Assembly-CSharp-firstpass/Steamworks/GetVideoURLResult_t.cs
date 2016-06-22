using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4611)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct GetVideoURLResult_t
	{
		public const int k_iCallback = 4611;

		public EResult m_eResult;

		public AppId_t m_unVideoAppID;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string m_rgchURL;
	}
}
