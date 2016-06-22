using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public class Factory
	{
		public static RESULT System_Create(out System system)
		{
			system = null;
			IntPtr raw = 0;
			RESULT rESULT = Factory.FMOD5_System_Create(out raw);
			if (rESULT != RESULT.OK)
			{
				return rESULT;
			}
			system = new System(raw);
			return rESULT;
		}

		[DllImport("fmod")]
		private static extern RESULT FMOD5_System_Create(out IntPtr system);
	}
}
