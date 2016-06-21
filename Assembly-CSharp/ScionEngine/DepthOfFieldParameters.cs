using System;
using UnityEngine;

namespace ScionEngine
{
	public struct DepthOfFieldParameters
	{
		public bool useMedianFilter;

		public DepthOfFieldQuality quality;

		public DepthFocusMode depthFocusMode;

		public float maxCoCRadius;

		public Vector2 pointAveragePosition;

		public float pointAverageRange;

		public bool visualizePointFocus;

		public float depthAdaptionSpeed;

		public float focalDistance;

		public float focalRange;
	}
}
