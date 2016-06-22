using System;

namespace FMOD
{
	public delegate RESULT DSP_PAN_SUM_MONO_TO_SURROUND_MATRIX(ref DSP_STATE dsp_state, int targetSpeakerMode, float direction, float extent, float lowFrequencyGain, float overallGain, int matrixHop, IntPtr matrix);
}
