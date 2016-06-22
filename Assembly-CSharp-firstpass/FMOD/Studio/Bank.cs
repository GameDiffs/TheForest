using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	public class Bank : HandleBase
	{
		public Bank(IntPtr raw) : base(raw)
		{
		}

		public RESULT getID(out Guid id)
		{
			return Bank.FMOD_Studio_Bank_GetID(this.rawPtr, out id);
		}

		public RESULT getPath(out string path)
		{
			path = null;
			byte[] array = new byte[256];
			int num = 0;
			RESULT rESULT = Bank.FMOD_Studio_Bank_GetPath(this.rawPtr, array, array.Length, out num);
			if (rESULT == RESULT.ERR_TRUNCATED)
			{
				array = new byte[num];
				rESULT = Bank.FMOD_Studio_Bank_GetPath(this.rawPtr, array, array.Length, out num);
			}
			if (rESULT == RESULT.OK)
			{
				path = Encoding.UTF8.GetString(array, 0, num - 1);
			}
			return rESULT;
		}

		public RESULT unload()
		{
			RESULT rESULT = Bank.FMOD_Studio_Bank_Unload(this.rawPtr);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			this.rawPtr = IntPtr.Zero;
			return RESULT.OK;
		}

		public RESULT loadSampleData()
		{
			return Bank.FMOD_Studio_Bank_LoadSampleData(this.rawPtr);
		}

		public RESULT unloadSampleData()
		{
			return Bank.FMOD_Studio_Bank_UnloadSampleData(this.rawPtr);
		}

		public RESULT getLoadingState(out LOADING_STATE state)
		{
			return Bank.FMOD_Studio_Bank_GetLoadingState(this.rawPtr, out state);
		}

		public RESULT getSampleLoadingState(out LOADING_STATE state)
		{
			return Bank.FMOD_Studio_Bank_GetSampleLoadingState(this.rawPtr, out state);
		}

		public RESULT getStringCount(out int count)
		{
			return Bank.FMOD_Studio_Bank_GetStringCount(this.rawPtr, out count);
		}

		public RESULT getStringInfo(int index, out Guid id, out string path)
		{
			path = null;
			byte[] array = new byte[256];
			int num = 0;
			RESULT rESULT = Bank.FMOD_Studio_Bank_GetStringInfo(this.rawPtr, index, out id, array, array.Length, out num);
			if (rESULT == RESULT.ERR_TRUNCATED)
			{
				array = new byte[num];
				rESULT = Bank.FMOD_Studio_Bank_GetStringInfo(this.rawPtr, index, out id, array, array.Length, out num);
			}
			if (rESULT == RESULT.OK)
			{
				path = Encoding.UTF8.GetString(array, 0, num - 1);
			}
			return RESULT.OK;
		}

		public RESULT getEventCount(out int count)
		{
			return Bank.FMOD_Studio_Bank_GetEventCount(this.rawPtr, out count);
		}

		public RESULT getEventList(out EventDescription[] array)
		{
			array = null;
			int num;
			RESULT rESULT = Bank.FMOD_Studio_Bank_GetEventCount(this.rawPtr, out num);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			if (num == 0)
			{
				array = new EventDescription[0];
				return rESULT;
			}
			IntPtr[] array2 = new IntPtr[num];
			int num2;
			rESULT = Bank.FMOD_Studio_Bank_GetEventList(this.rawPtr, array2, num, out num2);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			if (num2 > num)
			{
				num2 = num;
			}
			array = new EventDescription[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = new EventDescription(array2[i]);
			}
			return RESULT.OK;
		}

		public RESULT getBusCount(out int count)
		{
			return Bank.FMOD_Studio_Bank_GetBusCount(this.rawPtr, out count);
		}

		public RESULT getBusList(out Bus[] array)
		{
			array = null;
			int num;
			RESULT rESULT = Bank.FMOD_Studio_Bank_GetBusCount(this.rawPtr, out num);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			if (num == 0)
			{
				array = new Bus[0];
				return rESULT;
			}
			IntPtr[] array2 = new IntPtr[num];
			int num2;
			rESULT = Bank.FMOD_Studio_Bank_GetBusList(this.rawPtr, array2, num, out num2);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			if (num2 > num)
			{
				num2 = num;
			}
			array = new Bus[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = new Bus(array2[i]);
			}
			return RESULT.OK;
		}

		public RESULT getVCACount(out int count)
		{
			return Bank.FMOD_Studio_Bank_GetVCACount(this.rawPtr, out count);
		}

		public RESULT getVCAList(out VCA[] array)
		{
			array = null;
			int num;
			RESULT rESULT = Bank.FMOD_Studio_Bank_GetVCACount(this.rawPtr, out num);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			if (num == 0)
			{
				array = new VCA[0];
				return rESULT;
			}
			IntPtr[] array2 = new IntPtr[num];
			int num2;
			rESULT = Bank.FMOD_Studio_Bank_GetVCAList(this.rawPtr, array2, num, out num2);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			if (num2 > num)
			{
				num2 = num;
			}
			array = new VCA[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = new VCA(array2[i]);
			}
			return RESULT.OK;
		}

		public RESULT getUserData(out IntPtr userData)
		{
			return Bank.FMOD_Studio_Bank_GetUserData(this.rawPtr, out userData);
		}

		public RESULT setUserData(IntPtr userData)
		{
			return Bank.FMOD_Studio_Bank_SetUserData(this.rawPtr, userData);
		}

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_Bank_IsValid(IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetID(IntPtr bank, out Guid id);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetPath(IntPtr bank, [Out] byte[] path, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_Unload(IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_LoadSampleData(IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_UnloadSampleData(IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetLoadingState(IntPtr bank, out LOADING_STATE state);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetSampleLoadingState(IntPtr bank, out LOADING_STATE state);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetStringCount(IntPtr bank, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetStringInfo(IntPtr bank, int index, out Guid id, [Out] byte[] path, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetEventCount(IntPtr bank, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetEventList(IntPtr bank, IntPtr[] array, int capacity, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetBusCount(IntPtr bank, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetBusList(IntPtr bank, IntPtr[] array, int capacity, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetVCACount(IntPtr bank, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetVCAList(IntPtr bank, IntPtr[] array, int capacity, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetUserData(IntPtr studiosystem, out IntPtr userData);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_SetUserData(IntPtr studiosystem, IntPtr userData);

		protected override bool isValidInternal()
		{
			return Bank.FMOD_Studio_Bank_IsValid(this.rawPtr);
		}
	}
}
