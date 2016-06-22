using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	[StructLayout(LayoutKind.Sequential)]
	public class DSP_METERING_INFO
	{
		public int numsamples;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public float[] peaklevel;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		public float[] rmslevel;

		public short numchannels;
	}
}
