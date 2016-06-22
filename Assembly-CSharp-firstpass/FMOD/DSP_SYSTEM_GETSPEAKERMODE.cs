using System;

namespace FMOD
{
	public delegate RESULT DSP_SYSTEM_GETSPEAKERMODE(ref DSP_STATE dsp_state, ref int speakermode_mixer, ref int speakermode_output);
}
