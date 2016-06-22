using System;

namespace FMOD
{
	public delegate RESULT SOUND_PCMSETPOSCALLBACK(IntPtr soundraw, int subsound, uint position, TIMEUNIT postype);
}
