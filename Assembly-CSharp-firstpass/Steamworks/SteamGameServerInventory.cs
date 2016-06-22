using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	public static class SteamGameServerInventory
	{
		public static EResult GetResultStatus(SteamInventoryResult_t resultHandle)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_GetResultStatus(resultHandle);
		}

		public static bool GetResultItems(SteamInventoryResult_t resultHandle, SteamItemDetails_t[] pOutItemsArray, ref uint punOutItemsArraySize)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_GetResultItems(resultHandle, pOutItemsArray, ref punOutItemsArraySize);
		}

		public static uint GetResultTimestamp(SteamInventoryResult_t resultHandle)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_GetResultTimestamp(resultHandle);
		}

		public static bool CheckResultSteamID(SteamInventoryResult_t resultHandle, CSteamID steamIDExpected)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_CheckResultSteamID(resultHandle, steamIDExpected);
		}

		public static void DestroyResult(SteamInventoryResult_t resultHandle)
		{
			InteropHelp.TestIfAvailableGameServer();
			NativeMethods.ISteamGameServerInventory_DestroyResult(resultHandle);
		}

		public static bool GetAllItems(out SteamInventoryResult_t pResultHandle)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_GetAllItems(out pResultHandle);
		}

		public static bool GetItemsByID(out SteamInventoryResult_t pResultHandle, SteamItemInstanceID_t[] pInstanceIDs, uint unCountInstanceIDs)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_GetItemsByID(out pResultHandle, pInstanceIDs, unCountInstanceIDs);
		}

		public static bool SerializeResult(SteamInventoryResult_t resultHandle, byte[] pOutBuffer, out uint punOutBufferSize)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_SerializeResult(resultHandle, pOutBuffer, out punOutBufferSize);
		}

		public static bool DeserializeResult(out SteamInventoryResult_t pOutResultHandle, byte[] pBuffer, uint unBufferSize, bool bRESERVED_MUST_BE_FALSE = false)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_DeserializeResult(out pOutResultHandle, pBuffer, unBufferSize, bRESERVED_MUST_BE_FALSE);
		}

		public static bool GenerateItems(out SteamInventoryResult_t pResultHandle, SteamItemDef_t[] pArrayItemDefs, uint[] punArrayQuantity, uint unArrayLength)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_GenerateItems(out pResultHandle, pArrayItemDefs, punArrayQuantity, unArrayLength);
		}

		public static bool GrantPromoItems(out SteamInventoryResult_t pResultHandle)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_GrantPromoItems(out pResultHandle);
		}

		public static bool AddPromoItem(out SteamInventoryResult_t pResultHandle, SteamItemDef_t itemDef)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_AddPromoItem(out pResultHandle, itemDef);
		}

		public static bool AddPromoItems(out SteamInventoryResult_t pResultHandle, SteamItemDef_t[] pArrayItemDefs, uint unArrayLength)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_AddPromoItems(out pResultHandle, pArrayItemDefs, unArrayLength);
		}

		public static bool ConsumeItem(out SteamInventoryResult_t pResultHandle, SteamItemInstanceID_t itemConsume, uint unQuantity)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_ConsumeItem(out pResultHandle, itemConsume, unQuantity);
		}

		public static bool ExchangeItems(out SteamInventoryResult_t pResultHandle, SteamItemDef_t[] pArrayGenerate, uint[] punArrayGenerateQuantity, uint unArrayGenerateLength, SteamItemInstanceID_t[] pArrayDestroy, uint[] punArrayDestroyQuantity, uint unArrayDestroyLength)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_ExchangeItems(out pResultHandle, pArrayGenerate, punArrayGenerateQuantity, unArrayGenerateLength, pArrayDestroy, punArrayDestroyQuantity, unArrayDestroyLength);
		}

		public static bool TransferItemQuantity(out SteamInventoryResult_t pResultHandle, SteamItemInstanceID_t itemIdSource, uint unQuantity, SteamItemInstanceID_t itemIdDest)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_TransferItemQuantity(out pResultHandle, itemIdSource, unQuantity, itemIdDest);
		}

		public static void SendItemDropHeartbeat()
		{
			InteropHelp.TestIfAvailableGameServer();
			NativeMethods.ISteamGameServerInventory_SendItemDropHeartbeat();
		}

		public static bool TriggerItemDrop(out SteamInventoryResult_t pResultHandle, SteamItemDef_t dropListDefinition)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_TriggerItemDrop(out pResultHandle, dropListDefinition);
		}

		public static bool TradeItems(out SteamInventoryResult_t pResultHandle, CSteamID steamIDTradePartner, SteamItemInstanceID_t[] pArrayGive, uint[] pArrayGiveQuantity, uint nArrayGiveLength, SteamItemInstanceID_t[] pArrayGet, uint[] pArrayGetQuantity, uint nArrayGetLength)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_TradeItems(out pResultHandle, steamIDTradePartner, pArrayGive, pArrayGiveQuantity, nArrayGiveLength, pArrayGet, pArrayGetQuantity, nArrayGetLength);
		}

		public static bool LoadItemDefinitions()
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_LoadItemDefinitions();
		}

		public static bool GetItemDefinitionIDs(SteamItemDef_t[] pItemDefIDs, out uint punItemDefIDsArraySize)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamGameServerInventory_GetItemDefinitionIDs(pItemDefIDs, out punItemDefIDsArraySize);
		}

		public static bool GetItemDefinitionProperty(SteamItemDef_t iDefinition, string pchPropertyName, out string pchValueBuffer, ref uint punValueBufferSize)
		{
			InteropHelp.TestIfAvailableGameServer();
			IntPtr intPtr = Marshal.AllocHGlobal((int)punValueBufferSize);
			bool result;
			using (InteropHelp.UTF8StringHandle uTF8StringHandle = new InteropHelp.UTF8StringHandle(pchPropertyName))
			{
				bool flag = NativeMethods.ISteamGameServerInventory_GetItemDefinitionProperty(iDefinition, uTF8StringHandle, intPtr, ref punValueBufferSize);
				pchValueBuffer = ((!flag) ? null : InteropHelp.PtrToStringUTF8(intPtr));
				Marshal.FreeHGlobal(intPtr);
				result = flag;
			}
			return result;
		}
	}
}
