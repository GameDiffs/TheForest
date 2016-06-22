using System;

namespace Steamworks
{
	public static class SteamUnifiedMessages
	{
		public static ClientUnifiedMessageHandle SendMethod(string pchServiceMethod, byte[] pRequestBuffer, uint unRequestBufferSize, ulong unContext)
		{
			InteropHelp.TestIfAvailableClient();
			ClientUnifiedMessageHandle result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchServiceMethod))
			{
				result = (ClientUnifiedMessageHandle)NativeMethods.ISteamUnifiedMessages_SendMethod(uTF8StringHandle, pRequestBuffer, unRequestBufferSize, unContext);
			}
			return result;
		}

		public static bool GetMethodResponseInfo(ClientUnifiedMessageHandle hHandle, out uint punResponseSize, out EResult peResult)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUnifiedMessages_GetMethodResponseInfo(hHandle, out punResponseSize, out peResult);
		}

		public static bool GetMethodResponseData(ClientUnifiedMessageHandle hHandle, byte[] pResponseBuffer, uint unResponseBufferSize, bool bAutoRelease)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUnifiedMessages_GetMethodResponseData(hHandle, pResponseBuffer, unResponseBufferSize, bAutoRelease);
		}

		public static bool ReleaseMethod(ClientUnifiedMessageHandle hHandle)
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamUnifiedMessages_ReleaseMethod(hHandle);
		}

		public static bool SendNotification(string pchServiceNotification, byte[] pNotificationBuffer, uint unNotificationBufferSize)
		{
			InteropHelp.TestIfAvailableClient();
			bool result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchServiceNotification))
			{
				result = NativeMethods.ISteamUnifiedMessages_SendNotification(uTF8StringHandle, pNotificationBuffer, unNotificationBufferSize);
			}
			return result;
		}
	}
}
