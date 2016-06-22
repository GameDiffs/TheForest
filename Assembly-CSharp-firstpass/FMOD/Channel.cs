using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public class Channel : ChannelControl
	{
		public Channel(IntPtr raw) : base(raw)
		{
		}

		public RESULT setFrequency(float frequency)
		{
			return Channel.FMOD5_Channel_SetFrequency(base.getRaw(), frequency);
		}

		public RESULT getFrequency(out float frequency)
		{
			return Channel.FMOD5_Channel_GetFrequency(base.getRaw(), out frequency);
		}

		public RESULT setPriority(int priority)
		{
			return Channel.FMOD5_Channel_SetPriority(base.getRaw(), priority);
		}

		public RESULT getPriority(out int priority)
		{
			return Channel.FMOD5_Channel_GetPriority(base.getRaw(), out priority);
		}

		public RESULT setPosition(uint position, TIMEUNIT postype)
		{
			return Channel.FMOD5_Channel_SetPosition(base.getRaw(), position, postype);
		}

		public RESULT getPosition(out uint position, TIMEUNIT postype)
		{
			return Channel.FMOD5_Channel_GetPosition(base.getRaw(), out position, postype);
		}

		public RESULT setChannelGroup(ChannelGroup channelgroup)
		{
			return Channel.FMOD5_Channel_SetChannelGroup(base.getRaw(), channelgroup.getRaw());
		}

		public RESULT getChannelGroup(out ChannelGroup channelgroup)
		{
			channelgroup = null;
			IntPtr raw;
			RESULT result = Channel.FMOD5_Channel_GetChannelGroup(base.getRaw(), out raw);
			channelgroup = new ChannelGroup(raw);
			return result;
		}

		public RESULT setLoopCount(int loopcount)
		{
			return Channel.FMOD5_Channel_SetLoopCount(base.getRaw(), loopcount);
		}

		public RESULT getLoopCount(out int loopcount)
		{
			return Channel.FMOD5_Channel_GetLoopCount(base.getRaw(), out loopcount);
		}

		public RESULT setLoopPoints(uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype)
		{
			return Channel.FMOD5_Channel_SetLoopPoints(base.getRaw(), loopstart, loopstarttype, loopend, loopendtype);
		}

		public RESULT getLoopPoints(out uint loopstart, TIMEUNIT loopstarttype, out uint loopend, TIMEUNIT loopendtype)
		{
			return Channel.FMOD5_Channel_GetLoopPoints(base.getRaw(), out loopstart, loopstarttype, out loopend, loopendtype);
		}

		public RESULT isVirtual(out bool isvirtual)
		{
			return Channel.FMOD5_Channel_IsVirtual(base.getRaw(), out isvirtual);
		}

		public RESULT getCurrentSound(out Sound sound)
		{
			sound = null;
			IntPtr raw;
			RESULT result = Channel.FMOD5_Channel_GetCurrentSound(base.getRaw(), out raw);
			sound = new Sound(raw);
			return result;
		}

		public RESULT getIndex(out int index)
		{
			return Channel.FMOD5_Channel_GetIndex(base.getRaw(), out index);
		}

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_SetFrequency(IntPtr channel, float frequency);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_GetFrequency(IntPtr channel, out float frequency);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_SetPriority(IntPtr channel, int priority);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_GetPriority(IntPtr channel, out int priority);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_SetChannelGroup(IntPtr channel, IntPtr channelgroup);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_GetChannelGroup(IntPtr channel, out IntPtr channelgroup);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_IsVirtual(IntPtr channel, out bool isvirtual);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_GetCurrentSound(IntPtr channel, out IntPtr sound);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_GetIndex(IntPtr channel, out int index);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_SetPosition(IntPtr channel, uint position, TIMEUNIT postype);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_GetPosition(IntPtr channel, out uint position, TIMEUNIT postype);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_SetMode(IntPtr channel, MODE mode);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_GetMode(IntPtr channel, out MODE mode);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_SetLoopCount(IntPtr channel, int loopcount);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_GetLoopCount(IntPtr channel, out int loopcount);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_SetLoopPoints(IntPtr channel, uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_GetLoopPoints(IntPtr channel, out uint loopstart, TIMEUNIT loopstarttype, out uint loopend, TIMEUNIT loopendtype);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_SetUserData(IntPtr channel, IntPtr userdata);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Channel_GetUserData(IntPtr channel, out IntPtr userdata);
	}
}
