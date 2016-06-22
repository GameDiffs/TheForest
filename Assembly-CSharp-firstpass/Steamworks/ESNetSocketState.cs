using System;

namespace Steamworks
{
	public enum ESNetSocketState
	{
		k_ESNetSocketStateInvalid,
		k_ESNetSocketStateConnected,
		k_ESNetSocketStateInitiated = 10,
		k_ESNetSocketStateLocalCandidatesFound,
		k_ESNetSocketStateReceivedRemoteCandidates,
		k_ESNetSocketStateChallengeHandshake = 15,
		k_ESNetSocketStateDisconnecting = 21,
		k_ESNetSocketStateLocalDisconnect,
		k_ESNetSocketStateTimeoutDuringConnect,
		k_ESNetSocketStateRemoteEndDisconnected,
		k_ESNetSocketStateConnectionBroken
	}
}
