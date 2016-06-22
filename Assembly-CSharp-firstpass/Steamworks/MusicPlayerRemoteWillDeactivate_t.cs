using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4102)]
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 1)]
	public struct MusicPlayerRemoteWillDeactivate_t
	{
		public const int k_iCallback = 4102;
	}
}
