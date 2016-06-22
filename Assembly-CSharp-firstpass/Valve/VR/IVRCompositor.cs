using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public struct IVRCompositor
	{
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetTrackingSpace(ETrackingUniverseOrigin eOrigin);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ETrackingUniverseOrigin _GetTrackingSpace();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRCompositorError _WaitGetPoses([In] [Out] TrackedDevicePose_t[] pRenderPoseArray, uint unRenderPoseArrayCount, [In] [Out] TrackedDevicePose_t[] pGamePoseArray, uint unGamePoseArrayCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRCompositorError _GetLastPoses([In] [Out] TrackedDevicePose_t[] pRenderPoseArray, uint unRenderPoseArrayCount, [In] [Out] TrackedDevicePose_t[] pGamePoseArray, uint unGamePoseArrayCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRCompositorError _GetLastPoseForTrackedDeviceIndex(uint unDeviceIndex, ref TrackedDevicePose_t pOutputPose, ref TrackedDevicePose_t pOutputGamePose);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRCompositorError _Submit(EVREye eEye, ref Texture_t pTexture, ref VRTextureBounds_t pBounds, EVRSubmitFlags nSubmitFlags);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ClearLastSubmittedFrame();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _PostPresentHandoff();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetFrameTiming(ref Compositor_FrameTiming pTiming, uint unFramesAgo);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate float _GetFrameTimeRemaining();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _FadeToColor(float fSeconds, float fRed, float fGreen, float fBlue, float fAlpha, bool bBackground);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _FadeGrid(float fSeconds, bool bFadeIn);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRCompositorError _SetSkyboxOverride([In] [Out] Texture_t[] pTextures, uint unTextureCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ClearSkyboxOverride();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _CompositorBringToFront();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _CompositorGoToBack();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _CompositorQuit();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _IsFullscreen();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetCurrentSceneFocusProcess();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetLastFrameRenderer();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _CanRenderScene();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ShowMirrorWindow();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _HideMirrorWindow();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _IsMirrorWindowVisible();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _CompositorDumpImages();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _ShouldAppRenderWithLowResources();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ForceInterleavedReprojectionOn(bool bOverride);

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._SetTrackingSpace SetTrackingSpace;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetTrackingSpace GetTrackingSpace;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._WaitGetPoses WaitGetPoses;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetLastPoses GetLastPoses;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetLastPoseForTrackedDeviceIndex GetLastPoseForTrackedDeviceIndex;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._Submit Submit;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._ClearLastSubmittedFrame ClearLastSubmittedFrame;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._PostPresentHandoff PostPresentHandoff;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetFrameTiming GetFrameTiming;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetFrameTimeRemaining GetFrameTimeRemaining;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._FadeToColor FadeToColor;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._FadeGrid FadeGrid;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._SetSkyboxOverride SetSkyboxOverride;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._ClearSkyboxOverride ClearSkyboxOverride;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._CompositorBringToFront CompositorBringToFront;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._CompositorGoToBack CompositorGoToBack;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._CompositorQuit CompositorQuit;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._IsFullscreen IsFullscreen;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetCurrentSceneFocusProcess GetCurrentSceneFocusProcess;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._GetLastFrameRenderer GetLastFrameRenderer;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._CanRenderScene CanRenderScene;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._ShowMirrorWindow ShowMirrorWindow;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._HideMirrorWindow HideMirrorWindow;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._IsMirrorWindowVisible IsMirrorWindowVisible;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._CompositorDumpImages CompositorDumpImages;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._ShouldAppRenderWithLowResources ShouldAppRenderWithLowResources;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRCompositor._ForceInterleavedReprojectionOn ForceInterleavedReprojectionOn;
	}
}
