using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
	public struct StringWrapper
	{
		private IntPtr nativeUtf8Ptr;

		public static implicit operator string(StringWrapper fstring)
		{
			if (fstring.nativeUtf8Ptr == IntPtr.Zero)
			{
				return string.Empty;
			}
			int num = 0;
			while (Marshal.ReadByte(fstring.nativeUtf8Ptr, num) != 0)
			{
				num++;
			}
			if (num > 0)
			{
				byte[] array = new byte[num];
				Marshal.Copy(fstring.nativeUtf8Ptr, array, 0, num);
				return Encoding.UTF8.GetString(array, 0, num);
			}
			return string.Empty;
		}
	}
}
