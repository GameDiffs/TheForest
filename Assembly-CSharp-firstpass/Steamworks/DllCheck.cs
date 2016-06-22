using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks
{
	public class DllCheck
	{
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern int GetModuleFileName(IntPtr hModule, StringBuilder strFullPath, int nSize);

		public static bool Test()
		{
			return true;
		}

		private static bool CheckSteamAPIDLL()
		{
			string lpModuleName;
			int num;
			if (IntPtr.Size == 4)
			{
				lpModuleName = "steam_api.dll";
				num = 191168;
			}
			else
			{
				lpModuleName = "steam_api64.dll";
				num = 211368;
			}
			IntPtr moduleHandle = DllCheck.GetModuleHandle(lpModuleName);
			if (moduleHandle == IntPtr.Zero)
			{
				return true;
			}
			StringBuilder stringBuilder = new StringBuilder(256);
			DllCheck.GetModuleFileName(moduleHandle, stringBuilder, stringBuilder.Capacity);
			string text = stringBuilder.ToString();
			if (File.Exists(text))
			{
				FileInfo fileInfo = new FileInfo(text);
				if (fileInfo.Length != (long)num)
				{
					return false;
				}
				if (FileVersionInfo.GetVersionInfo(text).FileVersion != "02.71.29.02")
				{
					return false;
				}
			}
			return true;
		}
	}
}
