using BoltInternal;
using System;
using UdpKit;
using UnityEngine;

public class BoltDebugStart : GlobalEventListenerBase
{
	private UdpEndPoint _serverEndPoint;

	private UdpEndPoint _clientEndPoint;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		this._serverEndPoint = new UdpEndPoint(UdpIPv4Address.Localhost, (ushort)BoltRuntimeSettings.instance.debugStartPort);
		this._clientEndPoint = new UdpEndPoint(UdpIPv4Address.Localhost, 0);
		BoltConfig configCopy = BoltRuntimeSettings.instance.GetConfigCopy();
		configCopy.connectionTimeout = 60000000;
		configCopy.connectionRequestTimeout = 500;
		configCopy.connectionRequestAttempts = 1000;
		if (!string.IsNullOrEmpty(BoltRuntimeSettings.instance.debugStartMapName))
		{
			if (BoltDebugStartSettings.startServer)
			{
				BoltLauncher.StartServer(this._serverEndPoint, configCopy);
			}
			else if (BoltDebugStartSettings.startClient)
			{
				BoltLauncher.StartClient(this._clientEndPoint, configCopy);
			}
			BoltDebugStartSettings.PositionWindow();
		}
		if (BoltNetwork.isClient || !BoltNetwork.isServer)
		{
		}
	}

	public override void BoltStartDone()
	{
		if (BoltNetwork.isServer)
		{
			BoltNetwork.LoadScene(BoltRuntimeSettings.instance.debugStartMapName);
		}
		else
		{
			BoltNetwork.Connect(this._serverEndPoint);
		}
	}

	public override void SceneLoadLocalDone(string arg)
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
