using System;

namespace Steamworks
{
	public static class SteamScreenshots
	{
		public static ScreenshotHandle WriteScreenshot(byte[] pubRGB, uint cubRGB, int nWidth, int nHeight)
		{
			InteropHelp.TestIfAvailableClient();
			return (ScreenshotHandle)NativeMethods.ISteamScreenshots_WriteScreenshot(pubRGB, cubRGB, nWidth, nHeight);
		}

		public static ScreenshotHandle AddScreenshotToLibrary(string pchFilename, string pchThumbnailFilename, int nWidth, int nHeight)
		{
			InteropHelp.TestIfAvailableClient();
			ScreenshotHandle result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchFilename))
			{
				using (InteropHelp.UTF8StringHandle uTF8StringHandle2 = new InteropHelp.UTF8StringHandle(pchThumbnailFilename))
				{
					result = (ScreenshotHandle)NativeMethods.ISteamScreenshots_AddScreenshotToLibrary(uTF8StringHandle, uTF8StringHandle2, nWidth, nHeight);
				}
			}
			return result;
		}

		public static void TriggerScreenshot()
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamScreenshots_TriggerScreenshot();
		}

		public static void HookScreenshots(bool bHook)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamScreenshots_HookScreenshots(bHook);
		}

		public static bool SetLocation(ScreenshotHandle hScreenshot, string pchLocation)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchLocation))
			{
				result = NativeMethods.ISteamScreenshots_SetLocation(hScreenshot, uTF8StringHandle);
			}
			return result;
		}

		public static bool TagUser(ScreenshotHandle hScreenshot, CSteamID steamID)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamScreenshots_TagUser(hScreenshot, steamID);
		}

		public static bool TagPublishedFile(ScreenshotHandle hScreenshot, PublishedFileId_t unPublishedFileID)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamScreenshots_TagPublishedFile(hScreenshot, unPublishedFileID);
		}
	}
}
