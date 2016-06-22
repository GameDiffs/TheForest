using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(4605)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct BroadcastUploadStop_t
	{
		public const int k_iCallback = 4605;

		public EBroadcastUploadResult m_eResult;
	}
}
