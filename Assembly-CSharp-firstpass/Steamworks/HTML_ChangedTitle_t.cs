using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4508)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_ChangedTitle_t
	{
		public const int k_iCallback = 4508;

		public HHTMLBrowser unBrowserHandle;

		public string pchTitle;
	}
}
