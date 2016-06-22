using System;

namespace FMOD
{
	public delegate RESULT DSP_PAN_SUM_SURROUND_MATRIX(ref DSP_STATE dsp_state, int sourceSpeakerMode, int targetSpeakerMode, float direction, float extent, float rotation, float lowFrequencyGain, float overallGain, int matrixHop, IntPtr matrix, DSP_PAN_SURROUND_FLAGS flags);
}
