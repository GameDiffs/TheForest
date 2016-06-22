using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	public class EventDescription : HandleBase
	{
		public EventDescription(IntPtr raw) : base(raw)
		{
		}

		public RESULT getID(out Guid id)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetID(this.rawPtr, out id);
		}

		public RESULT getPath(out string path)
		{
			path = null;
			byte[] array = new byte[256];
			int num = 0;
			RESULT rESULT = EventDescription.FMOD_Studio_EventDescription_GetPath(this.rawPtr, array, array.Length, out num);
			if (rESULT == RESULT.ERR_TRUNCATED)
			{
				array = new byte[num];
				rESULT = EventDescription.FMOD_Studio_EventDescription_GetPath(this.rawPtr, array, array.Length, out num);
			}
			if (rESULT == RESULT.OK)
			{
				path = Encoding.UTF8.GetString(array, 0, num - 1);
			}
			return rESULT;
		}

		public RESULT getParameterCount(out int count)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetParameterCount(this.rawPtr, out count);
		}

		public RESULT getParameterByIndex(int index, out PARAMETER_DESCRIPTION parameter)
		{
			parameter = default(PARAMETER_DESCRIPTION);
			PARAMETER_DESCRIPTION_INTERNAL pARAMETER_DESCRIPTION_INTERNAL;
			RESULT rESULT = EventDescription.FMOD_Studio_EventDescription_GetParameterByIndex(this.rawPtr, index, out pARAMETER_DESCRIPTION_INTERNAL);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			pARAMETER_DESCRIPTION_INTERNAL.assign(out parameter);
			return rESULT;
		}

		public RESULT getParameter(string name, out PARAMETER_DESCRIPTION parameter)
		{
			parameter = default(PARAMETER_DESCRIPTION);
			PARAMETER_DESCRIPTION_INTERNAL pARAMETER_DESCRIPTION_INTERNAL;
			RESULT rESULT = EventDescription.FMOD_Studio_EventDescription_GetParameter(this.rawPtr, Encoding.UTF8.GetBytes(name + '\0'), out pARAMETER_DESCRIPTION_INTERNAL);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			pARAMETER_DESCRIPTION_INTERNAL.assign(out parameter);
			return rESULT;
		}

		public RESULT getUserPropertyCount(out int count)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetUserPropertyCount(this.rawPtr, out count);
		}

		public RESULT getUserPropertyByIndex(int index, out USER_PROPERTY property)
		{
			USER_PROPERTY_INTERNAL uSER_PROPERTY_INTERNAL;
			RESULT rESULT = EventDescription.FMOD_Studio_EventDescription_GetUserPropertyByIndex(this.rawPtr, index, out uSER_PROPERTY_INTERNAL);
			if (rESULT != RESULT.OK)
			{
				property = default(USER_PROPERTY);
				return rESULT;
			}
			property = uSER_PROPERTY_INTERNAL.createPublic();
			return RESULT.OK;
		}

		public RESULT getUserProperty(string name, out USER_PROPERTY property)
		{
			USER_PROPERTY_INTERNAL uSER_PROPERTY_INTERNAL;
			RESULT rESULT = EventDescription.FMOD_Studio_EventDescription_GetUserProperty(this.rawPtr, Encoding.UTF8.GetBytes(name + '\0'), out uSER_PROPERTY_INTERNAL);
			if (rESULT != RESULT.OK)
			{
				property = default(USER_PROPERTY);
				return rESULT;
			}
			property = uSER_PROPERTY_INTERNAL.createPublic();
			return RESULT.OK;
		}

		public RESULT getLength(out int length)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetLength(this.rawPtr, out length);
		}

		public RESULT getMinimumDistance(out float distance)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetMinimumDistance(this.rawPtr, out distance);
		}

		public RESULT getMaximumDistance(out float distance)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetMaximumDistance(this.rawPtr, out distance);
		}

		public RESULT getSoundSize(out float size)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetSoundSize(this.rawPtr, out size);
		}

		public RESULT isOneshot(out bool oneshot)
		{
			return EventDescription.FMOD_Studio_EventDescription_IsOneshot(this.rawPtr, out oneshot);
		}

		public RESULT isStream(out bool isStream)
		{
			return EventDescription.FMOD_Studio_EventDescription_IsStream(this.rawPtr, out isStream);
		}

		public RESULT is3D(out bool is3D)
		{
			return EventDescription.FMOD_Studio_EventDescription_Is3D(this.rawPtr, out is3D);
		}

		public RESULT createInstance(out EventInstance instance)
		{
			instance = null;
			IntPtr raw = 0;
			RESULT rESULT = EventDescription.FMOD_Studio_EventDescription_CreateInstance(this.rawPtr, out raw);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			instance = new EventInstance(raw);
			return rESULT;
		}

		public RESULT getInstanceCount(out int count)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetInstanceCount(this.rawPtr, out count);
		}

		public RESULT getInstanceList(out EventInstance[] array)
		{
			array = null;
			int num;
			RESULT rESULT = EventDescription.FMOD_Studio_EventDescription_GetInstanceCount(this.rawPtr, out num);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			if (num == 0)
			{
				array = new EventInstance[0];
				return rESULT;
			}
			IntPtr[] array2 = new IntPtr[num];
			int num2;
			rESULT = EventDescription.FMOD_Studio_EventDescription_GetInstanceList(this.rawPtr, array2, num, out num2);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			if (num2 > num)
			{
				num2 = num;
			}
			array = new EventInstance[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = new EventInstance(array2[i]);
			}
			return RESULT.OK;
		}

		public RESULT loadSampleData()
		{
			return EventDescription.FMOD_Studio_EventDescription_LoadSampleData(this.rawPtr);
		}

		public RESULT unloadSampleData()
		{
			return EventDescription.FMOD_Studio_EventDescription_UnloadSampleData(this.rawPtr);
		}

		public RESULT getSampleLoadingState(out LOADING_STATE state)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetSampleLoadingState(this.rawPtr, out state);
		}

		public RESULT releaseAllInstances()
		{
			return EventDescription.FMOD_Studio_EventDescription_ReleaseAllInstances(this.rawPtr);
		}

		public RESULT setCallback(EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask = EVENT_CALLBACK_TYPE.ALL)
		{
			return EventDescription.FMOD_Studio_EventDescription_SetCallback(this.rawPtr, callback, callbackmask);
		}

		public RESULT getUserData(out IntPtr userData)
		{
			return EventDescription.FMOD_Studio_EventDescription_GetUserData(this.rawPtr, out userData);
		}

		public RESULT setUserData(IntPtr userData)
		{
			return EventDescription.FMOD_Studio_EventDescription_SetUserData(this.rawPtr, userData);
		}

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_EventDescription_IsValid(IntPtr eventdescription);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetID(IntPtr eventdescription, out Guid id);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetPath(IntPtr eventdescription, [Out] byte[] path, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetParameterCount(IntPtr eventdescription, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetParameterByIndex(IntPtr eventdescription, int index, out PARAMETER_DESCRIPTION_INTERNAL parameter);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetParameter(IntPtr eventdescription, byte[] name, out PARAMETER_DESCRIPTION_INTERNAL parameter);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetUserPropertyCount(IntPtr eventdescription, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetUserPropertyByIndex(IntPtr eventdescription, int index, out USER_PROPERTY_INTERNAL property);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetUserProperty(IntPtr eventdescription, byte[] name, out USER_PROPERTY_INTERNAL property);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetLength(IntPtr eventdescription, out int length);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetMinimumDistance(IntPtr eventdescription, out float distance);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetMaximumDistance(IntPtr eventdescription, out float distance);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetSoundSize(IntPtr eventdescription, out float size);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_IsOneshot(IntPtr eventdescription, out bool oneshot);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_IsStream(IntPtr eventdescription, out bool isStream);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_Is3D(IntPtr eventdescription, out bool is3D);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_CreateInstance(IntPtr eventdescription, out IntPtr instance);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetInstanceCount(IntPtr eventdescription, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetInstanceList(IntPtr eventdescription, IntPtr[] array, int capacity, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_LoadSampleData(IntPtr eventdescription);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_UnloadSampleData(IntPtr eventdescription);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetSampleLoadingState(IntPtr eventdescription, out LOADING_STATE state);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_ReleaseAllInstances(IntPtr eventdescription);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_SetCallback(IntPtr eventdescription, EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetUserData(IntPtr eventdescription, out IntPtr userData);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_SetUserData(IntPtr eventdescription, IntPtr userData);

		protected override bool isValidInternal()
		{
			return EventDescription.FMOD_Studio_EventDescription_IsValid(this.rawPtr);
		}
	}
}
