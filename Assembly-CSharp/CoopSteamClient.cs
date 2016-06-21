using System;
using UdpKit;
using UnityEngine;

public static class CoopSteamClient
{
	private static bool runInit = true;

	private static UdpSocket socket;

	public static void Start()
	{
		CoopSteamClient.Initialize();
	}

	public static void Update()
	{
		if (!CoopSteamClient.runInit)
		{
			if (CoopSteamClient.socket == null)
			{
				CoopSteamClient.socket = BoltNetwork.UdpSocket;
			}
			if (!object.ReferenceEquals(BoltNetwork.UdpSocket, CoopSteamClient.socket) && BoltNetwork.UdpSocket == null && CoopSteamClient.socket != null && !CoopSteamClient.socket.PlatformSocket.IsBound)
			{
				CoopSteamClient.socket = null;
			}
			if (CoopSteamClient.socket != null && CoopSteamClient.socket.PlatformSocket != null)
			{
				SteamSocket steamSocket = (SteamSocket)CoopSteamClient.socket.PlatformSocket;
				steamSocket.Recv(false);
				steamSocket.Send(false);
			}
		}
	}

	public static void Shutdown()
	{
		if (!CoopSteamClient.runInit)
		{
			try
			{
				CoopSteamClient.socket = null;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			finally
			{
				CoopSteamClient.runInit = true;
			}
		}
	}

	private static void Initialize()
	{
		if (CoopSteamClient.runInit)
		{
			CoopSteamClient.runInit = false;
		}
	}

	internal static void PerformReconnect()
	{
	}
}
