using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4525)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_UpdateToolTip_t
	{
		public const int k_iCallback = 4525;

		public HHTMLBrowser unBrowserHandle;

		public string pchMsg;
	}
}
