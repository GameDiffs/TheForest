using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public class CVROverlay
	{
		private IVROverlay FnTable;

		internal CVROverlay(IntPtr pInterface)
		{
			this.FnTable = (IVROverlay)Marshal.PtrToStructure(pInterface, typeof(IVROverlay));
		}

		public EVROverlayError FindOverlay(string pchOverlayKey, ref ulong pOverlayHandle)
		{
			pOverlayHandle = 0uL;
			return this.FnTable.FindOverlay(pchOverlayKey, ref pOverlayHandle);
		}

		public EVROverlayError CreateOverlay(string pchOverlayKey, string pchOverlayFriendlyName, ref ulong pOverlayHandle)
		{
			pOverlayHandle = 0uL;
			return this.FnTable.CreateOverlay(pchOverlayKey, pchOverlayFriendlyName, ref pOverlayHandle);
		}

		public EVROverlayError DestroyOverlay(ulong ulOverlayHandle)
		{
			return this.FnTable.DestroyOverlay(ulOverlayHandle);
		}

		public EVROverlayError SetHighQualityOverlay(ulong ulOverlayHandle)
		{
			return this.FnTable.SetHighQualityOverlay(ulOverlayHandle);
		}

		public ulong GetHighQualityOverlay()
		{
			return this.FnTable.GetHighQualityOverlay();
		}

		public uint GetOverlayKey(ulong ulOverlayHandle, StringBuilder pchValue, uint unBufferSize, ref EVROverlayError pError)
		{
			return this.FnTable.GetOverlayKey(ulOverlayHandle, pchValue, unBufferSize, ref pError);
		}

		public uint GetOverlayName(ulong ulOverlayHandle, StringBuilder pchValue, uint unBufferSize, ref EVROverlayError pError)
		{
			return this.FnTable.GetOverlayName(ulOverlayHandle, pchValue, unBufferSize, ref pError);
		}

		public EVROverlayError GetOverlayImageData(ulong ulOverlayHandle, IntPtr pvBuffer, uint unBufferSize, ref uint punWidth, ref uint punHeight)
		{
			punWidth = 0u;
			punHeight = 0u;
			return this.FnTable.GetOverlayImageData(ulOverlayHandle, pvBuffer, unBufferSize, ref punWidth, ref punHeight);
		}

		public string GetOverlayErrorNameFromEnum(EVROverlayError error)
		{
			IntPtr ptr = this.FnTable.GetOverlayErrorNameFromEnum(error);
			return (string)Marshal.PtrToStructure(ptr, typeof(string));
		}

		public EVROverlayError SetOverlayFlag(ulong ulOverlayHandle, VROverlayFlags eOverlayFlag, bool bEnabled)
		{
			return this.FnTable.SetOverlayFlag(ulOverlayHandle, eOverlayFlag, bEnabled);
		}

		public EVROverlayError GetOverlayFlag(ulong ulOverlayHandle, VROverlayFlags eOverlayFlag, ref bool pbEnabled)
		{
			pbEnabled = false;
			return this.FnTable.GetOverlayFlag(ulOverlayHandle, eOverlayFlag, ref pbEnabled);
		}

		public EVROverlayError SetOverlayColor(ulong ulOverlayHandle, float fRed, float fGreen, float fBlue)
		{
			return this.FnTable.SetOverlayColor(ulOverlayHandle, fRed, fGreen, fBlue);
		}

		public EVROverlayError GetOverlayColor(ulong ulOverlayHandle, ref float pfRed, ref float pfGreen, ref float pfBlue)
		{
			pfRed = 0f;
			pfGreen = 0f;
			pfBlue = 0f;
			return this.FnTable.GetOverlayColor(ulOverlayHandle, ref pfRed, ref pfGreen, ref pfBlue);
		}

		public EVROverlayError SetOverlayAlpha(ulong ulOverlayHandle, float fAlpha)
		{
			return this.FnTable.SetOverlayAlpha(ulOverlayHandle, fAlpha);
		}

		public EVROverlayError GetOverlayAlpha(ulong ulOverlayHandle, ref float pfAlpha)
		{
			pfAlpha = 0f;
			return this.FnTable.GetOverlayAlpha(ulOverlayHandle, ref pfAlpha);
		}

		public EVROverlayError SetOverlayWidthInMeters(ulong ulOverlayHandle, float fWidthInMeters)
		{
			return this.FnTable.SetOverlayWidthInMeters(ulOverlayHandle, fWidthInMeters);
		}

		public EVROverlayError GetOverlayWidthInMeters(ulong ulOverlayHandle, ref float pfWidthInMeters)
		{
			pfWidthInMeters = 0f;
			return this.FnTable.GetOverlayWidthInMeters(ulOverlayHandle, ref pfWidthInMeters);
		}

		public EVROverlayError SetOverlayAutoCurveDistanceRangeInMeters(ulong ulOverlayHandle, float fMinDistanceInMeters, float fMaxDistanceInMeters)
		{
			return this.FnTable.SetOverlayAutoCurveDistanceRangeInMeters(ulOverlayHandle, fMinDistanceInMeters, fMaxDistanceInMeters);
		}

		public EVROverlayError GetOverlayAutoCurveDistanceRangeInMeters(ulong ulOverlayHandle, ref float pfMinDistanceInMeters, ref float pfMaxDistanceInMeters)
		{
			pfMinDistanceInMeters = 0f;
			pfMaxDistanceInMeters = 0f;
			return this.FnTable.GetOverlayAutoCurveDistanceRangeInMeters(ulOverlayHandle, ref pfMinDistanceInMeters, ref pfMaxDistanceInMeters);
		}

		public EVROverlayError SetOverlayTextureColorSpace(ulong ulOverlayHandle, EColorSpace eTextureColorSpace)
		{
			return this.FnTable.SetOverlayTextureColorSpace(ulOverlayHandle, eTextureColorSpace);
		}

		public EVROverlayError GetOverlayTextureColorSpace(ulong ulOverlayHandle, ref EColorSpace peTextureColorSpace)
		{
			return this.FnTable.GetOverlayTextureColorSpace(ulOverlayHandle, ref peTextureColorSpace);
		}

		public EVROverlayError SetOverlayTextureBounds(ulong ulOverlayHandle, ref VRTextureBounds_t pOverlayTextureBounds)
		{
			return this.FnTable.SetOverlayTextureBounds(ulOverlayHandle, ref pOverlayTextureBounds);
		}

		public EVROverlayError GetOverlayTextureBounds(ulong ulOverlayHandle, ref VRTextureBounds_t pOverlayTextureBounds)
		{
			return this.FnTable.GetOverlayTextureBounds(ulOverlayHandle, ref pOverlayTextureBounds);
		}

		public EVROverlayError GetOverlayTransformType(ulong ulOverlayHandle, ref VROverlayTransformType peTransformType)
		{
			return this.FnTable.GetOverlayTransformType(ulOverlayHandle, ref peTransformType);
		}

		public EVROverlayError SetOverlayTransformAbsolute(ulong ulOverlayHandle, ETrackingUniverseOrigin eTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToOverlayTransform)
		{
			return this.FnTable.SetOverlayTransformAbsolute(ulOverlayHandle, eTrackingOrigin, ref pmatTrackingOriginToOverlayTransform);
		}

		public EVROverlayError GetOverlayTransformAbsolute(ulong ulOverlayHandle, ref ETrackingUniverseOrigin peTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToOverlayTransform)
		{
			return this.FnTable.GetOverlayTransformAbsolute(ulOverlayHandle, ref peTrackingOrigin, ref pmatTrackingOriginToOverlayTransform);
		}

		public EVROverlayError SetOverlayTransformTrackedDeviceRelative(ulong ulOverlayHandle, uint unTrackedDevice, ref HmdMatrix34_t pmatTrackedDeviceToOverlayTransform)
		{
			return this.FnTable.SetOverlayTransformTrackedDeviceRelative(ulOverlayHandle, unTrackedDevice, ref pmatTrackedDeviceToOverlayTransform);
		}

		public EVROverlayError GetOverlayTransformTrackedDeviceRelative(ulong ulOverlayHandle, ref uint punTrackedDevice, ref HmdMatrix34_t pmatTrackedDeviceToOverlayTransform)
		{
			punTrackedDevice = 0u;
			return this.FnTable.GetOverlayTransformTrackedDeviceRelative(ulOverlayHandle, ref punTrackedDevice, ref pmatTrackedDeviceToOverlayTransform);
		}

		public EVROverlayError SetOverlayTransformTrackedDeviceComponent(ulong ulOverlayHandle, uint unDeviceIndex, string pchComponentName)
		{
			return this.FnTable.SetOverlayTransformTrackedDeviceComponent(ulOverlayHandle, unDeviceIndex, pchComponentName);
		}

		public EVROverlayError GetOverlayTransformTrackedDeviceComponent(ulong ulOverlayHandle, ref uint punDeviceIndex, string pchComponentName, uint unComponentNameSize)
		{
			punDeviceIndex = 0u;
			return this.FnTable.GetOverlayTransformTrackedDeviceComponent(ulOverlayHandle, ref punDeviceIndex, pchComponentName, unComponentNameSize);
		}

		public EVROverlayError ShowOverlay(ulong ulOverlayHandle)
		{
			return this.FnTable.ShowOverlay(ulOverlayHandle);
		}

		public EVROverlayError HideOverlay(ulong ulOverlayHandle)
		{
			return this.FnTable.HideOverlay(ulOverlayHandle);
		}

		public bool IsOverlayVisible(ulong ulOverlayHandle)
		{
			return this.FnTable.IsOverlayVisible(ulOverlayHandle);
		}

		public EVROverlayError GetTransformForOverlayCoordinates(ulong ulOverlayHandle, ETrackingUniverseOrigin eTrackingOrigin, HmdVector2_t coordinatesInOverlay, ref HmdMatrix34_t pmatTransform)
		{
			return this.FnTable.GetTransformForOverlayCoordinates(ulOverlayHandle, eTrackingOrigin, coordinatesInOverlay, ref pmatTransform);
		}

		public bool PollNextOverlayEvent(ulong ulOverlayHandle, ref VREvent_t pEvent, uint uncbVREvent)
		{
			return this.FnTable.PollNextOverlayEvent(ulOverlayHandle, ref pEvent, uncbVREvent);
		}

		public EVROverlayError GetOverlayInputMethod(ulong ulOverlayHandle, ref VROverlayInputMethod peInputMethod)
		{
			return this.FnTable.GetOverlayInputMethod(ulOverlayHandle, ref peInputMethod);
		}

		public EVROverlayError SetOverlayInputMethod(ulong ulOverlayHandle, VROverlayInputMethod eInputMethod)
		{
			return this.FnTable.SetOverlayInputMethod(ulOverlayHandle, eInputMethod);
		}

		public EVROverlayError GetOverlayMouseScale(ulong ulOverlayHandle, ref HmdVector2_t pvecMouseScale)
		{
			return this.FnTable.GetOverlayMouseScale(ulOverlayHandle, ref pvecMouseScale);
		}

		public EVROverlayError SetOverlayMouseScale(ulong ulOverlayHandle, ref HmdVector2_t pvecMouseScale)
		{
			return this.FnTable.SetOverlayMouseScale(ulOverlayHandle, ref pvecMouseScale);
		}

		public bool ComputeOverlayIntersection(ulong ulOverlayHandle, ref VROverlayIntersectionParams_t pParams, ref VROverlayIntersectionResults_t pResults)
		{
			return this.FnTable.ComputeOverlayIntersection(ulOverlayHandle, ref pParams, ref pResults);
		}

		public bool HandleControllerOverlayInteractionAsMouse(ulong ulOverlayHandle, uint unControllerDeviceIndex)
		{
			return this.FnTable.HandleControllerOverlayInteractionAsMouse(ulOverlayHandle, unControllerDeviceIndex);
		}

		public bool IsHoverTargetOverlay(ulong ulOverlayHandle)
		{
			return this.FnTable.IsHoverTargetOverlay(ulOverlayHandle);
		}

		public ulong GetGamepadFocusOverlay()
		{
			return this.FnTable.GetGamepadFocusOverlay();
		}

		public EVROverlayError SetGamepadFocusOverlay(ulong ulNewFocusOverlay)
		{
			return this.FnTable.SetGamepadFocusOverlay(ulNewFocusOverlay);
		}

		public EVROverlayError SetOverlayNeighbor(EOverlayDirection eDirection, ulong ulFrom, ulong ulTo)
		{
			return this.FnTable.SetOverlayNeighbor(eDirection, ulFrom, ulTo);
		}

		public EVROverlayError MoveGamepadFocusToNeighbor(EOverlayDirection eDirection, ulong ulFrom)
		{
			return this.FnTable.MoveGamepadFocusToNeighbor(eDirection, ulFrom);
		}

		public EVROverlayError SetOverlayTexture(ulong ulOverlayHandle, ref Texture_t pTexture)
		{
			return this.FnTable.SetOverlayTexture(ulOverlayHandle, ref pTexture);
		}

		public EVROverlayError ClearOverlayTexture(ulong ulOverlayHandle)
		{
			return this.FnTable.ClearOverlayTexture(ulOverlayHandle);
		}

		public EVROverlayError SetOverlayRaw(ulong ulOverlayHandle, IntPtr pvBuffer, uint unWidth, uint unHeight, uint unDepth)
		{
			return this.FnTable.SetOverlayRaw(ulOverlayHandle, pvBuffer, unWidth, unHeight, unDepth);
		}

		public EVROverlayError SetOverlayFromFile(ulong ulOverlayHandle, string pchFilePath)
		{
			return this.FnTable.SetOverlayFromFile(ulOverlayHandle, pchFilePath);
		}

		public EVROverlayError CreateDashboardOverlay(string pchOverlayKey, string pchOverlayFriendlyName, ref ulong pMainHandle, ref ulong pThumbnailHandle)
		{
			pMainHandle = 0uL;
			pThumbnailHandle = 0uL;
			return this.FnTable.CreateDashboardOverlay(pchOverlayKey, pchOverlayFriendlyName, ref pMainHandle, ref pThumbnailHandle);
		}

		public bool IsDashboardVisible()
		{
			return this.FnTable.IsDashboardVisible();
		}

		public bool IsActiveDashboardOverlay(ulong ulOverlayHandle)
		{
			return this.FnTable.IsActiveDashboardOverlay(ulOverlayHandle);
		}

		public EVROverlayError SetDashboardOverlaySceneProcess(ulong ulOverlayHandle, uint unProcessId)
		{
			return this.FnTable.SetDashboardOverlaySceneProcess(ulOverlayHandle, unProcessId);
		}

		public EVROverlayError GetDashboardOverlaySceneProcess(ulong ulOverlayHandle, ref uint punProcessId)
		{
			punProcessId = 0u;
			return this.FnTable.GetDashboardOverlaySceneProcess(ulOverlayHandle, ref punProcessId);
		}

		public void ShowDashboard(string pchOverlayToShow)
		{
			this.FnTable.ShowDashboard(pchOverlayToShow);
		}

		public uint GetPrimaryDashboardDevice()
		{
			return this.FnTable.GetPrimaryDashboardDevice();
		}

		public EVROverlayError ShowKeyboard(int eInputMode, int eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText, bool bUseMinimalMode, ulong uUserValue)
		{
			return this.FnTable.ShowKeyboard(eInputMode, eLineInputMode, pchDescription, unCharMax, pchExistingText, bUseMinimalMode, uUserValue);
		}

		public EVROverlayError ShowKeyboardForOverlay(ulong ulOverlayHandle, int eInputMode, int eLineInputMode, string pchDescription, uint unCharMax, string pchExistingText, bool bUseMinimalMode, ulong uUserValue)
		{
			return this.FnTable.ShowKeyboardForOverlay(ulOverlayHandle, eInputMode, eLineInputMode, pchDescription, unCharMax, pchExistingText, bUseMinimalMode, uUserValue);
		}

		public uint GetKeyboardText(StringBuilder pchText, uint cchText)
		{
			return this.FnTable.GetKeyboardText(pchText, cchText);
		}

		public void HideKeyboard()
		{
			this.FnTable.HideKeyboard();
		}

		public void SetKeyboardTransformAbsolute(ETrackingUniverseOrigin eTrackingOrigin, ref HmdMatrix34_t pmatTrackingOriginToKeyboardTransform)
		{
			this.FnTable.SetKeyboardTransformAbsolute(eTrackingOrigin, ref pmatTrackingOriginToKeyboardTransform);
		}

		public void SetKeyboardPositionForOverlay(ulong ulOverlayHandle, HmdRect2_t avoidRect)
		{
			this.FnTable.SetKeyboardPositionForOverlay(ulOverlayHandle, avoidRect);
		}
	}
}
