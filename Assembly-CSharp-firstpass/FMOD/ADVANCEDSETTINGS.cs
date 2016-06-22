using System;

namespace FMOD
{
	public struct ADVANCEDSETTINGS
	{
		public int cbSize;

		public int maxMPEGCodecs;

		public int maxADPCMCodecs;

		public int maxXMACodecs;

		public int maxVorbisCodecs;

		public int maxAT9Codecs;

		public int maxFADPCMCodecs;

		public int maxPCMCodecs;

		public int ASIONumChannels;

		public IntPtr ASIOChannelList;

		public IntPtr ASIOSpeakerList;

		public float HRTFMinAngle;

		public float HRTFMaxAngle;

		public float HRTFFreq;

		public float vol0virtualvol;

		public uint defaultDecodeBufferSize;

		public ushort profilePort;

		public uint geometryMaxFadeTime;

		public float distanceFilterCenterFreq;

		public int reverb3Dinstance;

		public int DSPBufferPoolSize;

		public uint stackSizeStream;

		public uint stackSizeNonBlocking;

		public uint stackSizeMixer;

		public DSP_RESAMPLER resamplerMethod;

		public uint commandQueueSize;

		public uint randomSeed;
	}
}
