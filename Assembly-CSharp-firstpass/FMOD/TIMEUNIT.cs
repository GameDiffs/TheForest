using System;

namespace FMOD
{
	[Flags]
	public enum TIMEUNIT : uint
	{
		MS = 1u,
		PCM = 2u,
		PCMBYTES = 4u,
		RAWBYTES = 8u,
		PCMFRACTION = 16u,
		MODORDER = 256u,
		MODROW = 512u,
		MODPATTERN = 1024u,
		BUFFERED = 268435456u
	}
}
