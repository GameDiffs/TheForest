using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(3404)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct SubmitItemUpdateResult_t
	{
		public const int k_iCallback = 3404;

		public EResult m_eResult;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bUserNeedsToAcceptWorkshopLegalAgreement;
	}
}
