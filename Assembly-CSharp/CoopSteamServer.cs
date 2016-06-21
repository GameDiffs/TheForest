using Steamworks;
using System;
using UnityEngine;

internal static class CoopSteamServer
{
	private static bool runInit = true;

	private static Action Connected;

	private static Action Failed;

	private static Callback<P2PSessionRequest_t> P2PSessionRequest_Callback;

	private static Callback<SteamServersConnected_t> SteamServersConnected_Callback;

	private static Callback<SteamServersDisconnected_t> SteamServersDisconnected_Callback;

	private static Callback<SteamServerConnectFailure_t> SteamServerConnectFailure_Callback;

	public static bool IsConnectedToSteam
	{
		get;
		private set;
	}

	public static CSteamID SteamId
	{
		get
		{
			if (CoopSteamServer.IsConnectedToSteam)
			{
				return SteamGameServer.GetSteamID();
			}
			return default(CSteamID);
		}
	}

	public static bool Start(Action connected, Action failed)
	{
		CoopSteamServer.Connected = connected;
		CoopSteamServer.Failed = failed;
		CoopSteamServer.Initialize();
		if (GameServer.Init(0u, 8766, 27015, 27016, EServerMode.eServerModeNoAuthentication, "0.11.3.0.0"))
		{
			Debug.Log("started steam server");
			SteamGameServer.EnableHeartbeats(true);
			SteamGameServer.SetProduct("The Forest");
			SteamGameServer.SetGameDescription("The Forest Game Description");
			SteamGameServer.LogOnAnonymous();
			return true;
		}
		Debug.LogError("Could not start SteamGameServer");
		CoopSteamServer.Shutdown();
		return false;
	}

	public static void Update()
	{
		if (!CoopSteamServer.runInit)
		{
			GameServer.RunCallbacks();
			if (BoltNetwork.UdpSocket != null)
			{
				SteamSocket steamSocket = (SteamSocket)BoltNetwork.UdpSocket.PlatformSocket;
				if (steamSocket != null)
				{
					steamSocket.Recv(true);
					steamSocket.Send(true);
				}
			}
		}
	}

	public static void Shutdown()
	{
		if (!CoopSteamServer.runInit)
		{
			try
			{
				GameServer.Shutdown();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			finally
			{
				CoopSteamServer.Failed = null;
				CoopSteamServer.Connected = null;
				CoopSteamServer.IsConnectedToSteam = false;
				CoopSteamServer.P2PSessionRequest_Callback = null;
				CoopSteamServer.SteamServersConnected_Callback = null;
				CoopSteamServer.SteamServersDisconnected_Callback = null;
				CoopSteamServer.SteamServerConnectFailure_Callback = null;
				CoopSteamServer.runInit = true;
			}
		}
	}

	private static void Initialize()
	{
		if (CoopSteamServer.runInit)
		{
			CoopSteamServer.runInit = false;
			CoopSteamServer.P2PSessionRequest_Callback = Callback<P2PSessionRequest_t>.CreateGameServer(new Callback<P2PSessionRequest_t>.DispatchDelegate(CoopSteamServer.P2PSessionRequest));
			CoopSteamServer.SteamServersConnected_Callback = Callback<SteamServersConnected_t>.CreateGameServer(new Callback<SteamServersConnected_t>.DispatchDelegate(CoopSteamServer.SteamServersConnected));
			CoopSteamServer.SteamServersDisconnected_Callback = Callback<SteamServersDisconnected_t>.CreateGameServer(new Callback<SteamServersDisconnected_t>.DispatchDelegate(CoopSteamServer.SteamServersDisconnected));
			CoopSteamServer.SteamServerConnectFailure_Callback = Callback<SteamServerConnectFailure_t>.CreateGameServer(new Callback<SteamServerConnectFailure_t>.DispatchDelegate(CoopSteamServer.SteamServerConnectFailure));
		}
	}

	private static void P2PSessionState(P2PSessionState_t param)
	{
		Debug.Log("param.m_bConnectionActive: " + param.m_bConnectionActive);
	}

	private static void P2PSessionRequest(P2PSessionRequest_t param)
	{
		SteamGameServer.EnableHeartbeats(true);
		Debug.Log("CoopSteamServer.P2PSessionRequest (host): remoteId=" + param.m_steamIDRemote);
		SteamGameServerNetworking.AcceptP2PSessionWithUser(param.m_steamIDRemote);
		P2PSessionState_t p2PSessionState_t;
		if (SteamGameServerNetworking.GetP2PSessionState(param.m_steamIDRemote, out p2PSessionState_t))
		{
			Debug.Log("P2P Session with " + param.m_steamIDRemote + " accepted");
		}
		else
		{
			Debug.Log("P2P Session with " + param.m_steamIDRemote + " failed");
		}
	}

	private static void SteamServerConnectFailure(SteamServerConnectFailure_t param)
	{
		CoopSteamServer.IsConnectedToSteam = false;
		if (CoopSteamServer.Failed != null)
		{
			CoopSteamServer.Failed();
			CoopSteamServer.Shutdown();
		}
		CoopSteamServer.Failed = null;
		CoopSteamServer.Connected = null;
	}

	private static void SteamServersDisconnected(SteamServersDisconnected_t param)
	{
		CoopSteamServer.IsConnectedToSteam = false;
		Debug.Log("Server was disconnected from steam");
		if (CoopSteamServer.Failed != null)
		{
			CoopSteamServer.Failed();
			CoopSteamServer.Shutdown();
		}
		CoopSteamServer.Failed = null;
		CoopSteamServer.Connected = null;
	}

	private static void SteamServersConnected(SteamServersConnected_t param)
	{
		CoopSteamServer.IsConnectedToSteam = true;
		Debug.Log("Server is connected to steam");
		if (CoopSteamServer.Connected != null)
		{
			CoopSteamServer.Connected();
		}
		CoopSteamServer.Failed = null;
		CoopSteamServer.Connected = null;
	}
}
