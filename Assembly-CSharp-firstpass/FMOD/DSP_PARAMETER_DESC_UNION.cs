using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	[StructLayout(LayoutKind.Explicit)]
	public struct DSP_PARAMETER_DESC_UNION
	{
		[FieldOffset(0)]
		public DSP_PARAMETER_DESC_FLOAT floatdesc;

		[FieldOffset(0)]
		public DSP_PARAMETER_DESC_INT intdesc;

		[FieldOffset(0)]
		public DSP_PARAMETER_DESC_BOOL booldesc;

		[FieldOffset(0)]
		public DSP_PARAMETER_DESC_DATA datadesc;
	}
}
