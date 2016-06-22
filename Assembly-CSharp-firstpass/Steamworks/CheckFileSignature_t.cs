using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	[CallbackIdentity(705)]
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct CheckFileSignature_t
	{
		public const int k_iCallback = 705;

		public ECheckFileSignature m_eCheckFileSignature;
	}
}
