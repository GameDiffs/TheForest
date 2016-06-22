using System;

namespace FMOD
{
	[Flags]
	public enum CHANNELMASK : uint
	{
		FRONT_LEFT = 1u,
		FRONT_RIGHT = 2u,
		FRONT_CENTER = 4u,
		LOW_FREQUENCY = 8u,
		SURROUND_LEFT = 16u,
		SURROUND_RIGHT = 32u,
		BACK_LEFT = 64u,
		BACK_RIGHT = 128u,
		BACK_CENTER = 256u,
		MONO = 1u,
		STEREO = 3u,
		LRC = 7u,
		QUAD = 51u,
		SURROUND = 55u,
		_5POINT1 = 63u,
		_5POINT1_REARS = 207u,
		_7POINT0 = 247u,
		_7POINT1 = 255u
	}
}
