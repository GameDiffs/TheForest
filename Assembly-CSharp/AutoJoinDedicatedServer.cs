using Steamworks;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class AutoJoinDedicatedServer : MonoBehaviour
{
	public static bool AutoStartAfterDelay;

	private Callback<GameServerChangeRequested_t> GameServerChangeRequested;

	[DebuggerHidden]
	private IEnumerator Start()
	{
		AutoJoinDedicatedServer.<Start>c__Iterator182 <Start>c__Iterator = new AutoJoinDedicatedServer.<Start>c__Iterator182();
		<Start>c__Iterator.<>f__this = this;
		return <Start>c__Iterator;
	}

	private void OnGameServerChangeRequested(GameServerChangeRequested_t param)
	{
		CoopDedicatedClientStarter.EndPoint = param.m_rgchServer;
		CoopDedicatedClientStarter.Password = param.m_rgchPassword;
		Application.LoadLevel("SteamStartSceneDedicatedServer_Client");
	}
}
