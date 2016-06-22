using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(516)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct FavoritesListAccountsUpdated_t
	{
		public const int k_iCallback = 516;

		public EResult m_eResult;
	}
}
