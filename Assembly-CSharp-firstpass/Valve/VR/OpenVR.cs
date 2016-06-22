using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public class OpenVR
	{
		private class COpenVRContext
		{
			private CVRSystem m_pVRSystem;

			private CVRChaperone m_pVRChaperone;

			private CVRChaperoneSetup m_pVRChaperoneSetup;

			private CVRCompositor m_pVRCompositor;

			private CVROverlay m_pVROverlay;

			private CVRRenderModels m_pVRRenderModels;

			private CVRExtendedDisplay m_pVRExtendedDisplay;

			private CVRSettings m_pVRSettings;

			private CVRApplications m_pVRApplications;

			public COpenVRContext()
			{
				this.Clear();
			}

			public void Clear()
			{
				this.m_pVRSystem = null;
				this.m_pVRChaperone = null;
				this.m_pVRChaperoneSetup = null;
				this.m_pVRCompositor = null;
				this.m_pVROverlay = null;
				this.m_pVRRenderModels = null;
				this.m_pVRExtendedDisplay = null;
				this.m_pVRSettings = null;
				this.m_pVRApplications = null;
			}

			private void CheckClear()
			{
				if (OpenVR.VRToken != OpenVR.GetInitToken())
				{
					this.Clear();
					OpenVR.VRToken = OpenVR.GetInitToken();
				}
			}

			public CVRSystem VRSystem()
			{
				this.CheckClear();
				if (this.m_pVRSystem == null)
				{
					EVRInitError eVRInitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRSystem_011", ref eVRInitError);
					if (genericInterface != IntPtr.Zero && eVRInitError == EVRInitError.None)
					{
						this.m_pVRSystem = new CVRSystem(genericInterface);
					}
				}
				return this.m_pVRSystem;
			}

			public CVRChaperone VRChaperone()
			{
				this.CheckClear();
				if (this.m_pVRChaperone == null)
				{
					EVRInitError eVRInitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRChaperone_003", ref eVRInitError);
					if (genericInterface != IntPtr.Zero && eVRInitError == EVRInitError.None)
					{
						this.m_pVRChaperone = new CVRChaperone(genericInterface);
					}
				}
				return this.m_pVRChaperone;
			}

			public CVRChaperoneSetup VRChaperoneSetup()
			{
				this.CheckClear();
				if (this.m_pVRChaperoneSetup == null)
				{
					EVRInitError eVRInitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRChaperoneSetup_005", ref eVRInitError);
					if (genericInterface != IntPtr.Zero && eVRInitError == EVRInitError.None)
					{
						this.m_pVRChaperoneSetup = new CVRChaperoneSetup(genericInterface);
					}
				}
				return this.m_pVRChaperoneSetup;
			}

			public CVRCompositor VRCompositor()
			{
				this.CheckClear();
				if (this.m_pVRCompositor == null)
				{
					EVRInitError eVRInitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRCompositor_013", ref eVRInitError);
					if (genericInterface != IntPtr.Zero && eVRInitError == EVRInitError.None)
					{
						this.m_pVRCompositor = new CVRCompositor(genericInterface);
					}
				}
				return this.m_pVRCompositor;
			}

			public CVROverlay VROverlay()
			{
				this.CheckClear();
				if (this.m_pVROverlay == null)
				{
					EVRInitError eVRInitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVROverlay_010", ref eVRInitError);
					if (genericInterface != IntPtr.Zero && eVRInitError == EVRInitError.None)
					{
						this.m_pVROverlay = new CVROverlay(genericInterface);
					}
				}
				return this.m_pVROverlay;
			}

			public CVRRenderModels VRRenderModels()
			{
				this.CheckClear();
				if (this.m_pVRRenderModels == null)
				{
					EVRInitError eVRInitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRRenderModels_005", ref eVRInitError);
					if (genericInterface != IntPtr.Zero && eVRInitError == EVRInitError.None)
					{
						this.m_pVRRenderModels = new CVRRenderModels(genericInterface);
					}
				}
				return this.m_pVRRenderModels;
			}

			public CVRExtendedDisplay VRExtendedDisplay()
			{
				this.CheckClear();
				if (this.m_pVRExtendedDisplay == null)
				{
					EVRInitError eVRInitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRExtendedDisplay_001", ref eVRInitError);
					if (genericInterface != IntPtr.Zero && eVRInitError == EVRInitError.None)
					{
						this.m_pVRExtendedDisplay = new CVRExtendedDisplay(genericInterface);
					}
				}
				return this.m_pVRExtendedDisplay;
			}

			public CVRSettings VRSettings()
			{
				this.CheckClear();
				if (this.m_pVRSettings == null)
				{
					EVRInitError eVRInitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRSettings_001", ref eVRInitError);
					if (genericInterface != IntPtr.Zero && eVRInitError == EVRInitError.None)
					{
						this.m_pVRSettings = new CVRSettings(genericInterface);
					}
				}
				return this.m_pVRSettings;
			}

			public CVRApplications VRApplications()
			{
				this.CheckClear();
				if (this.m_pVRApplications == null)
				{
					EVRInitError eVRInitError = EVRInitError.None;
					IntPtr genericInterface = OpenVRInterop.GetGenericInterface("FnTable:IVRApplications_004", ref eVRInitError);
					if (genericInterface != IntPtr.Zero && eVRInitError == EVRInitError.None)
					{
						this.m_pVRApplications = new CVRApplications(genericInterface);
					}
				}
				return this.m_pVRApplications;
			}
		}

		public const uint k_unTrackingStringSize = 32u;

		public const uint k_unMaxDriverDebugResponseSize = 32768u;

		public const uint k_unTrackedDeviceIndex_Hmd = 0u;

		public const uint k_unMaxTrackedDeviceCount = 16u;

		public const uint k_unTrackedDeviceIndexInvalid = 4294967295u;

		public const uint k_unMaxPropertyStringSize = 32768u;

		public const uint k_unControllerStateAxisCount = 5u;

		public const ulong k_ulOverlayHandleInvalid = 0uL;

		public const string IVRSystem_Version = "IVRSystem_011";

		public const string IVRExtendedDisplay_Version = "IVRExtendedDisplay_001";

		public const uint k_unMaxApplicationKeyLength = 128u;

		public const string IVRApplications_Version = "IVRApplications_004";

		public const string IVRChaperone_Version = "IVRChaperone_003";

		public const string IVRChaperoneSetup_Version = "IVRChaperoneSetup_005";

		public const string IVRCompositor_Version = "IVRCompositor_013";

		public const uint k_unVROverlayMaxKeyLength = 128u;

		public const uint k_unVROverlayMaxNameLength = 128u;

		public const uint k_unMaxOverlayCount = 32u;

		public const string IVROverlay_Version = "IVROverlay_010";

		public const string k_pch_Controller_Component_GDC2015 = "gdc2015";

		public const string k_pch_Controller_Component_Base = "base";

		public const string k_pch_Controller_Component_Tip = "tip";

		public const string k_pch_Controller_Component_HandGrip = "handgrip";

		public const string k_pch_Controller_Component_Status = "status";

		public const string IVRRenderModels_Version = "IVRRenderModels_005";

		public const uint k_unNotificationTextMaxSize = 256u;

		public const string IVRNotifications_Version = "IVRNotifications_002";

		public const uint k_unMaxSettingsKeyLength = 128u;

		public const string k_pch_SteamVR_Section = "steamvr";

		public const string k_pch_SteamVR_RequireHmd_String = "requireHmd";

		public const string k_pch_SteamVR_ForcedDriverKey_String = "forcedDriver";

		public const string k_pch_SteamVR_ForcedHmdKey_String = "forcedHmd";

		public const string k_pch_SteamVR_DisplayDebug_Bool = "displayDebug";

		public const string k_pch_SteamVR_DebugProcessPipe_String = "debugProcessPipe";

		public const string k_pch_SteamVR_EnableDistortion_Bool = "enableDistortion";

		public const string k_pch_SteamVR_DisplayDebugX_Int32 = "displayDebugX";

		public const string k_pch_SteamVR_DisplayDebugY_Int32 = "displayDebugY";

		public const string k_pch_SteamVR_SendSystemButtonToAllApps_Bool = "sendSystemButtonToAllApps";

		public const string k_pch_SteamVR_LogLevel_Int32 = "loglevel";

		public const string k_pch_SteamVR_IPD_Float = "ipd";

		public const string k_pch_SteamVR_Background_String = "background";

		public const string k_pch_SteamVR_GridColor_String = "gridColor";

		public const string k_pch_SteamVR_PlayAreaColor_String = "playAreaColor";

		public const string k_pch_SteamVR_ActivateMultipleDrivers_Bool = "activateMultipleDrivers";

		public const string k_pch_SteamVR_PowerOffOnExit_Bool = "powerOffOnExit";

		public const string k_pch_SteamVR_StandbyAppRunningTimeout_Float = "standbyAppRunningTimeout";

		public const string k_pch_SteamVR_StandbyNoAppTimeout_Float = "standbyNoAppTimeout";

		public const string k_pch_SteamVR_DirectMode_Bool = "directMode";

		public const string k_pch_SteamVR_DirectModeEdidVid_Int32 = "directModeEdidVid";

		public const string k_pch_SteamVR_DirectModeEdidPid_Int32 = "directModeEdidPid";

		public const string k_pch_SteamVR_UsingSpeakers_Bool = "usingSpeakers";

		public const string k_pch_SteamVR_SpeakersForwardYawOffsetDegrees_Float = "speakersForwardYawOffsetDegrees";

		public const string k_pch_SteamVR_BaseStationPowerManagement_Bool = "basestationPowerManagement";

		public const string k_pch_Lighthouse_Section = "driver_lighthouse";

		public const string k_pch_Lighthouse_DisableIMU_Bool = "disableimu";

		public const string k_pch_Lighthouse_UseDisambiguation_String = "usedisambiguation";

		public const string k_pch_Lighthouse_DisambiguationDebug_Int32 = "disambiguationdebug";

		public const string k_pch_Lighthouse_PrimaryBasestation_Int32 = "primarybasestation";

		public const string k_pch_Lighthouse_LighthouseName_String = "lighthousename";

		public const string k_pch_Lighthouse_MaxIncidenceAngleDegrees_Float = "maxincidenceangledegrees";

		public const string k_pch_Lighthouse_UseLighthouseDirect_Bool = "uselighthousedirect";

		public const string k_pch_Lighthouse_DBHistory_Bool = "dbhistory";

		public const string k_pch_Lighthouse_OriginOffsetX_Float = "originoffsetx";

		public const string k_pch_Lighthouse_OriginOffsetY_Float = "originoffsety";

		public const string k_pch_Lighthouse_OriginOffsetZ_Float = "originoffsetz";

		public const string k_pch_Lighthouse_HeadingOffset_Float = "headingoffset";

		public const string k_pch_Null_Section = "driver_null";

		public const string k_pch_Null_EnableNullDriver_Bool = "enable";

		public const string k_pch_Null_SerialNumber_String = "serialNumber";

		public const string k_pch_Null_ModelNumber_String = "modelNumber";

		public const string k_pch_Null_WindowX_Int32 = "windowX";

		public const string k_pch_Null_WindowY_Int32 = "windowY";

		public const string k_pch_Null_WindowWidth_Int32 = "windowWidth";

		public const string k_pch_Null_WindowHeight_Int32 = "windowHeight";

		public const string k_pch_Null_RenderWidth_Int32 = "renderWidth";

		public const string k_pch_Null_RenderHeight_Int32 = "renderHeight";

		public const string k_pch_Null_SecondsFromVsyncToPhotons_Float = "secondsFromVsyncToPhotons";

		public const string k_pch_Null_DisplayFrequency_Float = "displayFrequency";

		public const string k_pch_UserInterface_Section = "userinterface";

		public const string k_pch_UserInterface_StatusAlwaysOnTop_Bool = "StatusAlwaysOnTop";

		public const string k_pch_Notifications_Section = "notifications";

		public const string k_pch_Notifications_DoNotDisturb_Bool = "DoNotDisturb";

		public const string k_pch_Keyboard_Section = "keyboard";

		public const string k_pch_Keyboard_TutorialCompletions = "TutorialCompletions";

		public const string k_pch_Perf_Section = "perfcheck";

		public const string k_pch_Perf_HeuristicActive_Bool = "heuristicActive";

		public const string k_pch_Perf_NotifyInHMD_Bool = "warnInHMD";

		public const string k_pch_Perf_NotifyOnlyOnce_Bool = "warnOnlyOnce";

		public const string k_pch_Perf_AllowTimingStore_Bool = "allowTimingStore";

		public const string k_pch_Perf_SaveTimingsOnExit_Bool = "saveTimingsOnExit";

		public const string k_pch_Perf_TestData_Float = "perfTestData";

		public const string k_pch_Camera_Section = "camera";

		public const string IVRSettings_Version = "IVRSettings_001";

		public const string k_pch_audio_Section = "audio";

		public const string k_pch_audio_OnPlaybackDevice_String = "onPlaybackDevice";

		public const string k_pch_audio_OnRecordDevice_String = "onRecordDevice";

		public const string k_pch_audio_OffPlaybackDevice_String = "offPlaybackDevice";

		public const string k_pch_audio_OffRecordDevice_String = "offRecordDevice";

		public const string k_pch_audio_VIVEHDMIGain = "viveHDMIGain";

		public const string IVRTrackedCamera_Version = "IVRTrackedCamera_001";

		private const string FnTable_Prefix = "FnTable:";

		private static OpenVR.COpenVRContext _OpenVRInternal_ModuleContext;

		private static uint VRToken
		{
			get;
			set;
		}

		private static OpenVR.COpenVRContext OpenVRInternal_ModuleContext
		{
			get
			{
				if (OpenVR._OpenVRInternal_ModuleContext == null)
				{
					OpenVR._OpenVRInternal_ModuleContext = new OpenVR.COpenVRContext();
				}
				return OpenVR._OpenVRInternal_ModuleContext;
			}
		}

		public static CVRSystem System
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRSystem();
			}
		}

		public static CVRChaperone Chaperone
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRChaperone();
			}
		}

		public static CVRChaperoneSetup ChaperoneSetup
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRChaperoneSetup();
			}
		}

		public static CVRCompositor Compositor
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRCompositor();
			}
		}

		public static CVROverlay Overlay
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VROverlay();
			}
		}

		public static CVRRenderModels RenderModels
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRRenderModels();
			}
		}

		public static CVRApplications Applications
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRApplications();
			}
		}

		public static CVRSettings Settings
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRSettings();
			}
		}

		public static CVRExtendedDisplay ExtendedDisplay
		{
			get
			{
				return OpenVR.OpenVRInternal_ModuleContext.VRExtendedDisplay();
			}
		}

		public static uint InitInternal(ref EVRInitError peError, EVRApplicationType eApplicationType)
		{
			return OpenVRInterop.InitInternal(ref peError, eApplicationType);
		}

		public static void ShutdownInternal()
		{
			OpenVRInterop.ShutdownInternal();
		}

		public static bool IsHmdPresent()
		{
			return OpenVRInterop.IsHmdPresent();
		}

		public static bool IsRuntimeInstalled()
		{
			return OpenVRInterop.IsRuntimeInstalled();
		}

		public static string GetStringForHmdError(EVRInitError error)
		{
			return Marshal.PtrToStringAnsi(OpenVRInterop.GetStringForHmdError(error));
		}

		public static IntPtr GetGenericInterface(string pchInterfaceVersion, ref EVRInitError peError)
		{
			return OpenVRInterop.GetGenericInterface(pchInterfaceVersion, ref peError);
		}

		public static bool IsInterfaceVersionValid(string pchInterfaceVersion)
		{
			return OpenVRInterop.IsInterfaceVersionValid(pchInterfaceVersion);
		}

		public static uint GetInitToken()
		{
			return OpenVRInterop.GetInitToken();
		}

		public static CVRSystem Init(ref EVRInitError peError, EVRApplicationType eApplicationType = EVRApplicationType.VRApplication_Scene)
		{
			OpenVR.VRToken = OpenVR.InitInternal(ref peError, eApplicationType);
			OpenVR.OpenVRInternal_ModuleContext.Clear();
			if (peError != EVRInitError.None)
			{
				return null;
			}
			if (!OpenVR.IsInterfaceVersionValid("IVRSystem_011"))
			{
				OpenVR.ShutdownInternal();
				peError = EVRInitError.Init_InterfaceNotFound;
				return null;
			}
			return OpenVR.System;
		}

		public static void Shutdown()
		{
			OpenVR.ShutdownInternal();
		}
	}
}
