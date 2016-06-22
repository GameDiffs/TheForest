using System;

namespace Valve.VR
{
	public struct Compositor_FrameTiming
	{
		public uint m_nSize;

		public uint m_nFrameIndex;

		public uint m_nNumFramePresents;

		public uint m_nNumDroppedFrames;

		public double m_flSystemTimeInSeconds;

		public float m_flSceneRenderGpuMs;

		public float m_flTotalRenderGpuMs;

		public float m_flCompositorRenderGpuMs;

		public float m_flCompositorRenderCpuMs;

		public float m_flCompositorIdleCpuMs;

		public float m_flClientFrameIntervalMs;

		public float m_flPresentCallCpuMs;

		public float m_flWaitForPresentCpuMs;

		public float m_flSubmitFrameMs;

		public float m_flWaitGetPosesCalledMs;

		public float m_flNewPosesReadyMs;

		public float m_flNewFrameReadyMs;

		public float m_flCompositorUpdateStartMs;

		public float m_flCompositorUpdateEndMs;

		public float m_flCompositorRenderStartMs;

		public TrackedDevicePose_t m_HmdPose;

		public int m_nFidelityLevel;
	}
}
