using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4512)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_VerticalScroll_t
	{
		public const int k_iCallback = 4512;

		public HHTMLBrowser unBrowserHandle;

		public uint unScrollMax;

		public uint unScrollCurrent;

		public float flPageScale;

		[MarshalAs(UnmanagedType.I1)]
		public bool bVisible;

		public uint unPageSize;
	}
}
