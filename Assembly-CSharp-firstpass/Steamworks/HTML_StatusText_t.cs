using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4523)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_StatusText_t
	{
		public const int k_iCallback = 4523;

		public HHTMLBrowser unBrowserHandle;

		public string pchMsg;
	}
}
