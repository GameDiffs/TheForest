using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public struct VREvent_SeatedZeroPoseReset_t
	{
		[MarshalAs(UnmanagedType.I1)]
		public bool bResetBySystemMenu;
	}
}
