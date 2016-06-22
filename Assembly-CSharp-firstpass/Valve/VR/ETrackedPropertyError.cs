using System;

namespace Valve.VR
{
	public enum ETrackedPropertyError
	{
		TrackedProp_Success,
		TrackedProp_WrongDataType,
		TrackedProp_WrongDeviceClass,
		TrackedProp_BufferTooSmall,
		TrackedProp_UnknownProperty,
		TrackedProp_InvalidDevice,
		TrackedProp_CouldNotContactServer,
		TrackedProp_ValueNotProvidedByDevice,
		TrackedProp_StringExceedsMaximumLength,
		TrackedProp_NotYetAvailable
	}
}
