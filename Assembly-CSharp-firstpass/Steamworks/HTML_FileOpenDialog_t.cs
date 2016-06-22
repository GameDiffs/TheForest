using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4516)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_FileOpenDialog_t
	{
		public const int k_iCallback = 4516;

		public HHTMLBrowser unBrowserHandle;

		public string pchTitle;

		public string pchInitialFile;
	}
}
