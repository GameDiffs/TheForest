using System;

namespace FMOD
{
	public struct DSP_PARAMETER_DESC_INT
	{
		public int min;

		public int max;

		public int defaultval;

		public bool goestoinf;

		public IntPtr valuenames;
	}
}
