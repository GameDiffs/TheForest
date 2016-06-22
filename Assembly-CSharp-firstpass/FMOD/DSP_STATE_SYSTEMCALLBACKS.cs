using System;

namespace FMOD
{
	public struct DSP_STATE_SYSTEMCALLBACKS
	{
		private MEMORY_ALLOC_CALLBACK alloc;

		private MEMORY_REALLOC_CALLBACK realloc;

		private MEMORY_FREE_CALLBACK free;

		private DSP_SYSTEM_GETSAMPLERATE getsamplerate;

		private DSP_SYSTEM_GETBLOCKSIZE getblocksize;

		private IntPtr dft;

		private IntPtr pancallbacks;

		private DSP_SYSTEM_GETSPEAKERMODE getspeakermode;
	}
}
