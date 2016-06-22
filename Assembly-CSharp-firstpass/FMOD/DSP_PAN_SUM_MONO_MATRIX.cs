using System;

namespace FMOD
{
	public delegate RESULT DSP_PAN_SUM_MONO_MATRIX(ref DSP_STATE dsp_state, int sourceSpeakerMode, float lowFrequencyGain, float overallGain, IntPtr matrix);
}
