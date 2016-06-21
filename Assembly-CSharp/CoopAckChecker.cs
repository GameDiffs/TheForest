using Bolt;
using System;
using TheForest.Utils;
using UnityEngine;

public class CoopAckChecker : GlobalEventListener
{
	public static bool ACKED;

	public static CoopAckChecker Instance;

	private void Awake()
	{
		if (CoopAckChecker.Instance)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			CoopAckChecker.ACKED = false;
			CoopAckChecker.Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	public override void Connected(BoltConnection connection)
	{
		if (BoltNetwork.isServer)
		{
			if (CoopKick.IsBanned(connection.RemoteEndPoint.SteamId))
			{
				connection.Disconnect(new CoopKickToken
				{
					Banned = true,
					KickMessage = "Host banned you from his games"
				});
			}
			else
			{
				ClientACK.Create(connection).Send();
			}
		}
		if (!CoopPeerStarter.DedicatedHost && Scene.HudGui && Scene.HudGui.MpPlayerList && Scene.HudGui.MpPlayerList.gameObject && Scene.HudGui.MpPlayerList.gameObject.activeInHierarchy)
		{
			Scene.HudGui.MpPlayerList.Refresh();
		}
	}

	public override void OnEvent(ClientACK evnt)
	{
		UnityEngine.Object.FindObjectOfType<CoopSteamClientStarter>().CancelInvoke("OnDisconnected");
		Debug.Log("ACKED, Waiting to receive plane position");
		CoopAckChecker.ACKED = true;
	}
}
