using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct DSP_PARAMETER_3DATTRIBUTES_MULTI
	{
		public int numlisteners;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
		public ATTRIBUTES_3D[] relative;

		public ATTRIBUTES_3D absolute;
	}
}
