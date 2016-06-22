using System;

namespace Valve.VR
{
	public enum EVRApplicationError
	{
		None,
		AppKeyAlreadyExists = 100,
		NoManifest,
		NoApplication,
		InvalidIndex,
		UnknownApplication,
		IPCFailed,
		ApplicationAlreadyRunning,
		InvalidManifest,
		InvalidApplication,
		LaunchFailed,
		ApplicationAlreadyStarting,
		LaunchInProgress,
		OldApplicationQuitting,
		TransitionAborted,
		BufferTooSmall = 200,
		PropertyNotSet,
		UnknownProperty
	}
}
