using System;

namespace Steamworks
{
	[Flags]
	public enum EFriendFlags
	{
		k_EFriendFlagNone = 0,
		k_EFriendFlagBlocked = 1,
		k_EFriendFlagFriendshipRequested = 2,
		k_EFriendFlagImmediate = 4,
		k_EFriendFlagClanMember = 8,
		k_EFriendFlagOnGameServer = 16,
		k_EFriendFlagRequestingFriendship = 128,
		k_EFriendFlagRequestingInfo = 256,
		k_EFriendFlagIgnored = 512,
		k_EFriendFlagIgnoredFriend = 1024,
		k_EFriendFlagSuggested = 2048,
		k_EFriendFlagAll = 65535
	}
}
