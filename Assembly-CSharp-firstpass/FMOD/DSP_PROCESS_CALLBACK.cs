using System;

namespace FMOD
{
	public delegate RESULT DSP_PROCESS_CALLBACK(ref DSP_STATE dsp_state, uint length, ref DSP_BUFFER_ARRAY inbufferarray, ref DSP_BUFFER_ARRAY outbufferarray, bool inputsidle, DSP_PROCESS_OPERATION op);
}
