using System;

namespace FMOD
{
	[Flags]
	public enum MEMORY_TYPE : uint
	{
		NORMAL = 0u,
		STREAM_FILE = 1u,
		STREAM_DECODE = 2u,
		SAMPLEDATA = 4u,
		DSP_BUFFER = 8u,
		PLUGIN = 16u,
		XBOX360_PHYSICAL = 1048576u,
		PERSISTENT = 2097152u,
		SECONDARY = 4194304u,
		ALL = 4294967295u
	}
}
