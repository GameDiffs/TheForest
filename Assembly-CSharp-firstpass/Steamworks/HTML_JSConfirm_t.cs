using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4515)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_JSConfirm_t
	{
		public const int k_iCallback = 4515;

		public HHTMLBrowser unBrowserHandle;

		public string pchMessage;
	}
}
