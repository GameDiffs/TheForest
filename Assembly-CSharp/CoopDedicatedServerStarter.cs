using Steamworks;
using System;
using System.Collections;
using System.Diagnostics;
using UdpKit;
using UnityEngine;

public class CoopDedicatedServerStarter : MonoBehaviour
{
	private class RulesResponse : ISteamMatchmakingRulesResponse
	{
		public RulesResponse() : base(new ISteamMatchmakingRulesResponse.RulesResponded(CoopDedicatedServerStarter.RulesResponse.onRulesResponded), new ISteamMatchmakingRulesResponse.RulesFailedToRespond(CoopDedicatedServerStarter.RulesResponse.onRulesFailedToRespond), new ISteamMatchmakingRulesResponse.RulesRefreshComplete(CoopDedicatedServerStarter.RulesResponse.onRulesRefreshComplete))
		{
		}

		private static void onRulesRefreshComplete()
		{
			UnityEngine.Debug.Log("onRulesRefreshComplete");
		}

		private static void onRulesFailedToRespond()
		{
			UnityEngine.Debug.LogError("onRulesFailedToRespond");
		}

		private static void onRulesResponded(string pchRule, string pchValue)
		{
			UnityEngine.Debug.LogFormat("onRulesResponded:pchRule:{0}:pchValue:{1}", new object[]
			{
				pchRule,
				pchValue
			});
		}
	}

	public LoadAsync loadAsync;

	public static UdpEndPoint EndPoint = new UdpEndPoint(UdpIPv4Address.Any, 27015);

	public static string ServerPassword;

	public static string AdminPassword;

	public static int Players;

	public static int AutoSaveIntervalMinutes;

	private CoopSteamServerStarter serverStarter;

	private static uint app_id = 242760u;

	private static Callback<SteamServersConnected_t> SteamServersConnected;

	private static Callback<SteamServersDisconnected_t> SteamServersDisconnected;

	private static Callback<SteamServerConnectFailure_t> SteamServerConnectFailure;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this.loadAsync.gameObject);
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private static void OnSteamServersConnected(SteamServersConnected_t param)
	{
		UnityEngine.Debug.Log("OnSteamServersConnected");
	}

	private static void OnSteamServersDisconnected(SteamServersDisconnected_t param)
	{
		UnityEngine.Debug.Log("OnSteamServersDisconnected");
	}

	private static void OnSteamServerConnectFailure(SteamServerConnectFailure_t param)
	{
		UnityEngine.Debug.Log("OnSteamServerConnectFailure");
	}

	[DebuggerHidden]
	private IEnumerator Start()
	{
		CoopDedicatedServerStarter.<Start>c__Iterator2D <Start>c__Iterator2D = new CoopDedicatedServerStarter.<Start>c__Iterator2D();
		<Start>c__Iterator2D.<>f__this = this;
		return <Start>c__Iterator2D;
	}
}
