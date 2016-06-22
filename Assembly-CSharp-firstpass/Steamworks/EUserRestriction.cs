using System;

namespace Steamworks
{
	public enum EUserRestriction
	{
		k_nUserRestrictionNone,
		k_nUserRestrictionUnknown,
		k_nUserRestrictionAnyChat,
		k_nUserRestrictionVoiceChat = 4,
		k_nUserRestrictionGroupChat = 8,
		k_nUserRestrictionRating = 16,
		k_nUserRestrictionGameInvites = 32,
		k_nUserRestrictionTrading = 64
	}
}
