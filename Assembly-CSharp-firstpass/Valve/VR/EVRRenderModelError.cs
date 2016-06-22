using System;

namespace Valve.VR
{
	public enum EVRRenderModelError
	{
		None,
		Loading = 100,
		NotSupported = 200,
		InvalidArg = 300,
		InvalidModel,
		NoShapes,
		MultipleShapes,
		TooManyIndices,
		MultipleTextures,
		InvalidTexture = 400
	}
}
