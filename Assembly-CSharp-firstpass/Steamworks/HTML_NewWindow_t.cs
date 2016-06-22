using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4521)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_NewWindow_t
	{
		public const int k_iCallback = 4521;

		public HHTMLBrowser unBrowserHandle;

		public string pchURL;

		public uint unX;

		public uint unY;

		public uint unWide;

		public uint unTall;

		public HHTMLBrowser unNewWindow_BrowserHandle;
	}
}
