using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public struct VREvent_TouchPadMove_t
	{
		[MarshalAs(UnmanagedType.I1)]
		public bool bFingerDown;

		public float flSecondsFingerDown;

		public float fValueXFirst;

		public float fValueYFirst;

		public float fValueXRaw;

		public float fValueYRaw;
	}
}
