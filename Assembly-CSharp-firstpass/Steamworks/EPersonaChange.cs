using System;

namespace Steamworks
{
	[Flags]
	public enum EPersonaChange
	{
		k_EPersonaChangeName = 1,
		k_EPersonaChangeStatus = 2,
		k_EPersonaChangeComeOnline = 4,
		k_EPersonaChangeGoneOffline = 8,
		k_EPersonaChangeGamePlayed = 16,
		k_EPersonaChangeGameServer = 32,
		k_EPersonaChangeAvatar = 64,
		k_EPersonaChangeJoinedSource = 128,
		k_EPersonaChangeLeftSource = 256,
		k_EPersonaChangeRelationshipChanged = 512,
		k_EPersonaChangeNameFirstSet = 1024,
		k_EPersonaChangeFacebookInfo = 2048,
		k_EPersonaChangeNickname = 4096,
		k_EPersonaChangeSteamLevel = 8192
	}
}
