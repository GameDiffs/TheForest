using System;

namespace FMOD.Studio
{
	public delegate RESULT COMMANDREPLAY_LOAD_BANK_CALLBACK(IntPtr replay, ref Guid guid, StringWrapper bankFilename, LOAD_BANK_FLAGS flags, out IntPtr bank, IntPtr userdata);
}
