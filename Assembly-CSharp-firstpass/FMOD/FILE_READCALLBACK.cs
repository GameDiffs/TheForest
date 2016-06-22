using System;

namespace FMOD
{
	public delegate RESULT FILE_READCALLBACK(IntPtr handle, IntPtr buffer, uint sizebytes, ref uint bytesread, IntPtr userdata);
}
