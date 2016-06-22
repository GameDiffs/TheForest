using System;

namespace FMOD.Studio
{
	[Flags]
	public enum COMMANDCAPTURE_FLAGS : uint
	{
		NORMAL = 0u,
		FILEFLUSH = 1u,
		SKIP_INITIAL_STATE = 2u
	}
}
