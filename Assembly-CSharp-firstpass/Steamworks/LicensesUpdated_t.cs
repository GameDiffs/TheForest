using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(125)]
	[StructLayout(LayoutKind.Sequential, Pack = 8, Size = 1)]
	public struct LicensesUpdated_t
	{
		public const int k_iCallback = 125;
	}
}
