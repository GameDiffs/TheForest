using System;

namespace FMOD
{
	public delegate RESULT DSP_GETPARAM_BOOL_CALLBACK(ref DSP_STATE dsp_state, int index, ref bool value, IntPtr valuestr);
}
