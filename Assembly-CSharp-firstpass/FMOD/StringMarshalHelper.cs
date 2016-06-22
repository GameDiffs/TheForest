using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD
{
	internal class StringMarshalHelper
	{
		internal static void NativeToBuilder(StringBuilder builder, IntPtr nativeMem)
		{
			byte[] array = new byte[builder.Capacity];
			Marshal.Copy(nativeMem, array, 0, builder.Capacity);
			int num = Array.IndexOf<byte>(array, 0);
			if (num > 0)
			{
				string @string = Encoding.UTF8.GetString(array, 0, num);
				builder.Append(@string);
			}
		}
	}
}
