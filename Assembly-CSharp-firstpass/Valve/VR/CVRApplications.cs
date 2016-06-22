using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public class CVRApplications
	{
		private IVRApplications FnTable;

		internal CVRApplications(IntPtr pInterface)
		{
			this.FnTable = (IVRApplications)Marshal.PtrToStructure(pInterface, typeof(IVRApplications));
		}

		public EVRApplicationError AddApplicationManifest(string pchApplicationManifestFullPath, bool bTemporary)
		{
			return this.FnTable.AddApplicationManifest(pchApplicationManifestFullPath, bTemporary);
		}

		public EVRApplicationError RemoveApplicationManifest(string pchApplicationManifestFullPath)
		{
			return this.FnTable.RemoveApplicationManifest(pchApplicationManifestFullPath);
		}

		public bool IsApplicationInstalled(string pchAppKey)
		{
			return this.FnTable.IsApplicationInstalled(pchAppKey);
		}

		public uint GetApplicationCount()
		{
			return this.FnTable.GetApplicationCount();
		}

		public EVRApplicationError GetApplicationKeyByIndex(uint unApplicationIndex, string pchAppKeyBuffer, uint unAppKeyBufferLen)
		{
			return this.FnTable.GetApplicationKeyByIndex(unApplicationIndex, pchAppKeyBuffer, unAppKeyBufferLen);
		}

		public EVRApplicationError GetApplicationKeyByProcessId(uint unProcessId, string pchAppKeyBuffer, uint unAppKeyBufferLen)
		{
			return this.FnTable.GetApplicationKeyByProcessId(unProcessId, pchAppKeyBuffer, unAppKeyBufferLen);
		}

		public EVRApplicationError LaunchApplication(string pchAppKey)
		{
			return this.FnTable.LaunchApplication(pchAppKey);
		}

		public EVRApplicationError LaunchDashboardOverlay(string pchAppKey)
		{
			return this.FnTable.LaunchDashboardOverlay(pchAppKey);
		}

		public bool CancelApplicationLaunch(string pchAppKey)
		{
			return this.FnTable.CancelApplicationLaunch(pchAppKey);
		}

		public EVRApplicationError IdentifyApplication(uint unProcessId, string pchAppKey)
		{
			return this.FnTable.IdentifyApplication(unProcessId, pchAppKey);
		}

		public uint GetApplicationProcessId(string pchAppKey)
		{
			return this.FnTable.GetApplicationProcessId(pchAppKey);
		}

		public string GetApplicationsErrorNameFromEnum(EVRApplicationError error)
		{
			IntPtr ptr = this.FnTable.GetApplicationsErrorNameFromEnum(error);
			return (string)Marshal.PtrToStructure(ptr, typeof(string));
		}

		public uint GetApplicationPropertyString(string pchAppKey, EVRApplicationProperty eProperty, string pchPropertyValueBuffer, uint unPropertyValueBufferLen, ref EVRApplicationError peError)
		{
			return this.FnTable.GetApplicationPropertyString(pchAppKey, eProperty, pchPropertyValueBuffer, unPropertyValueBufferLen, ref peError);
		}

		public bool GetApplicationPropertyBool(string pchAppKey, EVRApplicationProperty eProperty, ref EVRApplicationError peError)
		{
			return this.FnTable.GetApplicationPropertyBool(pchAppKey, eProperty, ref peError);
		}

		public ulong GetApplicationPropertyUint64(string pchAppKey, EVRApplicationProperty eProperty, ref EVRApplicationError peError)
		{
			return this.FnTable.GetApplicationPropertyUint64(pchAppKey, eProperty, ref peError);
		}

		public EVRApplicationError SetApplicationAutoLaunch(string pchAppKey, bool bAutoLaunch)
		{
			return this.FnTable.SetApplicationAutoLaunch(pchAppKey, bAutoLaunch);
		}

		public bool GetApplicationAutoLaunch(string pchAppKey)
		{
			return this.FnTable.GetApplicationAutoLaunch(pchAppKey);
		}

		public EVRApplicationError GetStartingApplication(string pchAppKeyBuffer, uint unAppKeyBufferLen)
		{
			return this.FnTable.GetStartingApplication(pchAppKeyBuffer, unAppKeyBufferLen);
		}

		public EVRApplicationTransitionState GetTransitionState()
		{
			return this.FnTable.GetTransitionState();
		}

		public EVRApplicationError PerformApplicationPrelaunchCheck(string pchAppKey)
		{
			return this.FnTable.PerformApplicationPrelaunchCheck(pchAppKey);
		}

		public string GetApplicationsTransitionStateNameFromEnum(EVRApplicationTransitionState state)
		{
			IntPtr ptr = this.FnTable.GetApplicationsTransitionStateNameFromEnum(state);
			return (string)Marshal.PtrToStructure(ptr, typeof(string));
		}

		public bool IsQuitUserPromptRequested()
		{
			return this.FnTable.IsQuitUserPromptRequested();
		}

		public EVRApplicationError LaunchInternalProcess(string pchBinaryPath, string pchArguments, string pchWorkingDirectory)
		{
			return this.FnTable.LaunchInternalProcess(pchBinaryPath, pchArguments, pchWorkingDirectory);
		}
	}
}
