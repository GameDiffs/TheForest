using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public class Reverb3D : HandleBase
	{
		public Reverb3D(IntPtr raw) : base(raw)
		{
		}

		public RESULT release()
		{
			RESULT rESULT = Reverb3D.FMOD5_Reverb3D_Release(base.getRaw());
			if (rESULT == RESULT.OK)
			{
				this.rawPtr = IntPtr.Zero;
			}
			return rESULT;
		}

		public RESULT set3DAttributes(ref VECTOR position, float mindistance, float maxdistance)
		{
			return Reverb3D.FMOD5_Reverb3D_Set3DAttributes(this.rawPtr, ref position, mindistance, maxdistance);
		}

		public RESULT get3DAttributes(ref VECTOR position, ref float mindistance, ref float maxdistance)
		{
			return Reverb3D.FMOD5_Reverb3D_Get3DAttributes(this.rawPtr, ref position, ref mindistance, ref maxdistance);
		}

		public RESULT setProperties(ref REVERB_PROPERTIES properties)
		{
			return Reverb3D.FMOD5_Reverb3D_SetProperties(this.rawPtr, ref properties);
		}

		public RESULT getProperties(ref REVERB_PROPERTIES properties)
		{
			return Reverb3D.FMOD5_Reverb3D_GetProperties(this.rawPtr, ref properties);
		}

		public RESULT setActive(bool active)
		{
			return Reverb3D.FMOD5_Reverb3D_SetActive(this.rawPtr, active);
		}

		public RESULT getActive(out bool active)
		{
			return Reverb3D.FMOD5_Reverb3D_GetActive(this.rawPtr, out active);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return Reverb3D.FMOD5_Reverb3D_SetUserData(this.rawPtr, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return Reverb3D.FMOD5_Reverb3D_GetUserData(this.rawPtr, out userdata);
		}

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Reverb3D_Release(IntPtr reverb);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Reverb3D_Set3DAttributes(IntPtr reverb, ref VECTOR position, float mindistance, float maxdistance);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Reverb3D_Get3DAttributes(IntPtr reverb, ref VECTOR position, ref float mindistance, ref float maxdistance);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Reverb3D_SetProperties(IntPtr reverb, ref REVERB_PROPERTIES properties);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Reverb3D_GetProperties(IntPtr reverb, ref REVERB_PROPERTIES properties);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Reverb3D_SetActive(IntPtr reverb, bool active);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Reverb3D_GetActive(IntPtr reverb, out bool active);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Reverb3D_SetUserData(IntPtr reverb, IntPtr userdata);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Reverb3D_GetUserData(IntPtr reverb, out IntPtr userdata);
	}
}
