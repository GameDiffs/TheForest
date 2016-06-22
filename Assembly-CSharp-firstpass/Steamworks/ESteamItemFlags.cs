using System;

namespace Steamworks
{
	[Flags]
	public enum ESteamItemFlags
	{
		k_ESteamItemNoTrade = 1,
		k_ESteamItemRemoved = 256,
		k_ESteamItemConsumed = 512
	}
}
