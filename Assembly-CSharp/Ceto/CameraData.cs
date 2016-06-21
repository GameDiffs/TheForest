using System;

namespace Ceto
{
	public class CameraData
	{
		public bool checkedForSettings;

		public OceanCameraSettings settings;

		public MaskData mask;

		public DepthData depth;

		public WaveOverlayData overlay;

		public ProjectionData projection;

		public ReflectionData reflection;
	}
}
