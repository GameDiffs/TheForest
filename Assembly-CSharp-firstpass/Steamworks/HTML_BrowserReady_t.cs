using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4501)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_BrowserReady_t
	{
		public const int k_iCallback = 4501;

		public HHTMLBrowser unBrowserHandle;
	}
}
