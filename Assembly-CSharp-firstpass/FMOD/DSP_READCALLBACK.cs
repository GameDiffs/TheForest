using System;

namespace FMOD
{
	public delegate RESULT DSP_READCALLBACK(ref DSP_STATE dsp_state, IntPtr inbuffer, IntPtr outbuffer, uint length, int inchannels, ref int outchannels);
}
