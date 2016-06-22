using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public struct RenderModel_ControllerMode_State_t
	{
		[MarshalAs(UnmanagedType.I1)]
		public bool bScrollWheelVisible;
	}
}
