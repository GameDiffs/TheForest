using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4524)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_ShowToolTip_t
	{
		public const int k_iCallback = 4524;

		public HHTMLBrowser unBrowserHandle;

		public string pchMsg;
	}
}
