using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4511)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_HorizontalScroll_t
	{
		public const int k_iCallback = 4511;

		public HHTMLBrowser unBrowserHandle;

		public uint unScrollMax;

		public uint unScrollCurrent;

		public float flPageScale;

		[MarshalAs(UnmanagedType.I1)]
		public bool bVisible;

		public uint unPageSize;
	}
}
