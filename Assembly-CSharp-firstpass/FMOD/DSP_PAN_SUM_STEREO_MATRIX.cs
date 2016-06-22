using System;

namespace FMOD
{
	public delegate RESULT DSP_PAN_SUM_STEREO_MATRIX(ref DSP_STATE dsp_state, int sourceSpeakerMode, float pan, float lowFrequencyGain, float overallGain, int matrixHop, IntPtr matrix);
}
