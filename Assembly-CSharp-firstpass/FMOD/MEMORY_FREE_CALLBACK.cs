using System;

namespace FMOD
{
	public delegate void MEMORY_FREE_CALLBACK(IntPtr ptr, MEMORY_TYPE type, StringWrapper sourcestr);
}
