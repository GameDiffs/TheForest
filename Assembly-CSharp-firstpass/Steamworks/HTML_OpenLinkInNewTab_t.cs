using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4507)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_OpenLinkInNewTab_t
	{
		public const int k_iCallback = 4507;

		public HHTMLBrowser unBrowserHandle;

		public string pchURL;
	}
}
