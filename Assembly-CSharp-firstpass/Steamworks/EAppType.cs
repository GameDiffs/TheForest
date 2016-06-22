using System;

namespace Steamworks
{
	[Flags]
	public enum EAppType
	{
		k_EAppType_Invalid = 0,
		k_EAppType_Game = 1,
		k_EAppType_Application = 2,
		k_EAppType_Tool = 4,
		k_EAppType_Demo = 8,
		k_EAppType_Media_DEPRECATED = 16,
		k_EAppType_DLC = 32,
		k_EAppType_Guide = 64,
		k_EAppType_Driver = 128,
		k_EAppType_Config = 256,
		k_EAppType_Hardware = 512,
		k_EAppType_Video = 2048,
		k_EAppType_Plugin = 4096,
		k_EAppType_Music = 8192,
		k_EAppType_Shortcut = 1073741824,
		k_EAppType_DepotOnly = -2147483647
	}
}
