using System;

namespace FMOD
{
	public delegate RESULT DSP_GETPARAM_FLOAT_CALLBACK(ref DSP_STATE dsp_state, int index, ref float value, IntPtr valuestr);
}
