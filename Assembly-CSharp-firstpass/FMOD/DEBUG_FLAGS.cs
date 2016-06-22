using System;

namespace FMOD
{
	[Flags]
	public enum DEBUG_FLAGS : uint
	{
		NONE = 0u,
		ERROR = 1u,
		WARNING = 2u,
		LOG = 4u,
		TYPE_MEMORY = 256u,
		TYPE_FILE = 512u,
		TYPE_CODEC = 1024u,
		TYPE_TRACE = 2048u,
		DISPLAY_TIMESTAMPS = 65536u,
		DISPLAY_LINENUMBERS = 131072u,
		DISPLAY_THREAD = 262144u
	}
}
