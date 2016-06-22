using System;

namespace FMOD
{
	public struct DSP_BUFFER_ARRAY
	{
		public int numbuffers;

		public int[] buffernumchannels;

		public CHANNELMASK[] bufferchannelmask;

		public IntPtr[] buffers;

		public SPEAKERMODE speakermode;
	}
}
