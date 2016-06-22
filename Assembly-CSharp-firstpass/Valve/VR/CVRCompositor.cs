using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public class CVRCompositor
	{
		private IVRCompositor FnTable;

		internal CVRCompositor(IntPtr pInterface)
		{
			this.FnTable = (IVRCompositor)Marshal.PtrToStructure(pInterface, typeof(IVRCompositor));
		}

		public void SetTrackingSpace(ETrackingUniverseOrigin eOrigin)
		{
			this.FnTable.SetTrackingSpace(eOrigin);
		}

		public ETrackingUniverseOrigin GetTrackingSpace()
		{
			return this.FnTable.GetTrackingSpace();
		}

		public EVRCompositorError WaitGetPoses(TrackedDevicePose_t[] pRenderPoseArray, TrackedDevicePose_t[] pGamePoseArray)
		{
			return this.FnTable.WaitGetPoses(pRenderPoseArray, (uint)pRenderPoseArray.Length, pGamePoseArray, (uint)pGamePoseArray.Length);
		}

		public EVRCompositorError GetLastPoses(TrackedDevicePose_t[] pRenderPoseArray, TrackedDevicePose_t[] pGamePoseArray)
		{
			return this.FnTable.GetLastPoses(pRenderPoseArray, (uint)pRenderPoseArray.Length, pGamePoseArray, (uint)pGamePoseArray.Length);
		}

		public EVRCompositorError GetLastPoseForTrackedDeviceIndex(uint unDeviceIndex, ref TrackedDevicePose_t pOutputPose, ref TrackedDevicePose_t pOutputGamePose)
		{
			return this.FnTable.GetLastPoseForTrackedDeviceIndex(unDeviceIndex, ref pOutputPose, ref pOutputGamePose);
		}

		public EVRCompositorError Submit(EVREye eEye, ref Texture_t pTexture, ref VRTextureBounds_t pBounds, EVRSubmitFlags nSubmitFlags)
		{
			return this.FnTable.Submit(eEye, ref pTexture, ref pBounds, nSubmitFlags);
		}

		public void ClearLastSubmittedFrame()
		{
			this.FnTable.ClearLastSubmittedFrame();
		}

		public void PostPresentHandoff()
		{
			this.FnTable.PostPresentHandoff();
		}

		public bool GetFrameTiming(ref Compositor_FrameTiming pTiming, uint unFramesAgo)
		{
			return this.FnTable.GetFrameTiming(ref pTiming, unFramesAgo);
		}

		public float GetFrameTimeRemaining()
		{
			return this.FnTable.GetFrameTimeRemaining();
		}

		public void FadeToColor(float fSeconds, float fRed, float fGreen, float fBlue, float fAlpha, bool bBackground)
		{
			this.FnTable.FadeToColor(fSeconds, fRed, fGreen, fBlue, fAlpha, bBackground);
		}

		public void FadeGrid(float fSeconds, bool bFadeIn)
		{
			this.FnTable.FadeGrid(fSeconds, bFadeIn);
		}

		public EVRCompositorError SetSkyboxOverride(Texture_t[] pTextures)
		{
			return this.FnTable.SetSkyboxOverride(pTextures, (uint)pTextures.Length);
		}

		public void ClearSkyboxOverride()
		{
			this.FnTable.ClearSkyboxOverride();
		}

		public void CompositorBringToFront()
		{
			this.FnTable.CompositorBringToFront();
		}

		public void CompositorGoToBack()
		{
			this.FnTable.CompositorGoToBack();
		}

		public void CompositorQuit()
		{
			this.FnTable.CompositorQuit();
		}

		public bool IsFullscreen()
		{
			return this.FnTable.IsFullscreen();
		}

		public uint GetCurrentSceneFocusProcess()
		{
			return this.FnTable.GetCurrentSceneFocusProcess();
		}

		public uint GetLastFrameRenderer()
		{
			return this.FnTable.GetLastFrameRenderer();
		}

		public bool CanRenderScene()
		{
			return this.FnTable.CanRenderScene();
		}

		public void ShowMirrorWindow()
		{
			this.FnTable.ShowMirrorWindow();
		}

		public void HideMirrorWindow()
		{
			this.FnTable.HideMirrorWindow();
		}

		public bool IsMirrorWindowVisible()
		{
			return this.FnTable.IsMirrorWindowVisible();
		}

		public void CompositorDumpImages()
		{
			this.FnTable.CompositorDumpImages();
		}

		public bool ShouldAppRenderWithLowResources()
		{
			return this.FnTable.ShouldAppRenderWithLowResources();
		}

		public void ForceInterleavedReprojectionOn(bool bOverride)
		{
			this.FnTable.ForceInterleavedReprojectionOn(bOverride);
		}
	}
}
