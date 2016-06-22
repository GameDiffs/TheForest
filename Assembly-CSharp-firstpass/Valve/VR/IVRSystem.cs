using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public struct IVRSystem
	{
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _GetRecommendedRenderTargetSize(ref uint pnWidth, ref uint pnHeight);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate HmdMatrix44_t _GetProjectionMatrix(EVREye eEye, float fNearZ, float fFarZ, EGraphicsAPIConvention eProjType);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _GetProjectionRaw(EVREye eEye, ref float pfLeft, ref float pfRight, ref float pfTop, ref float pfBottom);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate DistortionCoordinates_t _ComputeDistortion(EVREye eEye, float fU, float fV);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate HmdMatrix34_t _GetEyeToHeadTransform(EVREye eEye);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetTimeSinceLastVsync(ref float pfSecondsSinceLastVsync, ref ulong pulFrameCounter);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate int _GetD3D9AdapterIndex();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _GetDXGIOutputInfo(ref int pnAdapterIndex);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _IsDisplayOnDesktop();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _SetDisplayVisibility(bool bIsVisibleOnDesktop);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin eOrigin, float fPredictedSecondsToPhotonsFromNow, [In] [Out] TrackedDevicePose_t[] pTrackedDevicePoseArray, uint unTrackedDevicePoseArrayCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ResetSeatedZeroPose();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate HmdMatrix34_t _GetSeatedZeroPoseToStandingAbsoluteTrackingPose();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate HmdMatrix34_t _GetRawZeroPoseToStandingAbsoluteTrackingPose();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetSortedTrackedDeviceIndicesOfClass(ETrackedDeviceClass eTrackedDeviceClass, [In] [Out] uint[] punTrackedDeviceIndexArray, uint unTrackedDeviceIndexArrayCount, uint unRelativeToTrackedDeviceIndex);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EDeviceActivityLevel _GetTrackedDeviceActivityLevel(uint unDeviceId);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ApplyTransform(ref TrackedDevicePose_t pOutputPose, ref TrackedDevicePose_t pTrackedDevicePose, ref HmdMatrix34_t pTransform);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole unDeviceType);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ETrackedControllerRole _GetControllerRoleForTrackedDeviceIndex(uint unDeviceIndex);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ETrackedDeviceClass _GetTrackedDeviceClass(uint unDeviceIndex);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _IsTrackedDeviceConnected(uint unDeviceIndex);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetBoolTrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, ref ETrackedPropertyError pError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate float _GetFloatTrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, ref ETrackedPropertyError pError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate int _GetInt32TrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, ref ETrackedPropertyError pError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ulong _GetUint64TrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, ref ETrackedPropertyError pError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate HmdMatrix34_t _GetMatrix34TrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, ref ETrackedPropertyError pError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetStringTrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, StringBuilder pchValue, uint unBufferSize, ref ETrackedPropertyError pError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr _GetPropErrorNameFromEnum(ETrackedPropertyError error);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _PollNextEvent(ref VREvent_t pEvent, uint uncbVREvent);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _PollNextEventWithPose(ETrackingUniverseOrigin eOrigin, ref VREvent_t pEvent, uint uncbVREvent, ref TrackedDevicePose_t pTrackedDevicePose);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr _GetEventTypeNameFromEnum(EVREventType eType);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate HiddenAreaMesh_t _GetHiddenAreaMesh(EVREye eEye);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetControllerState(uint unControllerDeviceIndex, ref VRControllerState_t pControllerState);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetControllerStateWithPose(ETrackingUniverseOrigin eOrigin, uint unControllerDeviceIndex, ref VRControllerState_t pControllerState, ref TrackedDevicePose_t pTrackedDevicePose);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _TriggerHapticPulse(uint unControllerDeviceIndex, uint unAxisId, char usDurationMicroSec);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr _GetButtonIdNameFromEnum(EVRButtonId eButtonId);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr _GetControllerAxisTypeNameFromEnum(EVRControllerAxisType eAxisType);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _CaptureInputFocus();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ReleaseInputFocus();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _IsInputFocusCapturedByAnotherProcess();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _DriverDebugRequest(uint unDeviceIndex, string pchRequest, string pchResponseBuffer, uint unResponseBufferSize);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRFirmwareError _PerformFirmwareUpdate(uint unDeviceIndex);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _AcknowledgeQuit_Exiting();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _AcknowledgeQuit_UserPrompt();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _PerformanceTestEnableCapture(bool bEnable);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _PerformanceTestReportFidelityLevelChange(int nFidelityLevel);

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetRecommendedRenderTargetSize GetRecommendedRenderTargetSize;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetProjectionMatrix GetProjectionMatrix;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetProjectionRaw GetProjectionRaw;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._ComputeDistortion ComputeDistortion;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetEyeToHeadTransform GetEyeToHeadTransform;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetTimeSinceLastVsync GetTimeSinceLastVsync;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetD3D9AdapterIndex GetD3D9AdapterIndex;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetDXGIOutputInfo GetDXGIOutputInfo;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._IsDisplayOnDesktop IsDisplayOnDesktop;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._SetDisplayVisibility SetDisplayVisibility;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetDeviceToAbsoluteTrackingPose GetDeviceToAbsoluteTrackingPose;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._ResetSeatedZeroPose ResetSeatedZeroPose;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetSeatedZeroPoseToStandingAbsoluteTrackingPose GetSeatedZeroPoseToStandingAbsoluteTrackingPose;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetRawZeroPoseToStandingAbsoluteTrackingPose GetRawZeroPoseToStandingAbsoluteTrackingPose;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetSortedTrackedDeviceIndicesOfClass GetSortedTrackedDeviceIndicesOfClass;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetTrackedDeviceActivityLevel GetTrackedDeviceActivityLevel;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._ApplyTransform ApplyTransform;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetTrackedDeviceIndexForControllerRole GetTrackedDeviceIndexForControllerRole;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetControllerRoleForTrackedDeviceIndex GetControllerRoleForTrackedDeviceIndex;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetTrackedDeviceClass GetTrackedDeviceClass;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._IsTrackedDeviceConnected IsTrackedDeviceConnected;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetBoolTrackedDeviceProperty GetBoolTrackedDeviceProperty;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetFloatTrackedDeviceProperty GetFloatTrackedDeviceProperty;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetInt32TrackedDeviceProperty GetInt32TrackedDeviceProperty;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetUint64TrackedDeviceProperty GetUint64TrackedDeviceProperty;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetMatrix34TrackedDeviceProperty GetMatrix34TrackedDeviceProperty;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetStringTrackedDeviceProperty GetStringTrackedDeviceProperty;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetPropErrorNameFromEnum GetPropErrorNameFromEnum;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._PollNextEvent PollNextEvent;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._PollNextEventWithPose PollNextEventWithPose;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetEventTypeNameFromEnum GetEventTypeNameFromEnum;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetHiddenAreaMesh GetHiddenAreaMesh;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetControllerState GetControllerState;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetControllerStateWithPose GetControllerStateWithPose;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._TriggerHapticPulse TriggerHapticPulse;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetButtonIdNameFromEnum GetButtonIdNameFromEnum;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._GetControllerAxisTypeNameFromEnum GetControllerAxisTypeNameFromEnum;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._CaptureInputFocus CaptureInputFocus;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._ReleaseInputFocus ReleaseInputFocus;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._IsInputFocusCapturedByAnotherProcess IsInputFocusCapturedByAnotherProcess;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._DriverDebugRequest DriverDebugRequest;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._PerformFirmwareUpdate PerformFirmwareUpdate;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._AcknowledgeQuit_Exiting AcknowledgeQuit_Exiting;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._AcknowledgeQuit_UserPrompt AcknowledgeQuit_UserPrompt;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._PerformanceTestEnableCapture PerformanceTestEnableCapture;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSystem._PerformanceTestReportFidelityLevelChange PerformanceTestReportFidelityLevelChange;
	}
}
