using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4526)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_HideToolTip_t
	{
		public const int k_iCallback = 4526;

		public HHTMLBrowser unBrowserHandle;
	}
}
