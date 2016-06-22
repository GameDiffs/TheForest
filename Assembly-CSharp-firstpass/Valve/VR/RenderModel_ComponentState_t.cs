using System;

namespace Valve.VR
{
	public struct RenderModel_ComponentState_t
	{
		public HmdMatrix34_t mTrackingToComponentRenderModel;

		public HmdMatrix34_t mTrackingToComponentLocal;

		public uint uProperties;
	}
}
