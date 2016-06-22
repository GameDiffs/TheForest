using System;

namespace FMOD
{
	public delegate RESULT DSP_SETPARAM_DATA_CALLBACK(ref DSP_STATE dsp_state, int index, IntPtr data, uint length);
}
