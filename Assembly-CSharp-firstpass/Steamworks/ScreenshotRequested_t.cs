using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(2302)]
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 1)]
	public struct ScreenshotRequested_t
	{
		public const int k_iCallback = 2302;
	}
}
