using System;

namespace FMOD
{
	public delegate RESULT DSP_DFT_IFFTREAL(ref DSP_STATE dsp_state, int size, IntPtr dft, IntPtr signal, IntPtr window, int signalhop);
}
