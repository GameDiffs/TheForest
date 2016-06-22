using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1013)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct AppProofOfPurchaseKeyResponse_t
	{
		public const int k_iCallback = 1013;

		public EResult m_eResult;

		public uint m_nAppID;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		public string m_rgchKey;
	}
}
