using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct ERRORCALLBACK_INFO
	{
		public RESULT result;

		public ERRORCALLBACK_INSTANCETYPE instancetype;

		public IntPtr instance;

		private IntPtr functionname_internal;

		private IntPtr functionparams_internal;

		public string functionname
		{
			get
			{
				return Marshal.PtrToStringAnsi(this.functionname_internal);
			}
		}

		public string functionparams
		{
			get
			{
				return Marshal.PtrToStringAnsi(this.functionparams_internal);
			}
		}
	}
}
