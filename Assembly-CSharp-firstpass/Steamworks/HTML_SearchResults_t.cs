using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4509)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_SearchResults_t
	{
		public const int k_iCallback = 4509;

		public HHTMLBrowser unBrowserHandle;

		public uint unResults;

		public uint unCurrentMatch;
	}
}
