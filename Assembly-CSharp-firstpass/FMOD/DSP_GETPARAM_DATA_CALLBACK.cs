using System;

namespace FMOD
{
	public delegate RESULT DSP_GETPARAM_DATA_CALLBACK(ref DSP_STATE dsp_state, int index, ref IntPtr data, ref uint length, IntPtr valuestr);
}
