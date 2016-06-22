using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	public class VCA : HandleBase
	{
		public VCA(IntPtr raw) : base(raw)
		{
		}

		public RESULT getID(out Guid id)
		{
			return VCA.FMOD_Studio_VCA_GetID(this.rawPtr, out id);
		}

		public RESULT getPath(out string path)
		{
			path = null;
			byte[] array = new byte[256];
			int num = 0;
			RESULT rESULT = VCA.FMOD_Studio_VCA_GetPath(this.rawPtr, array, array.Length, out num);
			if (rESULT == RESULT.ERR_TRUNCATED)
			{
				array = new byte[num];
				rESULT = VCA.FMOD_Studio_VCA_GetPath(this.rawPtr, array, array.Length, out num);
			}
			if (rESULT == RESULT.OK)
			{
				path = Encoding.UTF8.GetString(array, 0, num - 1);
			}
			return rESULT;
		}

		public RESULT getFaderLevel(out float volume)
		{
			return VCA.FMOD_Studio_VCA_GetFaderLevel(this.rawPtr, out volume);
		}

		public RESULT setFaderLevel(float volume)
		{
			return VCA.FMOD_Studio_VCA_SetFaderLevel(this.rawPtr, volume);
		}

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_VCA_IsValid(IntPtr vca);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_VCA_GetID(IntPtr vca, out Guid id);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_VCA_GetPath(IntPtr vca, [Out] byte[] path, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_VCA_GetFaderLevel(IntPtr vca, out float value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_VCA_SetFaderLevel(IntPtr vca, float value);

		protected override bool isValidInternal()
		{
			return VCA.FMOD_Studio_VCA_IsValid(this.rawPtr);
		}
	}
}
