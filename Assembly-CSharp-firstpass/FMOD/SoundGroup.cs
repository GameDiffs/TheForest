using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
	public class SoundGroup : HandleBase
	{
		public SoundGroup(IntPtr raw) : base(raw)
		{
		}

		public RESULT release()
		{
			RESULT rESULT = SoundGroup.FMOD5_SoundGroup_Release(base.getRaw());
			if (rESULT == RESULT.OK)
			{
				this.rawPtr = IntPtr.Zero;
			}
			return rESULT;
		}

		public RESULT getSystemObject(out System system)
		{
			system = null;
			IntPtr raw;
			RESULT result = SoundGroup.FMOD5_SoundGroup_GetSystemObject(this.rawPtr, out raw);
			system = new System(raw);
			return result;
		}

		public RESULT setMaxAudible(int maxaudible)
		{
			return SoundGroup.FMOD5_SoundGroup_SetMaxAudible(this.rawPtr, maxaudible);
		}

		public RESULT getMaxAudible(out int maxaudible)
		{
			return SoundGroup.FMOD5_SoundGroup_GetMaxAudible(this.rawPtr, out maxaudible);
		}

		public RESULT setMaxAudibleBehavior(SOUNDGROUP_BEHAVIOR behavior)
		{
			return SoundGroup.FMOD5_SoundGroup_SetMaxAudibleBehavior(this.rawPtr, behavior);
		}

		public RESULT getMaxAudibleBehavior(out SOUNDGROUP_BEHAVIOR behavior)
		{
			return SoundGroup.FMOD5_SoundGroup_GetMaxAudibleBehavior(this.rawPtr, out behavior);
		}

		public RESULT setMuteFadeSpeed(float speed)
		{
			return SoundGroup.FMOD5_SoundGroup_SetMuteFadeSpeed(this.rawPtr, speed);
		}

		public RESULT getMuteFadeSpeed(out float speed)
		{
			return SoundGroup.FMOD5_SoundGroup_GetMuteFadeSpeed(this.rawPtr, out speed);
		}

		public RESULT setVolume(float volume)
		{
			return SoundGroup.FMOD5_SoundGroup_SetVolume(this.rawPtr, volume);
		}

		public RESULT getVolume(out float volume)
		{
			return SoundGroup.FMOD5_SoundGroup_GetVolume(this.rawPtr, out volume);
		}

		public RESULT stop()
		{
			return SoundGroup.FMOD5_SoundGroup_Stop(this.rawPtr);
		}

		public RESULT getName(StringBuilder name, int namelen)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(name.Capacity);
			RESULT result = SoundGroup.FMOD5_SoundGroup_GetName(this.rawPtr, intPtr, namelen);
			StringMarshalHelper.NativeToBuilder(name, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public RESULT getNumSounds(out int numsounds)
		{
			return SoundGroup.FMOD5_SoundGroup_GetNumSounds(this.rawPtr, out numsounds);
		}

		public RESULT getSound(int index, out Sound sound)
		{
			sound = null;
			IntPtr raw;
			RESULT result = SoundGroup.FMOD5_SoundGroup_GetSound(this.rawPtr, index, out raw);
			sound = new Sound(raw);
			return result;
		}

		public RESULT getNumPlaying(out int numplaying)
		{
			return SoundGroup.FMOD5_SoundGroup_GetNumPlaying(this.rawPtr, out numplaying);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return SoundGroup.FMOD5_SoundGroup_SetUserData(this.rawPtr, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return SoundGroup.FMOD5_SoundGroup_GetUserData(this.rawPtr, out userdata);
		}

		[DllImport("fmod")]
		private static extern RESULT FMOD5_SoundGroup_Release(IntPtr soundgroup);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_SoundGroup_GetSystemObject(IntPtr soundgroup, out IntPtr system);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_SoundGroup_SetMaxAudible(IntPtr soundgroup, int maxaudible);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_SoundGroup_GetMaxAudible(IntPtr soundgroup, out int maxaudible);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_SoundGroup_SetMaxAudibleBehavior(IntPtr soundgroup, SOUNDGROUP_BEHAVIOR behavior);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_SoundGroup_GetMaxAudibleBehavior(IntPtr soundgroup, out SOUNDGROUP_BEHAVIOR behavior);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_SoundGroup_SetMuteFadeSpeed(IntPtr soundgroup, float speed);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_SoundGroup_GetMuteFadeSpeed(IntPtr soundgroup, out float speed);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_SoundGroup_SetVolume(IntPtr soundgroup, float volume);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_SoundGroup_GetVolume(IntPtr soundgroup, out float volume);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_SoundGroup_Stop(IntPtr soundgroup);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_SoundGroup_GetName(IntPtr soundgroup, IntPtr name, int namelen);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_SoundGroup_GetNumSounds(IntPtr soundgroup, out int numsounds);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_SoundGroup_GetSound(IntPtr soundgroup, int index, out IntPtr sound);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_SoundGroup_GetNumPlaying(IntPtr soundgroup, out int numplaying);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_SoundGroup_SetUserData(IntPtr soundgroup, IntPtr userdata);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_SoundGroup_GetUserData(IntPtr soundgroup, out IntPtr userdata);
	}
}
