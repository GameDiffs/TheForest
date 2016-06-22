using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public class CVRSystem
	{
		private IVRSystem FnTable;

		internal CVRSystem(IntPtr pInterface)
		{
			this.FnTable = (IVRSystem)Marshal.PtrToStructure(pInterface, typeof(IVRSystem));
		}

		public void GetRecommendedRenderTargetSize(ref uint pnWidth, ref uint pnHeight)
		{
			pnWidth = 0u;
			pnHeight = 0u;
			this.FnTable.GetRecommendedRenderTargetSize(ref pnWidth, ref pnHeight);
		}

		public HmdMatrix44_t GetProjectionMatrix(EVREye eEye, float fNearZ, float fFarZ, EGraphicsAPIConvention eProjType)
		{
			return this.FnTable.GetProjectionMatrix(eEye, fNearZ, fFarZ, eProjType);
		}

		public void GetProjectionRaw(EVREye eEye, ref float pfLeft, ref float pfRight, ref float pfTop, ref float pfBottom)
		{
			pfLeft = 0f;
			pfRight = 0f;
			pfTop = 0f;
			pfBottom = 0f;
			this.FnTable.GetProjectionRaw(eEye, ref pfLeft, ref pfRight, ref pfTop, ref pfBottom);
		}

		public DistortionCoordinates_t ComputeDistortion(EVREye eEye, float fU, float fV)
		{
			return this.FnTable.ComputeDistortion(eEye, fU, fV);
		}

		public HmdMatrix34_t GetEyeToHeadTransform(EVREye eEye)
		{
			return this.FnTable.GetEyeToHeadTransform(eEye);
		}

		public bool GetTimeSinceLastVsync(ref float pfSecondsSinceLastVsync, ref ulong pulFrameCounter)
		{
			pfSecondsSinceLastVsync = 0f;
			pulFrameCounter = 0uL;
			return this.FnTable.GetTimeSinceLastVsync(ref pfSecondsSinceLastVsync, ref pulFrameCounter);
		}

		public int GetD3D9AdapterIndex()
		{
			return this.FnTable.GetD3D9AdapterIndex();
		}

		public void GetDXGIOutputInfo(ref int pnAdapterIndex)
		{
			pnAdapterIndex = 0;
			this.FnTable.GetDXGIOutputInfo(ref pnAdapterIndex);
		}

		public bool IsDisplayOnDesktop()
		{
			return this.FnTable.IsDisplayOnDesktop();
		}

		public bool SetDisplayVisibility(bool bIsVisibleOnDesktop)
		{
			return this.FnTable.SetDisplayVisibility(bIsVisibleOnDesktop);
		}

		public void GetDeviceToAbsoluteTrackingPose(ETrackingUniverseOrigin eOrigin, float fPredictedSecondsToPhotonsFromNow, TrackedDevicePose_t[] pTrackedDevicePoseArray)
		{
			this.FnTable.GetDeviceToAbsoluteTrackingPose(eOrigin, fPredictedSecondsToPhotonsFromNow, pTrackedDevicePoseArray, (uint)pTrackedDevicePoseArray.Length);
		}

		public void ResetSeatedZeroPose()
		{
			this.FnTable.ResetSeatedZeroPose();
		}

		public HmdMatrix34_t GetSeatedZeroPoseToStandingAbsoluteTrackingPose()
		{
			return this.FnTable.GetSeatedZeroPoseToStandingAbsoluteTrackingPose();
		}

		public HmdMatrix34_t GetRawZeroPoseToStandingAbsoluteTrackingPose()
		{
			return this.FnTable.GetRawZeroPoseToStandingAbsoluteTrackingPose();
		}

		public uint GetSortedTrackedDeviceIndicesOfClass(ETrackedDeviceClass eTrackedDeviceClass, uint[] punTrackedDeviceIndexArray, uint unRelativeToTrackedDeviceIndex)
		{
			return this.FnTable.GetSortedTrackedDeviceIndicesOfClass(eTrackedDeviceClass, punTrackedDeviceIndexArray, (uint)punTrackedDeviceIndexArray.Length, unRelativeToTrackedDeviceIndex);
		}

		public EDeviceActivityLevel GetTrackedDeviceActivityLevel(uint unDeviceId)
		{
			return this.FnTable.GetTrackedDeviceActivityLevel(unDeviceId);
		}

		public void ApplyTransform(ref TrackedDevicePose_t pOutputPose, ref TrackedDevicePose_t pTrackedDevicePose, ref HmdMatrix34_t pTransform)
		{
			this.FnTable.ApplyTransform(ref pOutputPose, ref pTrackedDevicePose, ref pTransform);
		}

		public uint GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole unDeviceType)
		{
			return this.FnTable.GetTrackedDeviceIndexForControllerRole(unDeviceType);
		}

		public ETrackedControllerRole GetControllerRoleForTrackedDeviceIndex(uint unDeviceIndex)
		{
			return this.FnTable.GetControllerRoleForTrackedDeviceIndex(unDeviceIndex);
		}

		public ETrackedDeviceClass GetTrackedDeviceClass(uint unDeviceIndex)
		{
			return this.FnTable.GetTrackedDeviceClass(unDeviceIndex);
		}

		public bool IsTrackedDeviceConnected(uint unDeviceIndex)
		{
			return this.FnTable.IsTrackedDeviceConnected(unDeviceIndex);
		}

		public bool GetBoolTrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, ref ETrackedPropertyError pError)
		{
			return this.FnTable.GetBoolTrackedDeviceProperty(unDeviceIndex, prop, ref pError);
		}

		public float GetFloatTrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, ref ETrackedPropertyError pError)
		{
			return this.FnTable.GetFloatTrackedDeviceProperty(unDeviceIndex, prop, ref pError);
		}

		public int GetInt32TrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, ref ETrackedPropertyError pError)
		{
			return this.FnTable.GetInt32TrackedDeviceProperty(unDeviceIndex, prop, ref pError);
		}

		public ulong GetUint64TrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, ref ETrackedPropertyError pError)
		{
			return this.FnTable.GetUint64TrackedDeviceProperty(unDeviceIndex, prop, ref pError);
		}

		public HmdMatrix34_t GetMatrix34TrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, ref ETrackedPropertyError pError)
		{
			return this.FnTable.GetMatrix34TrackedDeviceProperty(unDeviceIndex, prop, ref pError);
		}

		public uint GetStringTrackedDeviceProperty(uint unDeviceIndex, ETrackedDeviceProperty prop, StringBuilder pchValue, uint unBufferSize, ref ETrackedPropertyError pError)
		{
			return this.FnTable.GetStringTrackedDeviceProperty(unDeviceIndex, prop, pchValue, unBufferSize, ref pError);
		}

		public string GetPropErrorNameFromEnum(ETrackedPropertyError error)
		{
			IntPtr ptr = this.FnTable.GetPropErrorNameFromEnum(error);
			return (string)Marshal.PtrToStructure(ptr, typeof(string));
		}

		public bool PollNextEvent(ref VREvent_t pEvent, uint uncbVREvent)
		{
			return this.FnTable.PollNextEvent(ref pEvent, uncbVREvent);
		}

		public bool PollNextEventWithPose(ETrackingUniverseOrigin eOrigin, ref VREvent_t pEvent, uint uncbVREvent, ref TrackedDevicePose_t pTrackedDevicePose)
		{
			return this.FnTable.PollNextEventWithPose(eOrigin, ref pEvent, uncbVREvent, ref pTrackedDevicePose);
		}

		public string GetEventTypeNameFromEnum(EVREventType eType)
		{
			IntPtr ptr = this.FnTable.GetEventTypeNameFromEnum(eType);
			return (string)Marshal.PtrToStructure(ptr, typeof(string));
		}

		public HiddenAreaMesh_t GetHiddenAreaMesh(EVREye eEye)
		{
			return this.FnTable.GetHiddenAreaMesh(eEye);
		}

		public bool GetControllerState(uint unControllerDeviceIndex, ref VRControllerState_t pControllerState)
		{
			return this.FnTable.GetControllerState(unControllerDeviceIndex, ref pControllerState);
		}

		public bool GetControllerStateWithPose(ETrackingUniverseOrigin eOrigin, uint unControllerDeviceIndex, ref VRControllerState_t pControllerState, ref TrackedDevicePose_t pTrackedDevicePose)
		{
			return this.FnTable.GetControllerStateWithPose(eOrigin, unControllerDeviceIndex, ref pControllerState, ref pTrackedDevicePose);
		}

		public void TriggerHapticPulse(uint unControllerDeviceIndex, uint unAxisId, char usDurationMicroSec)
		{
			this.FnTable.TriggerHapticPulse(unControllerDeviceIndex, unAxisId, usDurationMicroSec);
		}

		public string GetButtonIdNameFromEnum(EVRButtonId eButtonId)
		{
			IntPtr ptr = this.FnTable.GetButtonIdNameFromEnum(eButtonId);
			return (string)Marshal.PtrToStructure(ptr, typeof(string));
		}

		public string GetControllerAxisTypeNameFromEnum(EVRControllerAxisType eAxisType)
		{
			IntPtr ptr = this.FnTable.GetControllerAxisTypeNameFromEnum(eAxisType);
			return (string)Marshal.PtrToStructure(ptr, typeof(string));
		}

		public bool CaptureInputFocus()
		{
			return this.FnTable.CaptureInputFocus();
		}

		public void ReleaseInputFocus()
		{
			this.FnTable.ReleaseInputFocus();
		}

		public bool IsInputFocusCapturedByAnotherProcess()
		{
			return this.FnTable.IsInputFocusCapturedByAnotherProcess();
		}

		public uint DriverDebugRequest(uint unDeviceIndex, string pchRequest, string pchResponseBuffer, uint unResponseBufferSize)
		{
			return this.FnTable.DriverDebugRequest(unDeviceIndex, pchRequest, pchResponseBuffer, unResponseBufferSize);
		}

		public EVRFirmwareError PerformFirmwareUpdate(uint unDeviceIndex)
		{
			return this.FnTable.PerformFirmwareUpdate(unDeviceIndex);
		}

		public void AcknowledgeQuit_Exiting()
		{
			this.FnTable.AcknowledgeQuit_Exiting();
		}

		public void AcknowledgeQuit_UserPrompt()
		{
			this.FnTable.AcknowledgeQuit_UserPrompt();
		}

		public void PerformanceTestEnableCapture(bool bEnable)
		{
			this.FnTable.PerformanceTestEnableCapture(bEnable);
		}

		public void PerformanceTestReportFidelityLevelChange(int nFidelityLevel)
		{
			this.FnTable.PerformanceTestReportFidelityLevelChange(nFidelityLevel);
		}
	}
}
