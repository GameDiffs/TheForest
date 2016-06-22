using System;

namespace FMOD.Studio
{
	public delegate RESULT COMMANDREPLAY_CREATE_INSTANCE_CALLBACK(IntPtr replay, IntPtr eventDescription, IntPtr originalHandle, out IntPtr instance, IntPtr userdata);
}
