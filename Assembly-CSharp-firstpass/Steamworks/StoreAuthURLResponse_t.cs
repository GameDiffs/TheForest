using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(165)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct StoreAuthURLResponse_t
	{
		public const int k_iCallback = 165;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
		public string m_szURL;
	}
}
