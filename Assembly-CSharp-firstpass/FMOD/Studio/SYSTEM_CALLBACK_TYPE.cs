using System;

namespace FMOD.Studio
{
	[Flags]
	public enum SYSTEM_CALLBACK_TYPE : uint
	{
		PREUPDATE = 1u,
		POSTUPDATE = 2u,
		BANK_UNLOAD = 4u,
		ALL = 4294967295u
	}
}
