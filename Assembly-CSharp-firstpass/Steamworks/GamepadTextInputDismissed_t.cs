using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(714)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct GamepadTextInputDismissed_t
	{
		public const int k_iCallback = 714;

		[MarshalAs(UnmanagedType.I1)]
		public bool m_bSubmitted;

		public uint m_unSubmittedText;
	}
}
