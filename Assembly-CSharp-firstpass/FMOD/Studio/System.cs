using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	public class System : HandleBase
	{
		public System(IntPtr raw) : base(raw)
		{
		}

		public static RESULT create(out System studiosystem)
		{
			studiosystem = null;
			IntPtr raw;
			RESULT rESULT = System.FMOD_Studio_System_Create(out raw, 67335u);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			studiosystem = new System(raw);
			return rESULT;
		}

		public RESULT setAdvancedSettings(ADVANCEDSETTINGS settings)
		{
			settings.cbSize = Marshal.SizeOf(typeof(ADVANCEDSETTINGS));
			return System.FMOD_Studio_System_SetAdvancedSettings(this.rawPtr, ref settings);
		}

		public RESULT getAdvancedSettings(out ADVANCEDSETTINGS settings)
		{
			settings.cbSize = Marshal.SizeOf(typeof(ADVANCEDSETTINGS));
			return System.FMOD_Studio_System_GetAdvancedSettings(this.rawPtr, out settings);
		}

		public RESULT initialize(int maxchannels, INITFLAGS studioFlags, FMOD.INITFLAGS flags, IntPtr extradriverdata)
		{
			return System.FMOD_Studio_System_Initialize(this.rawPtr, maxchannels, studioFlags, flags, extradriverdata);
		}

		public RESULT release()
		{
			return System.FMOD_Studio_System_Release(this.rawPtr);
		}

		public RESULT update()
		{
			return System.FMOD_Studio_System_Update(this.rawPtr);
		}

		public RESULT getLowLevelSystem(out FMOD.System system)
		{
			system = null;
			IntPtr raw = 0;
			RESULT rESULT = System.FMOD_Studio_System_GetLowLevelSystem(this.rawPtr, out raw);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			system = new FMOD.System(raw);
			return rESULT;
		}

		public RESULT getEvent(string path, out EventDescription _event)
		{
			_event = null;
			IntPtr raw = 0;
			RESULT rESULT = System.FMOD_Studio_System_GetEvent(this.rawPtr, Encoding.UTF8.GetBytes(path + '\0'), out raw);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			_event = new EventDescription(raw);
			return rESULT;
		}

		public RESULT getBus(string path, out Bus bus)
		{
			bus = null;
			IntPtr raw = 0;
			RESULT rESULT = System.FMOD_Studio_System_GetBus(this.rawPtr, Encoding.UTF8.GetBytes(path + '\0'), out raw);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			bus = new Bus(raw);
			return rESULT;
		}

		public RESULT getVCA(string path, out VCA vca)
		{
			vca = null;
			IntPtr raw = 0;
			RESULT rESULT = System.FMOD_Studio_System_GetVCA(this.rawPtr, Encoding.UTF8.GetBytes(path + '\0'), out raw);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			vca = new VCA(raw);
			return rESULT;
		}

		public RESULT getBank(string path, out Bank bank)
		{
			bank = null;
			IntPtr raw = 0;
			RESULT rESULT = System.FMOD_Studio_System_GetBank(this.rawPtr, Encoding.UTF8.GetBytes(path + '\0'), out raw);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			bank = new Bank(raw);
			return rESULT;
		}

		public RESULT getEventByID(Guid guid, out EventDescription _event)
		{
			_event = null;
			IntPtr raw = 0;
			RESULT rESULT = System.FMOD_Studio_System_GetEventByID(this.rawPtr, ref guid, out raw);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			_event = new EventDescription(raw);
			return rESULT;
		}

		public RESULT getBusByID(Guid guid, out Bus bus)
		{
			bus = null;
			IntPtr raw = 0;
			RESULT rESULT = System.FMOD_Studio_System_GetBusByID(this.rawPtr, ref guid, out raw);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			bus = new Bus(raw);
			return rESULT;
		}

		public RESULT getVCAByID(Guid guid, out VCA vca)
		{
			vca = null;
			IntPtr raw = 0;
			RESULT rESULT = System.FMOD_Studio_System_GetVCAByID(this.rawPtr, ref guid, out raw);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			vca = new VCA(raw);
			return rESULT;
		}

		public RESULT getBankByID(Guid guid, out Bank bank)
		{
			bank = null;
			IntPtr raw = 0;
			RESULT rESULT = System.FMOD_Studio_System_GetBankByID(this.rawPtr, ref guid, out raw);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			bank = new Bank(raw);
			return rESULT;
		}

		public RESULT getSoundInfo(string key, out SOUND_INFO info)
		{
			SOUND_INFO_INTERNAL sOUND_INFO_INTERNAL;
			RESULT rESULT = System.FMOD_Studio_System_GetSoundInfo(this.rawPtr, Encoding.UTF8.GetBytes(key + '\0'), out sOUND_INFO_INTERNAL);
			if (rESULT != RESULT.OK)
			{
				info = new SOUND_INFO();
				return rESULT;
			}
			sOUND_INFO_INTERNAL.assign(out info);
			return rESULT;
		}

		public RESULT lookupID(string path, out Guid guid)
		{
			return System.FMOD_Studio_System_LookupID(this.rawPtr, Encoding.UTF8.GetBytes(path + '\0'), out guid);
		}

		public RESULT lookupPath(Guid guid, out string path)
		{
			path = null;
			byte[] array = new byte[256];
			int num = 0;
			RESULT rESULT = System.FMOD_Studio_System_LookupPath(this.rawPtr, ref guid, array, array.Length, out num);
			if (rESULT == RESULT.ERR_TRUNCATED)
			{
				array = new byte[num];
				rESULT = System.FMOD_Studio_System_LookupPath(this.rawPtr, ref guid, array, array.Length, out num);
			}
			if (rESULT == RESULT.OK)
			{
				path = Encoding.UTF8.GetString(array, 0, num - 1);
			}
			return rESULT;
		}

		public RESULT getNumListeners(out int numlisteners)
		{
			return System.FMOD_Studio_System_GetNumListeners(this.rawPtr, out numlisteners);
		}

		public RESULT setNumListeners(int numlisteners)
		{
			return System.FMOD_Studio_System_SetNumListeners(this.rawPtr, numlisteners);
		}

		public RESULT getListenerAttributes(int listener, out ATTRIBUTES_3D attributes)
		{
			return System.FMOD_Studio_System_GetListenerAttributes(this.rawPtr, listener, out attributes);
		}

		public RESULT setListenerAttributes(int listener, ATTRIBUTES_3D attributes)
		{
			return System.FMOD_Studio_System_SetListenerAttributes(this.rawPtr, listener, ref attributes);
		}

		public RESULT loadBankFile(string name, LOAD_BANK_FLAGS flags, out Bank bank)
		{
			bank = null;
			IntPtr raw = 0;
			RESULT rESULT = System.FMOD_Studio_System_LoadBankFile(this.rawPtr, Encoding.UTF8.GetBytes(name + '\0'), flags, out raw);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			bank = new Bank(raw);
			return rESULT;
		}

		public RESULT loadBankMemory(byte[] buffer, LOAD_BANK_FLAGS flags, out Bank bank)
		{
			bank = null;
			IntPtr raw = 0;
			RESULT rESULT = System.FMOD_Studio_System_LoadBankMemory(this.rawPtr, buffer, buffer.Length, LOAD_MEMORY_MODE.LOAD_MEMORY, flags, out raw);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			bank = new Bank(raw);
			return rESULT;
		}

		public RESULT loadBankCustom(BANK_INFO info, LOAD_BANK_FLAGS flags, out Bank bank)
		{
			bank = null;
			info.size = Marshal.SizeOf(info);
			IntPtr raw = 0;
			RESULT rESULT = System.FMOD_Studio_System_LoadBankCustom(this.rawPtr, ref info, flags, out raw);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			bank = new Bank(raw);
			return rESULT;
		}

		public RESULT unloadAll()
		{
			return System.FMOD_Studio_System_UnloadAll(this.rawPtr);
		}

		public RESULT flushCommands()
		{
			return System.FMOD_Studio_System_FlushCommands(this.rawPtr);
		}

		public RESULT startCommandCapture(string path, COMMANDCAPTURE_FLAGS flags)
		{
			return System.FMOD_Studio_System_StartCommandCapture(this.rawPtr, Encoding.UTF8.GetBytes(path + '\0'), flags);
		}

		public RESULT stopCommandCapture()
		{
			return System.FMOD_Studio_System_StopCommandCapture(this.rawPtr);
		}

		public RESULT loadCommandReplay(string path, COMMANDREPLAY_FLAGS flags, out CommandReplay replay)
		{
			replay = null;
			IntPtr raw = 0;
			RESULT rESULT = System.FMOD_Studio_System_LoadCommandReplay(this.rawPtr, Encoding.UTF8.GetBytes(path + '\0'), flags, out raw);
			if (rESULT == RESULT.OK)
			{
				replay = new CommandReplay(raw);
			}
			return rESULT;
		}

		public RESULT getBankCount(out int count)
		{
			return System.FMOD_Studio_System_GetBankCount(this.rawPtr, out count);
		}

		public RESULT getBankList(out Bank[] array)
		{
			array = null;
			int num;
			RESULT rESULT = System.FMOD_Studio_System_GetBankCount(this.rawPtr, out num);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			if (num == 0)
			{
				array = new Bank[0];
				return rESULT;
			}
			IntPtr[] array2 = new IntPtr[num];
			int num2;
			rESULT = System.FMOD_Studio_System_GetBankList(this.rawPtr, array2, num, out num2);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			if (num2 > num)
			{
				num2 = num;
			}
			array = new Bank[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = new Bank(array2[i]);
			}
			return RESULT.OK;
		}

		public RESULT getCPUUsage(out CPU_USAGE usage)
		{
			return System.FMOD_Studio_System_GetCPUUsage(this.rawPtr, out usage);
		}

		public RESULT getBufferUsage(out BUFFER_USAGE usage)
		{
			return System.FMOD_Studio_System_GetBufferUsage(this.rawPtr, out usage);
		}

		public RESULT resetBufferUsage()
		{
			return System.FMOD_Studio_System_ResetBufferUsage(this.rawPtr);
		}

		public RESULT setCallback(SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask = SYSTEM_CALLBACK_TYPE.ALL)
		{
			return System.FMOD_Studio_System_SetCallback(this.rawPtr, callback, callbackmask);
		}

		public RESULT getUserData(out IntPtr userData)
		{
			return System.FMOD_Studio_System_GetUserData(this.rawPtr, out userData);
		}

		public RESULT setUserData(IntPtr userData)
		{
			return System.FMOD_Studio_System_SetUserData(this.rawPtr, userData);
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_Create(out IntPtr studiosystem, uint headerversion);

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_System_IsValid(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetAdvancedSettings(IntPtr studiosystem, ref ADVANCEDSETTINGS settings);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetAdvancedSettings(IntPtr studiosystem, out ADVANCEDSETTINGS settings);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_Initialize(IntPtr studiosystem, int maxchannels, INITFLAGS studioFlags, FMOD.INITFLAGS flags, IntPtr extradriverdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_Release(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_Update(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetLowLevelSystem(IntPtr studiosystem, out IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetEvent(IntPtr studiosystem, byte[] path, out IntPtr description);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBus(IntPtr studiosystem, byte[] path, out IntPtr bus);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetVCA(IntPtr studiosystem, byte[] path, out IntPtr vca);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBank(IntPtr studiosystem, byte[] path, out IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetEventByID(IntPtr studiosystem, ref Guid guid, out IntPtr description);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBusByID(IntPtr studiosystem, ref Guid guid, out IntPtr bus);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetVCAByID(IntPtr studiosystem, ref Guid guid, out IntPtr vca);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBankByID(IntPtr studiosystem, ref Guid guid, out IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetSoundInfo(IntPtr studiosystem, byte[] key, out SOUND_INFO_INTERNAL info);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LookupID(IntPtr studiosystem, byte[] path, out Guid guid);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LookupPath(IntPtr studiosystem, ref Guid guid, [Out] byte[] path, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetNumListeners(IntPtr studiosystem, out int numlisteners);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetNumListeners(IntPtr studiosystem, int numlisteners);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetListenerAttributes(IntPtr studiosystem, int listener, out ATTRIBUTES_3D attributes);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetListenerAttributes(IntPtr studiosystem, int listener, ref ATTRIBUTES_3D attributes);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LoadBankFile(IntPtr studiosystem, byte[] filename, LOAD_BANK_FLAGS flags, out IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LoadBankMemory(IntPtr studiosystem, byte[] buffer, int length, LOAD_MEMORY_MODE mode, LOAD_BANK_FLAGS flags, out IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LoadBankCustom(IntPtr studiosystem, ref BANK_INFO info, LOAD_BANK_FLAGS flags, out IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_UnloadAll(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_FlushCommands(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_StartCommandCapture(IntPtr studiosystem, byte[] path, COMMANDCAPTURE_FLAGS flags);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_StopCommandCapture(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LoadCommandReplay(IntPtr studiosystem, byte[] path, COMMANDREPLAY_FLAGS flags, out IntPtr commandReplay);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBankCount(IntPtr studiosystem, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBankList(IntPtr studiosystem, IntPtr[] array, int capacity, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetCPUUsage(IntPtr studiosystem, out CPU_USAGE usage);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBufferUsage(IntPtr studiosystem, out BUFFER_USAGE usage);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_ResetBufferUsage(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetCallback(IntPtr studiosystem, SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetUserData(IntPtr studiosystem, out IntPtr userData);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetUserData(IntPtr studiosystem, IntPtr userData);

		protected override bool isValidInternal()
		{
			return System.FMOD_Studio_System_IsValid(this.rawPtr);
		}
	}
}
