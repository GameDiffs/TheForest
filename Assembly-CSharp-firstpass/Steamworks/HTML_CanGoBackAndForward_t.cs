using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4510)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_CanGoBackAndForward_t
	{
		public const int k_iCallback = 4510;

		public HHTMLBrowser unBrowserHandle;

		[MarshalAs(UnmanagedType.I1)]
		public bool bCanGoBack;

		[MarshalAs(UnmanagedType.I1)]
		public bool bCanGoForward;
	}
}
