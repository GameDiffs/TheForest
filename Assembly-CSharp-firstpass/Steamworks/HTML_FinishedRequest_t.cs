using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4506)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_FinishedRequest_t
	{
		public const int k_iCallback = 4506;

		public HHTMLBrowser unBrowserHandle;

		public string pchURL;

		public string pchPageTitle;
	}
}
