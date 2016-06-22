using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	public class CommandReplay : HandleBase
	{
		public CommandReplay(IntPtr raw) : base(raw)
		{
		}

		public RESULT getSystem(out System system)
		{
			system = null;
			IntPtr raw = 0;
			RESULT rESULT = CommandReplay.FMOD_Studio_CommandReplay_GetSystem(this.rawPtr, out raw);
			if (rESULT == RESULT.OK)
			{
				system = new System(raw);
			}
			return rESULT;
		}

		public RESULT getLength(out float totalTime)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_GetLength(this.rawPtr, out totalTime);
		}

		public RESULT getCommandCount(out int count)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_GetCommandCount(this.rawPtr, out count);
		}

		public RESULT getCommandInfo(int commandIndex, out COMMAND_INFO info)
		{
			COMMAND_INFO_INTERNAL cOMMAND_INFO_INTERNAL = default(COMMAND_INFO_INTERNAL);
			RESULT rESULT = CommandReplay.FMOD_Studio_CommandReplay_GetCommandInfo(this.rawPtr, commandIndex, out cOMMAND_INFO_INTERNAL);
			if (rESULT != RESULT.OK)
			{
				info = default(COMMAND_INFO);
				return rESULT;
			}
			info = cOMMAND_INFO_INTERNAL.createPublic();
			return rESULT;
		}

		public RESULT getCommandString(int commandIndex, out string description)
		{
			description = null;
			byte[] array = new byte[8];
			RESULT rESULT;
			while (true)
			{
				rESULT = CommandReplay.FMOD_Studio_CommandReplay_GetCommandString(this.rawPtr, commandIndex, array, array.Length);
				if (rESULT != RESULT.ERR_TRUNCATED)
				{
					break;
				}
				array = new byte[2 * array.Length];
			}
			if (rESULT == RESULT.OK)
			{
				int num = 0;
				while (array[num] != 0)
				{
					num++;
				}
				description = Encoding.UTF8.GetString(array, 0, num);
			}
			return rESULT;
		}

		public RESULT getCommandAtTime(float time, out int commandIndex)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_GetCommandAtTime(this.rawPtr, time, out commandIndex);
		}

		public RESULT setBankPath(string bankPath)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_SetBankPath(this.rawPtr, Encoding.UTF8.GetBytes(bankPath + '\0'));
		}

		public RESULT start()
		{
			return CommandReplay.FMOD_Studio_CommandReplay_Start(this.rawPtr);
		}

		public RESULT stop()
		{
			return CommandReplay.FMOD_Studio_CommandReplay_Stop(this.rawPtr);
		}

		public RESULT seekToTime(float time)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_SeekToTime(this.rawPtr, time);
		}

		public RESULT seekToCommand(int commandIndex)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_SeekToCommand(this.rawPtr, commandIndex);
		}

		public RESULT getPaused(out bool paused)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_GetPaused(this.rawPtr, out paused);
		}

		public RESULT setPaused(bool paused)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_SetPaused(this.rawPtr, paused);
		}

		public RESULT getPlaybackState(out PLAYBACK_STATE state)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_GetPlaybackState(this.rawPtr, out state);
		}

		public RESULT getCurrentCommand(out int commandIndex, out float currentTime)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_GetCurrentCommand(this.rawPtr, out commandIndex, out currentTime);
		}

		public RESULT release()
		{
			return CommandReplay.FMOD_Studio_CommandReplay_Release(this.rawPtr);
		}

		public RESULT setFrameCallback(COMMANDREPLAY_FRAME_CALLBACK callback)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_SetFrameCallback(this.rawPtr, callback);
		}

		public RESULT setLoadBankCallback(COMMANDREPLAY_LOAD_BANK_CALLBACK callback)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_SetLoadBankCallback(this.rawPtr, callback);
		}

		public RESULT setCreateInstanceCallback(COMMANDREPLAY_CREATE_INSTANCE_CALLBACK callback)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_SetCreateInstanceCallback(this.rawPtr, callback);
		}

		public RESULT getUserData(out IntPtr userData)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_GetUserData(this.rawPtr, out userData);
		}

		public RESULT setUserData(IntPtr userData)
		{
			return CommandReplay.FMOD_Studio_CommandReplay_SetUserData(this.rawPtr, userData);
		}

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_CommandReplay_IsValid(IntPtr replay);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetSystem(IntPtr replay, out IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetLength(IntPtr replay, out float totalTime);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetCommandCount(IntPtr replay, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetCommandInfo(IntPtr replay, int commandIndex, out COMMAND_INFO_INTERNAL info);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetCommandString(IntPtr replay, int commandIndex, [Out] byte[] description, int capacity);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetCommandAtTime(IntPtr replay, float time, out int commandIndex);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SetBankPath(IntPtr replay, byte[] bankPath);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_Start(IntPtr replay);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_Stop(IntPtr replay);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SeekToTime(IntPtr replay, float time);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SeekToCommand(IntPtr replay, int commandIndex);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetPaused(IntPtr replay, out bool paused);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SetPaused(IntPtr replay, bool paused);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetPlaybackState(IntPtr replay, out PLAYBACK_STATE state);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetCurrentCommand(IntPtr replay, out int commandIndex, out float currentTime);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_Release(IntPtr replay);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SetFrameCallback(IntPtr replay, COMMANDREPLAY_FRAME_CALLBACK callback);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SetLoadBankCallback(IntPtr replay, COMMANDREPLAY_LOAD_BANK_CALLBACK callback);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SetCreateInstanceCallback(IntPtr replay, COMMANDREPLAY_CREATE_INSTANCE_CALLBACK callback);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetUserData(IntPtr replay, out IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SetUserData(IntPtr replay, IntPtr userdata);

		protected override bool isValidInternal()
		{
			return CommandReplay.FMOD_Studio_CommandReplay_IsValid(this.rawPtr);
		}
	}
}
