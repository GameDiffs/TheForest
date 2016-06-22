using System;

namespace Valve.VR
{
	public enum EVRCompositorError
	{
		None,
		IncompatibleVersion = 100,
		DoNotHaveFocus,
		InvalidTexture,
		IsNotSceneApplication,
		TextureIsOnWrongDevice,
		TextureUsesUnsupportedFormat,
		SharedTexturesNotSupported,
		IndexOutOfRange
	}
}
