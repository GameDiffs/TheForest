using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4522)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_SetCursor_t
	{
		public const int k_iCallback = 4522;

		public HHTMLBrowser unBrowserHandle;

		public uint eMouseCursor;
	}
}
