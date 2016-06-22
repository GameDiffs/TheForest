using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct DSP_PARAMETER_FFT
	{
		public int length;

		public int numchannels;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
		private IntPtr[] spectrum_internal;

		public float[][] spectrum
		{
			get
			{
				float[][] array = new float[this.numchannels][];
				for (int i = 0; i < this.numchannels; i++)
				{
					array[i] = new float[this.length];
					Marshal.Copy(this.spectrum_internal[i], array[i], 0, this.length);
				}
				return array;
			}
		}
	}
}
