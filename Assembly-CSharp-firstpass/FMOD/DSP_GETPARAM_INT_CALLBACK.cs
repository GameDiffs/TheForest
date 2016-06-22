using System;

namespace FMOD
{
	public delegate RESULT DSP_GETPARAM_INT_CALLBACK(ref DSP_STATE dsp_state, int index, ref int value, IntPtr valuestr);
}
