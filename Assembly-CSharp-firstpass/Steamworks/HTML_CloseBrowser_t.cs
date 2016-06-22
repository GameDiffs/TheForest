using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4504)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_CloseBrowser_t
	{
		public const int k_iCallback = 4504;

		public HHTMLBrowser unBrowserHandle;
	}
}
