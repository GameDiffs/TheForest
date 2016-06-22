using System;

namespace FMOD
{
	public delegate IntPtr MEMORY_REALLOC_CALLBACK(IntPtr ptr, uint size, MEMORY_TYPE type, StringWrapper sourcestr);
}
