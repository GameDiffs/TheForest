using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(701)]
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 1)]
	public struct IPCountry_t
	{
		public const int k_iCallback = 701;
	}
}
