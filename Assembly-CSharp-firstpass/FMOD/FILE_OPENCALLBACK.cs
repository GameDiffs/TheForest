using System;

namespace FMOD
{
	public delegate RESULT FILE_OPENCALLBACK(StringWrapper name, ref uint filesize, ref IntPtr handle, IntPtr userdata);
}
