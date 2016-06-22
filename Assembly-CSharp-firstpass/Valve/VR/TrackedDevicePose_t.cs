using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public struct TrackedDevicePose_t
	{
		public HmdMatrix34_t mDeviceToAbsoluteTracking;

		public HmdVector3_t vVelocity;

		public HmdVector3_t vAngularVelocity;

		public ETrackingResult eTrackingResult;

		[MarshalAs(UnmanagedType.I1)]
		public bool bPoseIsValid;

		[MarshalAs(UnmanagedType.I1)]
		public bool bDeviceIsConnected;
	}
}
