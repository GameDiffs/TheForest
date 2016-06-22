using System;

namespace FMOD
{
	public struct DSP_STATE_PAN_CALLBACKS
	{
		public DSP_PAN_SUM_MONO_MATRIX summonomatrix;

		public DSP_PAN_SUM_STEREO_MATRIX sumstereomatrix;

		public DSP_PAN_SUM_SURROUND_MATRIX sumsurroundmatrix;

		public DSP_PAN_SUM_MONO_TO_SURROUND_MATRIX summonotosurroundmatrix;

		public DSP_PAN_SUM_STEREO_TO_SURROUND_MATRIX sumstereotosurroundmatrix;

		public DSP_PAN_3D_GET_ROLLOFF_GAIN getrolloffgain;
	}
}
