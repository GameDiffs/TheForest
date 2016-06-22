using System;

namespace FMOD
{
	public delegate RESULT CHANNEL_CALLBACK(IntPtr channelraw, CHANNELCONTROL_TYPE controltype, CHANNELCONTROL_CALLBACK_TYPE type, IntPtr commanddata1, IntPtr commanddata2);
}
