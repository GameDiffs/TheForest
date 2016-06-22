using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4502)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct HTML_NeedsPaint_t
	{
		public const int k_iCallback = 4502;

		public HHTMLBrowser unBrowserHandle;

		public IntPtr pBGRA;

		public uint unWide;

		public uint unTall;

		public uint unUpdateX;

		public uint unUpdateY;

		public uint unUpdateWide;

		public uint unUpdateTall;

		public uint unScrollX;

		public uint unScrollY;

		public float flPageScale;

		public uint unPageSerial;
	}
}
