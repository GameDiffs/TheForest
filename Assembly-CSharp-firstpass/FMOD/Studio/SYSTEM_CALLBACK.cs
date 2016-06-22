using System;

namespace FMOD.Studio
{
	public delegate RESULT SYSTEM_CALLBACK(IntPtr systemraw, SYSTEM_CALLBACK_TYPE type, IntPtr parameters, IntPtr userdata);
}
