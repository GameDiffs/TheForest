using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public struct IVRApplications
	{
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _AddApplicationManifest(string pchApplicationManifestFullPath, bool bTemporary);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _RemoveApplicationManifest(string pchApplicationManifestFullPath);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _IsApplicationInstalled(string pchAppKey);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetApplicationCount();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _GetApplicationKeyByIndex(uint unApplicationIndex, string pchAppKeyBuffer, uint unAppKeyBufferLen);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _GetApplicationKeyByProcessId(uint unProcessId, string pchAppKeyBuffer, uint unAppKeyBufferLen);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _LaunchApplication(string pchAppKey);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _LaunchDashboardOverlay(string pchAppKey);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _CancelApplicationLaunch(string pchAppKey);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _IdentifyApplication(uint unProcessId, string pchAppKey);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetApplicationProcessId(string pchAppKey);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr _GetApplicationsErrorNameFromEnum(EVRApplicationError error);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetApplicationPropertyString(string pchAppKey, EVRApplicationProperty eProperty, string pchPropertyValueBuffer, uint unPropertyValueBufferLen, ref EVRApplicationError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetApplicationPropertyBool(string pchAppKey, EVRApplicationProperty eProperty, ref EVRApplicationError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ulong _GetApplicationPropertyUint64(string pchAppKey, EVRApplicationProperty eProperty, ref EVRApplicationError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _SetApplicationAutoLaunch(string pchAppKey, bool bAutoLaunch);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetApplicationAutoLaunch(string pchAppKey);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _GetStartingApplication(string pchAppKeyBuffer, uint unAppKeyBufferLen);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationTransitionState _GetTransitionState();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _PerformApplicationPrelaunchCheck(string pchAppKey);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr _GetApplicationsTransitionStateNameFromEnum(EVRApplicationTransitionState state);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _IsQuitUserPromptRequested();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EVRApplicationError _LaunchInternalProcess(string pchBinaryPath, string pchArguments, string pchWorkingDirectory);

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._AddApplicationManifest AddApplicationManifest;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._RemoveApplicationManifest RemoveApplicationManifest;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._IsApplicationInstalled IsApplicationInstalled;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationCount GetApplicationCount;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationKeyByIndex GetApplicationKeyByIndex;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationKeyByProcessId GetApplicationKeyByProcessId;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._LaunchApplication LaunchApplication;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._LaunchDashboardOverlay LaunchDashboardOverlay;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._CancelApplicationLaunch CancelApplicationLaunch;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._IdentifyApplication IdentifyApplication;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationProcessId GetApplicationProcessId;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationsErrorNameFromEnum GetApplicationsErrorNameFromEnum;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationPropertyString GetApplicationPropertyString;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationPropertyBool GetApplicationPropertyBool;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationPropertyUint64 GetApplicationPropertyUint64;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._SetApplicationAutoLaunch SetApplicationAutoLaunch;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationAutoLaunch GetApplicationAutoLaunch;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetStartingApplication GetStartingApplication;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetTransitionState GetTransitionState;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._PerformApplicationPrelaunchCheck PerformApplicationPrelaunchCheck;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._GetApplicationsTransitionStateNameFromEnum GetApplicationsTransitionStateNameFromEnum;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._IsQuitUserPromptRequested IsQuitUserPromptRequested;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRApplications._LaunchInternalProcess LaunchInternalProcess;
	}
}
