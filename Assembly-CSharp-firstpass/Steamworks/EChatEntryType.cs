using System;

namespace Steamworks
{
	public enum EChatEntryType
	{
		k_EChatEntryTypeInvalid,
		k_EChatEntryTypeChatMsg,
		k_EChatEntryTypeTyping,
		k_EChatEntryTypeInviteGame,
		k_EChatEntryTypeEmote,
		k_EChatEntryTypeLeftConversation = 6,
		k_EChatEntryTypeEntered,
		k_EChatEntryTypeWasKicked,
		k_EChatEntryTypeWasBanned,
		k_EChatEntryTypeDisconnected,
		k_EChatEntryTypeHistoricalChat,
		k_EChatEntryTypeReserved1,
		k_EChatEntryTypeReserved2,
		k_EChatEntryTypeLinkBlocked
	}
}
