using System;

namespace FMOD.Studio
{
	[Flags]
	public enum INITFLAGS : uint
	{
		NORMAL = 0u,
		LIVEUPDATE = 1u,
		ALLOW_MISSING_PLUGINS = 2u,
		SYNCHRONOUS_UPDATE = 4u,
		DEFERRED_CALLBACKS = 8u
	}
}
