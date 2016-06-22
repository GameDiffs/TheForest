using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(210)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct AssociateWithClanResult_t
	{
		public const int k_iCallback = 210;

		public EResult m_eResult;
	}
}
