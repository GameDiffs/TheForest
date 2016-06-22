using System;

namespace FMOD.Studio
{
	[Flags]
	public enum LOAD_BANK_FLAGS : uint
	{
		NORMAL = 0u,
		NONBLOCKING = 1u,
		DECOMPRESS_SAMPLES = 2u
	}
}
