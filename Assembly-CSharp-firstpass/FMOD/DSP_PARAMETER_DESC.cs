using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct DSP_PARAMETER_DESC
	{
		public DSP_PARAMETER_TYPE type;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public char[] name;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		public char[] label;

		public string description;

		public DSP_PARAMETER_DESC_UNION desc;
	}
}
