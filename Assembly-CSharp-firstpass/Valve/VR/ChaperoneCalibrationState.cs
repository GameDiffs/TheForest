using System;

namespace Valve.VR
{
	public enum ChaperoneCalibrationState
	{
		OK = 1,
		Warning = 100,
		Warning_BaseStationMayHaveMoved,
		Warning_BaseStationRemoved,
		Warning_SeatedBoundsInvalid,
		Error = 200,
		Error_BaseStationUninitalized,
		Error_BaseStationConflict,
		Error_PlayAreaInvalid,
		Error_CollisionBoundsInvalid
	}
}
