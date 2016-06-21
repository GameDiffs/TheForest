using System;
using UdpKit;
using UnityEngine;

public class BoltInit : MonoBehaviour
{
	private enum State
	{
		SelectMode,
		SelectMap,
		EnterServerIp,
		StartServer,
		StartClient,
		Started
	}

	private BoltInit.State state;

	private string map;

	private string serverAddress = "127.0.0.1";

	private int serverPort = 25000;

	private void Awake()
	{
		this.serverPort = BoltRuntimeSettings.instance.debugStartPort;
	}

	private void OnGUI()
	{
		Rect position = new Rect(10f, 10f, 140f, 75f);
		Rect screenRect = new Rect(10f, 90f, (float)(Screen.width - 20), (float)(Screen.height - 100));
		GUI.Box(position, Resources.Load("BoltLogo") as Texture2D);
		GUILayout.BeginArea(screenRect);
		switch (this.state)
		{
		case BoltInit.State.SelectMode:
			this.State_SelectMode();
			break;
		case BoltInit.State.SelectMap:
			this.State_SelectMap();
			break;
		case BoltInit.State.EnterServerIp:
			this.State_EnterServerIp();
			break;
		case BoltInit.State.StartServer:
			this.State_StartServer();
			break;
		case BoltInit.State.StartClient:
			this.State_StartClient();
			break;
		}
		GUILayout.EndArea();
	}

	private void State_EnterServerIp()
	{
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		GUILayout.Label("Server IP: ", new GUILayoutOption[0]);
		this.serverAddress = GUILayout.TextField(this.serverAddress, new GUILayoutOption[0]);
		if (GUILayout.Button("Connect", new GUILayoutOption[0]))
		{
			this.state = BoltInit.State.StartClient;
		}
		GUILayout.EndHorizontal();
	}

	private void State_SelectMode()
	{
		if (this.ExpandButton("Server"))
		{
			this.state = BoltInit.State.SelectMap;
		}
		if (this.ExpandButton("Client"))
		{
			this.state = BoltInit.State.EnterServerIp;
		}
	}

	private void State_SelectMap()
	{
		foreach (string current in BoltScenes.AllScenes)
		{
			if (Application.loadedLevelName != current && this.ExpandButton(current))
			{
				this.map = current;
				this.state = BoltInit.State.StartServer;
			}
		}
	}

	private void State_StartServer()
	{
		BoltLauncher.StartServer(new UdpEndPoint(UdpIPv4Address.Any, (ushort)this.serverPort));
		BoltNetwork.LoadScene(this.map);
		this.state = BoltInit.State.Started;
	}

	private void State_StartClient()
	{
		BoltLauncher.StartClient(UdpEndPoint.Any);
		BoltNetwork.Connect(new UdpEndPoint(UdpIPv4Address.Parse(this.serverAddress), (ushort)this.serverPort));
		this.state = BoltInit.State.Started;
	}

	private bool ExpandButton(string text)
	{
		return GUILayout.Button(text, new GUILayoutOption[]
		{
			GUILayout.ExpandWidth(true),
			GUILayout.ExpandHeight(true)
		});
	}
}
