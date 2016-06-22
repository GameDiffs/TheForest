using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(1014)]
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 1)]
	public struct NewLaunchQueryParameters_t
	{
		public const int k_iCallback = 1014;
	}
}
