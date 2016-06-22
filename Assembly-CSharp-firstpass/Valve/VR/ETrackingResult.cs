using System;

namespace Valve.VR
{
	public enum ETrackingResult
	{
		Uninitialized = 1,
		Calibrating_InProgress = 100,
		Calibrating_OutOfRange,
		Running_OK = 200,
		Running_OutOfRange
	}
}
