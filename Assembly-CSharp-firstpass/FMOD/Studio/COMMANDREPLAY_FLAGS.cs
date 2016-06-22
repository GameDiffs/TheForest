using System;

namespace FMOD.Studio
{
	[Flags]
	public enum COMMANDREPLAY_FLAGS : uint
	{
		NORMAL = 0u,
		SKIP_CLEANUP = 1u
	}
}
