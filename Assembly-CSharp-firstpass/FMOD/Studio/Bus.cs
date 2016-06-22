using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	public class Bus : HandleBase
	{
		public Bus(IntPtr raw) : base(raw)
		{
		}

		public RESULT getID(out Guid id)
		{
			return Bus.FMOD_Studio_Bus_GetID(this.rawPtr, out id);
		}

		public RESULT getPath(out string path)
		{
			path = null;
			byte[] array = new byte[256];
			int num = 0;
			RESULT rESULT = Bus.FMOD_Studio_Bus_GetPath(this.rawPtr, array, array.Length, out num);
			if (rESULT == RESULT.ERR_TRUNCATED)
			{
				array = new byte[num];
				rESULT = Bus.FMOD_Studio_Bus_GetPath(this.rawPtr, array, array.Length, out num);
			}
			if (rESULT == RESULT.OK)
			{
				path = Encoding.UTF8.GetString(array, 0, num - 1);
			}
			return rESULT;
		}

		public RESULT getFaderLevel(out float volume)
		{
			return Bus.FMOD_Studio_Bus_GetFaderLevel(this.rawPtr, out volume);
		}

		public RESULT setFaderLevel(float volume)
		{
			return Bus.FMOD_Studio_Bus_SetFaderLevel(this.rawPtr, volume);
		}

		public RESULT getPaused(out bool paused)
		{
			return Bus.FMOD_Studio_Bus_GetPaused(this.rawPtr, out paused);
		}

		public RESULT setPaused(bool paused)
		{
			return Bus.FMOD_Studio_Bus_SetPaused(this.rawPtr, paused);
		}

		public RESULT getMute(out bool mute)
		{
			return Bus.FMOD_Studio_Bus_GetMute(this.rawPtr, out mute);
		}

		public RESULT setMute(bool mute)
		{
			return Bus.FMOD_Studio_Bus_SetMute(this.rawPtr, mute);
		}

		public RESULT stopAllEvents(STOP_MODE mode)
		{
			return Bus.FMOD_Studio_Bus_StopAllEvents(this.rawPtr, mode);
		}

		public RESULT lockChannelGroup()
		{
			return Bus.FMOD_Studio_Bus_LockChannelGroup(this.rawPtr);
		}

		public RESULT unlockChannelGroup()
		{
			return Bus.FMOD_Studio_Bus_UnlockChannelGroup(this.rawPtr);
		}

		public RESULT getChannelGroup(out ChannelGroup group)
		{
			group = null;
			IntPtr raw = 0;
			RESULT rESULT = Bus.FMOD_Studio_Bus_GetChannelGroup(this.rawPtr, out raw);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			group = new ChannelGroup(raw);
			return rESULT;
		}

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_Bus_IsValid(IntPtr bus);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_GetID(IntPtr bus, out Guid id);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_GetPath(IntPtr bus, [Out] byte[] path, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_GetFaderLevel(IntPtr bus, out float value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_SetFaderLevel(IntPtr bus, float value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_GetPaused(IntPtr bus, out bool paused);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_SetPaused(IntPtr bus, bool paused);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_GetMute(IntPtr bus, out bool mute);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_SetMute(IntPtr bus, bool mute);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_StopAllEvents(IntPtr bus, STOP_MODE mode);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_LockChannelGroup(IntPtr bus);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_UnlockChannelGroup(IntPtr bus);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_GetChannelGroup(IntPtr bus, out IntPtr group);

		protected override bool isValidInternal()
		{
			return Bus.FMOD_Studio_Bus_IsValid(this.rawPtr);
		}
	}
}
