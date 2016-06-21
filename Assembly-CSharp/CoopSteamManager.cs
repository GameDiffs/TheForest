using Steamworks;
using System;
using TheForest.Networking;
using UnityEngine;

internal static class CoopSteamManager
{
	private static bool runInit = true;

	private static Callback<LobbyInvite_t> LobbyInvite_Callback;

	private static Callback<P2PSessionRequest_t> P2PSessionRequest_Callback;

	private static Callback<P2PSessionConnectFail_t> P2PSessionConnectFail_Callback;

	public static void Initialize()
	{
		if (CoopSteamManager.runInit)
		{
			CoopSteamManager.runInit = false;
			CoopSteamManager.LobbyInvite_Callback = Callback<LobbyInvite_t>.Create(new Callback<LobbyInvite_t>.DispatchDelegate(CoopSteamManager.LobbyInvite));
			CoopSteamManager.P2PSessionRequest_Callback = Callback<P2PSessionRequest_t>.Create(new Callback<P2PSessionRequest_t>.DispatchDelegate(CoopSteamManager.P2PSessionRequest));
			CoopSteamManager.P2PSessionConnectFail_Callback = Callback<P2PSessionConnectFail_t>.Create(new Callback<P2PSessionConnectFail_t>.DispatchDelegate(CoopSteamManager.P2PSessionConnectFail));
		}
	}

	private static void LobbyInvite(LobbyInvite_t param)
	{
		if (BoltNetwork.isRunning)
		{
			return;
		}
		AutoJoinAfterMPInvite autoJoinAfterMPInvite = UnityEngine.Object.FindObjectOfType<AutoJoinAfterMPInvite>();
		CoopSteamNGUI coopSteamNGUI = UnityEngine.Object.FindObjectOfType<CoopSteamNGUI>();
		if (coopSteamNGUI)
		{
			if (CoopLobby.IsInLobby)
			{
				return;
			}
			coopSteamNGUI.SetJoinText(param);
		}
		else if (autoJoinAfterMPInvite)
		{
			autoJoinAfterMPInvite.SetInvitedToGameId(param.m_ulSteamIDLobby);
		}
	}

	public static void Shutdown()
	{
		CoopSteamManager.P2PSessionRequest_Callback = null;
		CoopSteamManager.P2PSessionConnectFail_Callback = null;
		CoopSteamManager.runInit = true;
	}

	public static void Dump(string tag, P2PSessionState_t s)
	{
		Debug.Log("##### " + tag + " #####");
		Debug.Log("m_bConnecting: " + s.m_bConnecting);
		Debug.Log("m_bConnectionActive: " + s.m_bConnectionActive);
		Debug.Log("m_bUsingRelay: " + s.m_bUsingRelay);
		Debug.Log("m_eP2PSessionError: " + s.m_eP2PSessionError);
		Debug.Log("m_nBytesQueuedForSend: " + s.m_nBytesQueuedForSend);
		Debug.Log("m_nPacketsQueuedForSend: " + s.m_nPacketsQueuedForSend);
		Debug.Log("m_nRemoteIP: " + s.m_nRemoteIP);
		Debug.Log("m_nRemotePort: " + s.m_nRemotePort);
	}

	private static void P2PSessionConnectFail(P2PSessionConnectFail_t param)
	{
		Debug.LogError(string.Concat(new object[]
		{
			"P2PSessionConnectFail: error=",
			param.m_eP2PSessionError,
			", remoteId=",
			param.m_steamIDRemote
		}));
		if (CoopLobby.Instance != null && CoopLobby.Instance.Info != null)
		{
			Debug.LogError("P2PSessionConnectFail: ServerId=" + CoopLobby.Instance.Info.ServerId);
			P2PSessionState_t s;
			if (SteamNetworking.GetP2PSessionState(CoopLobby.Instance.Info.ServerId, out s))
			{
				CoopSteamManager.Dump("Server", s);
			}
			Debug.LogError("P2PSessionConnectFail: OwnerSteamId=" + CoopLobby.Instance.Info.OwnerSteamId);
			if (SteamNetworking.GetP2PSessionState(CoopLobby.Instance.Info.OwnerSteamId, out s))
			{
				CoopSteamManager.Dump("Lobby Owner", s);
			}
		}
		else
		{
			Debug.LogError("P2PSessionConnectFail dump error: " + ((CoopLobby.Instance != null) ? "'CoopLobby.Instance.Info' is null" : "'CoopLobby.Instance' is null"));
		}
	}

	private static void P2PSessionRequest(P2PSessionRequest_t param)
	{
		Debug.Log("CoopSteamManager.P2PSessionRequest (client): remoteId=" + param.m_steamIDRemote);
		SteamNetworking.AcceptP2PSessionWithUser(param.m_steamIDRemote);
	}
}
