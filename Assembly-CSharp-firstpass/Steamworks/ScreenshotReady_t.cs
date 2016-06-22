using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(2301)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct ScreenshotReady_t
	{
		public const int k_iCallback = 2301;

		public ScreenshotHandle m_hLocal;

		public EResult m_eResult;
	}
}
